using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_Peanuts : MonoBehaviour
{
    public GameObject m_peanutAsset;
    public float m_velocity;
    public int m_lifeSpan;
    public Vector3 m_offset;
    public bool m_destroy_Peanut_On_Collison;
    

    private Rigidbody peanutRigidbody;


    // Use this for initialization
    void Start ()
    {

        peanutRigidbody = m_peanutAsset.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        //for testing
        if (Input.GetKeyDown(KeyCode.J))
        {
            Fire();
        }
    }

    void Fire()
    {
        //transform the offset from wolrd location to local
        Vector3 worldOffset = transform.rotation * m_offset;
        Vector3 offsetPosition = transform.position + worldOffset;

        //Instantiate a clone of the given asset
        GameObject clone;
        clone = (GameObject)Instantiate(m_peanutAsset, offsetPosition, transform.rotation);
        clone.AddComponent<Peanut_Script>();
        clone.GetComponent<Peanut_Script>().destroyOnCollision = m_destroy_Peanut_On_Collison;

        //set the velocity of the clone
        clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * m_velocity);

        //Destroy clone after x seconds
        Destroy(clone,m_lifeSpan);

    }
}
