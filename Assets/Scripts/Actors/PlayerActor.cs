using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerActor : Singleton<PlayerActor> {

    public Collider collider;

    void OnValidate()
    {
        collider = collider == null ? GetComponent<Collider>() : collider;
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
