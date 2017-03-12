using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_strength = 0.1f;

    [SerializeField]
    private float m_speed = 50.0f;

    [SerializeField]
    private float m_duration = 3.0f;

    private bool m_shaking = false;

    private float m_fixedFPS = 60.0f;

    public void Shake() {
        Shake(m_duration, m_strength);
    }

    public void Shake(float duration) {
        Shake(duration, m_strength);
    }

    public void Shake(float duration, float strength) {
        if (duration <= 0.0f)
            return;

        if (!m_shaking)
            StartCoroutine(DoShake(duration, strength));
    }

    private IEnumerator DoShake(float duration, float strength) {
        float randomStart = Random.Range(-10000.0f, 10000.0f);
        float progress = 0.0f;
        m_shaking = true;

        while (progress < 1.0f) {
            progress += Time.deltaTime / duration;
            float damping = 1.0f - Mathf.Clamp01(2.0f * progress - 1.0f);
            float offset = randomStart + m_speed * progress;

            float noiseX = Mathf.PerlinNoise(offset, 0.0f) * 2.0f - 1.0f;
            float noiseY = Mathf.PerlinNoise(0.0f, offset) * 2.0f - 1.0f;

            noiseX *= strength * damping;
            noiseY *= strength * damping;

            // To make it framerate independent
            float t = Time.deltaTime * m_fixedFPS;
            transform.position += (transform.right * noiseX + transform.up * noiseY) * t;

            yield return null;
        }

        m_shaking = false;
    }
}
