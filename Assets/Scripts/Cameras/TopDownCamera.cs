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

	private bool m_moving = true;

	private Vector2 m_velocity;

	private Vector3 m_targetedPosition;

	private Vector3 m_lastTarget;

	protected override void Awake () {
		base.Awake ();
		if (Target != null) {
			m_targetedPosition = Target.position;
		}
	}

	protected override void FollowTarget(float deltaTime) {
		var realTarget = Target.position + m_targetOffset;
		var screenTarget = GetComponent<Camera> ().WorldToViewportPoint (realTarget)*2f - new Vector3(1,1,1);

		if (m_moving || Mathf.Abs(screenTarget.x) >= m_noFollowDistance.x || Mathf.Abs (screenTarget.y) > m_noFollowDistance.y) {
			if (m_lookAheadFactor > 0f) {
				var rigidBody = Target.GetComponent<Rigidbody> ();
				if (rigidBody != null) {
					if (Mathf.Abs (rigidBody.velocity.x) > m_minLookAheadVelocity) {
						realTarget.x += rigidBody.velocity.x * m_lookAheadFactor;
					}
					if (Mathf.Abs (rigidBody.velocity.z) > m_minLookAheadVelocity) {
						realTarget.z += rigidBody.velocity.z * m_lookAheadFactor;
					}
				}
			}

			var diff = realTarget - m_targetedPosition;
			m_moving = Mathf.Abs(diff.x*diff.x + diff.y*diff.y) > m_stopFollowDistance;
			m_lastTarget = realTarget;
		}

		UpdatePosition (m_lastTarget, deltaTime);
		transform.position = m_targetedPosition + m_offset;
		transform.LookAt (m_targetedPosition);
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
}
