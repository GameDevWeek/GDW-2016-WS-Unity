using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer : MonoBehaviour {
    [SerializeField]
    private LayerMask m_layer;
    [SerializeField]
    private Cooldown m_afterSeconds = new Cooldown(0.5f);
    private bool m_changedLayer = false;

	void Update () {
        m_afterSeconds.Update(Time.deltaTime);
        if (m_afterSeconds.IsOver() && !m_changedLayer) {
            m_changedLayer = true;
            gameObject.layer = (int) Mathf.Log(m_layer, 2.0f);
        }
    }
}
