using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveComponents : MonoBehaviour {
    [SerializeField]
    private string[] m_components;
    public Cooldown removeAfter = new Cooldown(1.0f);
    public bool removeOnRigidbodyRest = true;

    private Rigidbody m_rigidbody;
    private bool m_removed = false;

	void Start () {
        m_rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update () {
        if (m_removed) {
            return;
        }

		if (removeOnRigidbodyRest) {
            if (m_rigidbody && m_rigidbody.IsSleeping()) {
                Remove();
            }
        } else {
            if (removeAfter.IsOver()) {
                Remove();
            }
        }

        removeAfter.Update(Time.deltaTime);
    }

    void Remove() {
        m_removed = true;
        foreach (var c in m_components) {
            Destroy(GetComponent(c));
        }
    }
}
