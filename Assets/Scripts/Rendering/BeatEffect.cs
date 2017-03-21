using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatEffect : MonoBehaviour {
    [SerializeField]
    private float m_interval = 0.32f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_minScale = 0.1f;
    private Vector3 m_startScale;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_shrinkProportion = 0.8f;
    private Coroutine m_coroutine;

    // Use this for initialization
    void Start () {
        m_startScale = transform.localScale;
    }

    private void OnEnable() {
        if (m_coroutine != null) {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }
    }

    private IEnumerator Beat(float time) {
        float progress = 0.0f;
        float shrinkTime = m_shrinkProportion * time;

        while (shrinkTime > 0.0f) {
            progress += Time.deltaTime / shrinkTime;

            transform.localScale = m_startScale * Mathf.Lerp(1.0f, m_minScale, progress);
            shrinkTime -= Time.deltaTime;
            yield return null;
        }

        float growTime = (1.0f - m_shrinkProportion) * time;
        progress = 0.0f;

        while (growTime > 0.0f) {
            progress += Time.deltaTime / growTime;

            transform.localScale = m_startScale * Mathf.Lerp(m_minScale, 1.0f, progress);
            growTime -= Time.deltaTime;
            yield return null;
        }

        m_coroutine = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_coroutine == null) {
            m_coroutine = StartCoroutine(Beat(m_interval));
        }
    }
}
