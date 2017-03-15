using UnityEngine;
using System.Collections;

/// <summary>
/// The GameObject with this component will (alpha) fade after a set duration if 
/// the "Rendering Mode" is set to "Fade" or "Transparent".
/// </summary>
[RequireComponent(typeof(Renderer))]
public class FadeComponent : MonoBehaviour {
    public float delay = 0.0f;
    public float duration = 5.0f;
    public bool fadeOut = true;
    public bool fadeOnRigidbodyRest = false;
    public bool destroyWhenFaded = false;

    private bool m_fadeActive = false;

    /// <summary>
    /// After fading in it will fade out and vise versa.
    /// </summary>
    public bool pingPong = false;

    private Renderer m_renderer;
    private Rigidbody m_rigidbody;

    private float m_durationTimer;
    private float m_delayTimer;

    void Start() {
        m_renderer = GetComponent<Renderer>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        if (!m_fadeActive && fadeOnRigidbodyRest && m_rigidbody) {
            if (m_rigidbody.IsSleeping()) {
                m_fadeActive = true;
            }
        }

        if (!m_fadeActive) {
            return;
        }

        m_delayTimer += Time.deltaTime;
        if (m_delayTimer < delay)
            return;

        var c = m_renderer.material.color;
        float dir = fadeOut ? -1.0f : 1.0f;
        float a = dir * Time.deltaTime / duration;
        m_renderer.material.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(c.a + a));

        if (c.a <= Mathf.Epsilon) {
            Destroy(gameObject);
        }

        if (pingPong) {
            m_durationTimer += Time.deltaTime;

            if (m_durationTimer >= duration) {
                m_durationTimer = 0.0f;
                fadeOut = !fadeOut;
                m_delayTimer = 0.0f;
            }
        }
    }
}
