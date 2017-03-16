using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathCamera : AbstractCamera {

	[SerializeField]
	private float m_targetDistance = 3.0f;

	[SerializeField]
	private float m_showRotationSpeed = 1.0f;

	[SerializeField]
	private float m_zoomSpeed = 1.0f;

	[SerializeField]
	private float m_targetElevation = 0.5f;

	[SerializeField]
	private Vector3 m_targetOffset = new Vector3(0,0,0);

	[SerializeField]
	private float m_lerpFactor = 2.0f;


	private List<AbstractCamera> m_prevCameras = new List<AbstractCamera>();

	private float m_initialDistance;
	private float m_distance;

	private float m_initialElevation;
	private float m_elevation;

	private float m_azimuth;
	private float m_initialAzimuth;
	private float m_targetAzimuth;
	private float m_azimuthStep;

	private bool m_initialZoom = true;
	private float m_timeAcc = 0f;


	public DeathCamera () {
	}

	protected override void FollowTarget(float deltaTime) {
		if (m_initialZoom) {
			m_timeAcc += deltaTime;

			m_azimuth = Mathf.Lerp (m_initialAzimuth, m_targetAzimuth, m_timeAcc * m_zoomSpeed);
			m_elevation = Mathf.Lerp (m_initialElevation, m_targetElevation, m_timeAcc * m_zoomSpeed);
			m_distance = Mathf.Lerp (m_initialDistance, m_targetDistance, m_timeAcc * m_zoomSpeed);

			if (Mathf.Abs (m_azimuth-m_targetAzimuth)<0.001f) {
				m_azimuth = m_targetAzimuth;
				m_initialZoom = false;
			}

		} else {
			m_azimuth += Mathf.Deg2Rad * m_showRotationSpeed * deltaTime;
		}

		Vector3 polar = Util.PolarToVector(m_distance, m_elevation, m_azimuth);

		var targetPos = Target.position + m_targetOffset;

		transform.position = Vector3.Lerp(transform.position, targetPos + polar, deltaTime*m_lerpFactor);
		GetComponent<Transform>().LookAt(targetPos);
	}

	void OnDisable() {
		foreach (var c in m_prevCameras) {
			c.enabled = true;
		}
	}

	void OnEnable() {
		// disable other camera controllers
		m_prevCameras.Clear();

		var cameras = GetComponents<AbstractCamera> ();
		foreach (var c in cameras) {
			if (c != this && c.enabled) {
				m_prevCameras.Add(c);
				c.enabled = false;
			}
		}


		var diffPolar = Util.VectorToPolar (transform.position - Target.position);

		m_initialDistance = m_distance = diffPolar.radius;

		m_initialElevation = m_elevation = diffPolar.elevation;

		m_initialAzimuth = m_azimuth = diffPolar.azimuth;

		m_targetAzimuth = -Mathf.Deg2Rad * Target.rotation.eulerAngles.y;
		m_targetAzimuth -= Mathf.Deg2Rad * m_showRotationSpeed;

		var azimuthDiff = m_targetAzimuth - m_initialAzimuth;
		if (azimuthDiff > 180 * Mathf.Deg2Rad) {
			azimuthDiff = 360 * Mathf.Deg2Rad - azimuthDiff;

		} else if (azimuthDiff < -180 * Mathf.Deg2Rad) {
			azimuthDiff = 360 * Mathf.Deg2Rad + azimuthDiff;
		}

		m_targetAzimuth = m_initialAzimuth + azimuthDiff;

		m_timeAcc = 0f;
		m_initialZoom = true;
	}

}
