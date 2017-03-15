using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(EdgeDetection))]
public class EdgeDetectionBlink : MonoBehaviour {

	[SerializeField]
	private float m_frequency = 1f;

	[SerializeField]
	private float m_maxIntensity = 0.8f;

	private EdgeDetection m_edgeDetection;

	private float m_timer = 0f;

	private bool m_active = false;

	void Awake () {
		m_edgeDetection = GetComponent<EdgeDetection> ();
	}

	void Update () {
		if (m_active || m_edgeDetection.edgesOnly > 0f) {
			m_timer += Time.deltaTime;
			m_edgeDetection.edgesOnly = Mathf.Clamp (Mathf.Sin (Mathf.PI * 2f * m_timer * m_frequency) / 2f + 0.5f, 0f, 1f) * m_maxIntensity;

			if (!m_active && m_edgeDetection.edgesOnly < 0.05f) {
				m_edgeDetection.edgesOnly = 0f;
			}
		}
	}

	public void EnableEffect() {
		m_active = false;
	}

	public void DisableEffect() {
		m_active = true;
		m_timer = 0f;
	}
}
