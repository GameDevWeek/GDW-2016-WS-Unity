using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCollectableObjects : MonoBehaviour {

    public float m_AnglePerSecond = 45;

    private Collectable[] m_pickObjs;

    // Use this for initialization
    void Start () {
        m_pickObjs = FindObjectsOfType<Collectable>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Collectable po in m_pickObjs)
        {
            if(po != null)
            po.transform.Rotate(0, 0 , m_AnglePerSecond * Time.deltaTime);
        }
	}
}
