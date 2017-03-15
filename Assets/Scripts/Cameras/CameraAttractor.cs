using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttractor : MonoBehaviour {

	[SerializeField]
	private float m_influenceRadius = 5f;
	[SerializeField]
	private float m_exclusiveRadius = 3f;
	[SerializeField]
	private float m_smoothness = 0.5f;

	private float m_activeFor = 0f;

	private bool m_active = false;

	private TopDownCamera m_camera;

	void Awake () {
		m_camera = Camera.main.GetComponent<TopDownCamera>();
	}

	void LateUpdate () {
		if (m_active) {
			m_activeFor += Time.deltaTime;
		}
	}

	public bool IsExclusive () {
		if (m_camera == null || m_camera.Target == null)
			return false;

		var diff = m_camera.Target.position - transform.position;

		return (diff.x * diff.x + diff.z * diff.z) <= m_exclusiveRadius*m_exclusiveRadius;
	}
	public bool IsActive () {
		if (m_camera == null || m_camera.Target == null)
			return false;

		var diff = m_camera.Target.position - transform.position;
		return (diff.x * diff.x + diff.z * diff.z) <= m_influenceRadius*m_influenceRadius;
	}
	public float LerpFactor () {
		return Mathf.Clamp (m_activeFor * m_smoothness, 0f, 1f);
	}
	public float Influence() {
		if (m_camera == null || m_camera.Target == null)
			return 0f;

		var diff = m_camera.Target.position - transform.position;
		var dist = Mathf.Sqrt (diff.x * diff.x + diff.z * diff.z);
		return 1f - Mathf.SmoothStep (m_exclusiveRadius, m_influenceRadius, dist);
	}

	public void OnTriggerEnter(Collider other) {
		if (m_camera != null && other.transform == m_camera.Target) {
			m_camera.EnableAttractor (this);
			m_active = true;
			m_activeFor = 0f;
		}
	}
	public void OnTriggerExit(Collider other) {
		if (m_camera != null && other.transform == m_camera.Target) {
			m_camera.DisableAttractor (this);
			m_active = false;
		}
	}

}
