using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peanut_Script : MonoBehaviour
{

    public bool destroyOnCollision;
    private bool callNoiseEmitterFlag=true;
	// Use this for initialization
	void Start () {
		

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
                Debug.Log("HIT and The script works: " + this.name);


                if (destroyOnCollision)
                    DestroyObject(this.gameObject);
            }
        }
    }
}
