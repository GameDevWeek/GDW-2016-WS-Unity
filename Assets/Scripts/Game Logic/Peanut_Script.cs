﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peanut_Script : MonoBehaviour
{
    private bool m_destroyOnCollision;
    public bool destroyOnCollision
    {
        get { return m_destroyOnCollision; }
        set { m_destroyOnCollision = value; }
    }

    private bool callNoiseEmitterFlag=true;
    private GameObject m_playerGameObject;
    private NoiseSource m_noiseSource;
    
   
	// Use this for initialization
    void Start () {

        //Find Gameobject of the player
        m_playerGameObject= GameObject.FindGameObjectWithTag("Player");
        //Set the projectile physics to ignore the player collision
<<<<<<< HEAD
        //Todo: this trows: MissingComponentException: There is no 'Collider' attached to the "Elephant-WK" game object, but a script is trying to access it.
=======
>>>>>>> 67f2227f5cdfcf8fb0f45993f76aa8efd249a229
        //Physics.IgnoreCollision(GetComponent<Collider>(), m_playerGameObject.GetComponent<Collider>());

        m_noiseSource = GetComponent<NoiseSource>();


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        //Call the noise emitter once per peanut
        if (callNoiseEmitterFlag)
        {
            callNoiseEmitterFlag = false;

            // siwtch tag id needed
            if (collision.gameObject.tag != "Player")
            {
                //todo: call noise emitter here
               // Debug.Log("The script works: " + this.name);

               
                m_noiseSource.Play();
            }

            if (m_destroyOnCollision)
                DestroyObject(this.gameObject);
        }
    }
}
