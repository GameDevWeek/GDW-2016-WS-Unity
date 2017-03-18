using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoiseSource))]
public class DestructibleObject : MonoBehaviour {
    [SerializeField]
    private string m_destructableName;
    [SerializeField]
    private GameObject m_completeObject;
    [SerializeField]
    private GameObject m_destrucedObject;
    
    public struct DestructionEventData
    {
        public GameObject destroyed;

        public DestructionEventData(GameObject destroyed)
        {
            this.destroyed = destroyed;
        }
    }

    public delegate void DestructionEvent(DestructionEventData data);
    public static event DestructionEvent OnDestruction;

    public void DestroyObject(Vector3 force) {
        if (!m_completeObject.activeSelf) {
            return;
        }

        GetComponent<Collider>().enabled = false;
        m_completeObject.SetActive(false);
        m_destrucedObject.SetActive(true);
        GetComponent<NoiseSource>().Play();

        var dTransform = m_destrucedObject.transform;
        for (int i = 0; i < dTransform.childCount; ++i) {
            var rb = dTransform.GetChild(i).GetComponent<Rigidbody>();
            //rb.AddForce(force);
            rb.AddExplosionForce(100.0f, transform.position, 3.0f);
        }

        if (OnDestruction != null)
            OnDestruction.Invoke(new DestructionEventData(this.gameObject));
    }

    public string GetDestructableName()
    {
        return m_destructableName;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_destructableName))
        {
            Debug.LogError("[" + name + "] Destructable Name is Empty!", this.gameObject);
        }
    }
}
