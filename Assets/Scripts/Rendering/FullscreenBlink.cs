using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(PlayerViewconeEffect))]
public class FullscreenBlink : MonoBehaviour {

	[SerializeField]
	private float m_frequency = 1f;

	[SerializeField]
	private float m_maxIntensity = 0.8f;

	[SerializeField]
	private bool m_active = false;

	private PlayerViewconeEffect m_effect;

	private float m_timer = 0f;

	void Awake () {
		m_effect = GetComponent<PlayerViewconeEffect> ();
	}

	void Update () {
		if (m_active || m_effect.FullscreenTint.a > 0f) {
			m_timer += Time.deltaTime;
			var c = m_effect.FullscreenTint;
			c.a = Mathf.Clamp (Mathf.Sin (Mathf.PI * 2f * m_timer * m_frequency) / 2f + 0.5f, 0f, 1f) * m_maxIntensity;

			if (!m_active && c.a < 0.05f) {
				c.a = 0f;
			}

			m_effect.FullscreenTint = c;
		}
	}

	public void EnableEffect() {
		m_active = true;
		m_timer = 0f;
	}

	public void DisableEffect() {
		m_active = false;
	}
}
