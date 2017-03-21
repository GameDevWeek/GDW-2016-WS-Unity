using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePeanut : Collectable {
    public override void Interact(Interactor interactor) {
        base.Interact(interactor);

        Shoot_Peanuts shootPeanuts = interactor.GetComponent<Shoot_Peanuts>();

        if (shootPeanuts) {
            shootPeanuts.ammo++;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
