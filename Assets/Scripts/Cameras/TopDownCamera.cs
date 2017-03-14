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
	protected float m_springConstant = 20.0f;

	[SerializeField]
	protected float m_lookAheadFactor = 1.0f;

	[SerializeField]
	protected float m_minLookAheadVelocity = 1.0f;

	[SerializeField]
	protected float m_LerpFactor = 10.0f;

	private bool m_moving = true;

	private Vector2 m_velocity;

	private Vector3 m_targetedPosition;

	private Vector3 m_lastTarget;

	private HashSet<CameraAttractor> m_activeAttractors = new HashSet<CameraAttractor>();

	private Camera m_camera;

	public Vector3 FocusPoint {
		get { return m_targetedPosition; }
	}

	protected override void Awake () {
		base.Awake ();

		m_camera = GetComponent<Camera> ();

		if (Target != null) {
			m_targetedPosition = Target.position;
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
			var rigidBody = Target.GetComponent<Rigidbody> ();
			if (rigidBody != null) {
				if ((rigidBody.velocity.x * rigidBody.velocity.x + rigidBody.velocity.y * rigidBody.velocity.y) >= m_minLookAheadVelocity) {
					position.x += rigidBody.velocity.x * m_lookAheadFactor;
					position.z += rigidBody.velocity.z * m_lookAheadFactor;
				}
			}
		}

		foreach (var a in m_activeAttractors) {
			if(a.IsActive())
				position = Vector3.Lerp(position, a.transform.position, a.LerpFactor() * a.Influence());
		}

		return position;
	}

	protected override void FollowTarget(float deltaTime) {
		var realTarget = GetTargetPosition();
		var screenTarget = m_camera.WorldToViewportPoint (realTarget)*2f - new Vector3(1,1,1);

		if (m_moving || Mathf.Abs(screenTarget.x) >= m_noFollowDistance.x || Mathf.Abs (screenTarget.y) > m_noFollowDistance.y) {
			var diff = realTarget - m_targetedPosition;
			m_moving = Mathf.Abs(diff.x*diff.x + diff.y*diff.y) > m_stopFollowDistance;
			m_lastTarget = realTarget;
		}

		UpdatePosition (m_lastTarget, deltaTime);

		transform.position = Vector3.Lerp(transform.position, m_targetedPosition + m_offset, m_LerpFactor*deltaTime);
		LimitPosition ();

		transform.LookAt (transform.position - m_offset);
	}

	private Vector3 ViewportToWorldPos(float x, float y) {
		Plane plane = new Plane( Vector3.up,  new Vector3(0,m_targetedPosition.y,0) );
		Ray ray = m_camera.ViewportPointToRay(new Vector3(x,y,0f));
		float distance;
		plane.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}

	private void LimitPosition() {
		var topCenterCam = ViewportToWorldPos (0.5f,1);
		var bottomRightCam = ViewportToWorldPos (1,0);
		var bottomLeftCam = ViewportToWorldPos (0,0);

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
		Vector2 accel = ComputeSpringAccel(new Vector2(m_targetedPosition.x, m_targetedPosition.z), new Vector2(target.x, target.z), m_velocity);
		m_velocity += accel * deltaTime;
		m_targetedPosition.x += m_velocity.x * deltaTime;
		m_targetedPosition.z += m_velocity.y * deltaTime;
	}

	// Computes the acceleration using a critical damping spring model
	protected Vector2 ComputeSpringAccel(Vector2 pos, Vector2 target, Vector2 velocity) {
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
