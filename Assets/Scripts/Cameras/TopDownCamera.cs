using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : AbstractCamera {

	[SerializeField]
	private Vector2 m_noFollowDistance = new Vector2(0.75f, 0.75f);

	[SerializeField]
	private float m_stopFollowDistance = 0.5f;

	[SerializeField]
	private Vector3 m_offset = new Vector3(0,10,-1);

	[SerializeField]
	private Vector3 m_targetOffset = new Vector3(0,0,0);

	[SerializeField]
	private float m_springConstant = 20.0f;

	[SerializeField]
	private float m_lookAheadFactor = 1.0f;

	[SerializeField]
	private float m_lookAheadLerpFactor = 0.5f;

	[SerializeField]
	private float m_minLookAheadVelocity = 1.0f;

	[SerializeField]
	private float m_LerpFactor = 10.0f;

	[SerializeField]
	private Vector3 m_roomCorrectionOffset = new Vector2 (0,0);

	[SerializeField]
	private float m_maxAcceleration = 10.0f;

	private Vector2 m_lastLookAheadVelocity;

	private Vector3 m_velocity;

	private Vector3 m_targetedPosition;

	private Vector3 m_lastTarget;

	private HashSet<CameraAttractor> m_activeAttractors = new HashSet<CameraAttractor>();

	private Camera m_camera;

	private bool m_moving_up = false;
	private bool m_moving_down = false;
	private bool m_moving_left = false;
	private bool m_moving_right = false;

	public Vector3 FocusPoint {
		get { return m_targetedPosition; }
	}

	protected override void Awake () {
		base.Awake ();

		m_camera = GetComponent<Camera> ();

		if (Target != null) {
			m_targetedPosition = m_lastTarget = Target.position;
			transform.position = m_targetedPosition + m_offset;
			transform.LookAt (transform.position - m_offset);
		}
	}

	private Vector3 GetTargetPosition() {
		foreach (var a in m_activeAttractors) {
			if (a.IsExclusive ()) {
				return a.transform.position;
			}
		}

		Vector3 position = Target.position + m_targetOffset;
		if (m_lookAheadFactor > 0f) {
			var targetVelocity = GetTargetVelocity();

			m_lastLookAheadVelocity = Vector2.Lerp (m_lastLookAheadVelocity, targetVelocity, m_lookAheadLerpFactor);

			position.x += m_lastLookAheadVelocity.x * m_lookAheadFactor;
			position.z += m_lastLookAheadVelocity.y * m_lookAheadFactor;
		}

		foreach (var a in m_activeAttractors) {
			if(a.IsActive())
				position = Vector3.Lerp(position, a.transform.position, a.LerpFactor() * a.Influence());
		}

		return position;
	}

	private Vector2 GetTargetVelocity() {
		var rigidBody = Target.GetComponent<Rigidbody> ();
		var v = rigidBody.velocity;
		if (rigidBody != null && (v.x*v.x+v.z*v.z) >= m_minLookAheadVelocity*m_minLookAheadVelocity) {
			return new Vector2(v.x, v.z);
		}

		return new Vector2 (0,0);
	}

	protected override void FollowTarget(float deltaTime) {
		var realTarget = GetTargetPosition();
		var diff = realTarget - m_targetedPosition;
		var screenTarget = m_camera.WorldToViewportPoint (realTarget)*2f - new Vector3(1,1,1);

		var target = m_lastTarget;

		if (m_moving_up || screenTarget.y >= m_noFollowDistance.y) {
			m_moving_up = diff.y >= m_stopFollowDistance;
			target.z = realTarget.z;
		} else if (m_moving_down || screenTarget.y <= -m_noFollowDistance.y) {
			m_moving_down = diff.y <= -m_stopFollowDistance;
			target.z = realTarget.z;
		}

		if (m_moving_right || screenTarget.x >= m_noFollowDistance.x) {
			m_moving_right = diff.x >= m_stopFollowDistance;
			target.x = realTarget.x;
		} else if (m_moving_left || screenTarget.x <= -m_noFollowDistance.x) {
			m_moving_left = diff.x <= -m_stopFollowDistance;
			target.x = realTarget.x;
		}


		if ((m_lastTarget - target).sqrMagnitude > 0.05f) {
			target = Vector3.Lerp (m_lastTarget, target, m_LerpFactor * deltaTime);
		}
		m_lastTarget = target;

		UpdatePosition (m_lastTarget + m_offset, deltaTime);
		LimitPosition ();

		m_targetedPosition = transform.position - m_offset;
		transform.LookAt (m_targetedPosition);
	}

	private Vector3 ViewportToWorldPos(float x, float y) {
		Plane plane = new Plane( Vector3.up,  new Vector3(0,m_targetedPosition.y,0) );
		Ray ray = m_camera.ViewportPointToRay(new Vector3(x,y,0f));
		float distance;
		plane.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}

	private void LimitPosition() {
		var topCenterCam   = ViewportToWorldPos (0.5f,1) + m_roomCorrectionOffset;
		var bottomRightCam = ViewportToWorldPos (1,0) + m_roomCorrectionOffset;
		var bottomLeftCam  = ViewportToWorldPos (0,0) + m_roomCorrectionOffset;

		var p = transform.position;

		foreach (var room in GameObject.FindObjectsOfType<CameraRoom>()) {
			if (!room.IsInside (Target.position + m_targetOffset)) {
				continue;
			}

			var leftOver   = bottomLeftCam.x < room.Left;
			var rightOver  = bottomRightCam.x > room.Right;
			var topOver    = topCenterCam.z > room.Top;
			var bottomOver = bottomRightCam.z < room.Bottom;

			if (leftOver && !rightOver) {
				p.x += room.Left - bottomLeftCam.x;
			} else if (rightOver && !leftOver && (bottomRightCam.x - bottomLeftCam.x) < room.Width*1.5f) {
				p.x += room.Right - bottomRightCam.x;
			}

			if (bottomOver && !topOver) {
				p.z += room.Bottom - bottomRightCam.z;
			} else if (topOver && !bottomOver && (topCenterCam.z - bottomRightCam.z) < room.Height*1.5f) {
				p.z += room.Top - topCenterCam.z;
			}
		}

		transform.position = p;
	}

	private void UpdatePosition(Vector3 target, float deltaTime) {
		var accel = ComputeSpringAccel(transform.position, target, m_velocity);
		if(accel.sqrMagnitude > m_maxAcceleration*m_maxAcceleration)
			accel = accel.normalized * m_maxAcceleration;

		m_velocity += accel * deltaTime;
		transform.position += m_velocity * deltaTime;
	}

	// Computes the acceleration using a critical damping spring model
	protected Vector3 ComputeSpringAccel(Vector3 pos, Vector3 target, Vector3 velocity) {
		var dampingCoefficient = 2.0f * Mathf.Sqrt(m_springConstant);

		var deltaAccel = (target - pos) * m_springConstant;
		var dampingAccel = -velocity * dampingCoefficient;

		return deltaAccel + dampingAccel;
	}

	public void EnableAttractor(CameraAttractor a) {
		m_activeAttractors.Add (a);
	}
	public void DisableAttractor(CameraAttractor a) {
		m_activeAttractors.Remove (a);
	}
}
