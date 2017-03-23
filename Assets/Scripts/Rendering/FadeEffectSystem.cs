using UnityEngine;
using System.Collections;

public class FadeEffectSystem : MonoBehaviour {
    public Texture2D fadeOutTexture;

    [SerializeField]
    private float m_duration = 1.0f;

    [SerializeField]
    private int m_drawDepth = -10;

    private float m_alpha = -1.0f;
    private int m_fadeDir = -1;
    private float m_currentDuration;

    public float duration {
        get { return m_currentDuration; }
        set { m_currentDuration = m_duration = value; }
    }

    void Awake() {

        Debug.Assert(fadeOutTexture);
        m_currentDuration = duration;
    }

    void OnGUI() {
        if (m_alpha < 0.0f)
            return;

        m_alpha += m_fadeDir * Time.deltaTime / m_currentDuration;
        m_alpha = Mathf.Clamp01(m_alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_alpha);
        GUI.depth = m_drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    /// <summary>
    /// Fades in the specified direction: Use -1 to FadeIn (fullscreen texture will disappear) and 1 to FadeOut (fullscreen texture will appear)
    /// </summary>
    public void Fade(int direction, float duration) {
        Debug.Assert(direction == 1 || direction == -1);
        Debug.Assert(duration > 0.0f);

        m_fadeDir = direction;
        m_currentDuration = duration;
        m_alpha = direction == 1 ? 0.0f : 1.0f;
    }

    /// <summary>
    /// Fades in the specified direction: Use -1 to FadeIn (fullscreen texture will disappear) and 1 to FadeOut (fullscreen texture will appear)
    /// </summary>
    public void Fade(int direction) {
        Fade(direction, m_duration);
    }

    public void FadeIn(float duration) {
        Fade(-1, duration);
    }

    public void FadeOut(float duration) {
        Fade(1, duration);
    }

    public void FadeIn() {
        FadeIn(m_duration);
    }

    public void FadeOut() {
        FadeOut(m_duration);
    }
}
