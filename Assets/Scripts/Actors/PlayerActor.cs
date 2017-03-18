using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class PlayerActor : MonoBehaviour {

    public Collider collider;
    public Shoot_Peanuts shootPeanuts;
    public CamouflageController camouflageController;
    public GameObject particlePrefab;
    [HideInInspector]
    public ParticleSystem sprintParticles;

    void OnValidate()
    {
        collider = collider == null ? GetComponent<Collider>() : collider;
        shootPeanuts = shootPeanuts == null ? GetComponent<Shoot_Peanuts>() : shootPeanuts;
        camouflageController = camouflageController == null ? GetComponent<CamouflageController>() : camouflageController;
        sprintParticles = sprintParticles == null ? particlePrefab.GetComponent<ParticleSystem>() : sprintParticles;
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
