using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugGizmo : MonoBehaviour {

public float range = 10.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, range);
	}
}
