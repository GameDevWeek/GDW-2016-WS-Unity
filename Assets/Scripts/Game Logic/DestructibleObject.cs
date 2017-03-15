using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
    [SerializeField]
    private GameObject m_completeObject;
    [SerializeField]
    private GameObject m_destrucedObject;

    public void DestroyObject(Vector3 force) {
        if (!m_completeObject.activeSelf) {
            return;
        }

        GetComponent<Collider>().enabled = false;
        m_completeObject.SetActive(false);
        m_destrucedObject.SetActive(true);

        var dTransform = m_destrucedObject.transform;
        for (int i = 0; i < dTransform.childCount; ++i) {
            var rb = dTransform.GetChild(i).GetComponent<Rigidbody>();
            //rb.AddForce(force);
            rb.AddExplosionForce(100.0f, transform.position, 3.0f);
        }
    }
}
