using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_Peanuts : MonoBehaviour {
    public GameObject m_peanutPrefab;
    public float m_velocity;
    public int m_lifeSpan;
    public Vector3 m_offset;
    public bool m_destroy_Peanut_On_Collison;

    public static event Action peanutWasShot;

    public Cooldown cooldown= new Cooldown(1f);
    public int ammo = 0;

    [SerializeField]
    private Transform m_shootOrigin;
    private ElephantControl m_elephantControl;

    // Use this for initialization
    void Start() {
        m_elephantControl = GetComponent<ElephantControl>();
    }

    void Update() {
        cooldown.Update(Time.deltaTime);
    }

    public void Fire() {
        if(!m_elephantControl.aiming || ! cooldown.IsOver() || ammo < 1) return;

        ammo -= 1;
        cooldown.Start();

        //transform the offset from wolrd location to local
        Vector3 worldOffset = transform.rotation * m_offset;
        Vector3 offsetPosition = transform.position + worldOffset;
        if (m_shootOrigin) {
            offsetPosition = m_shootOrigin.position;
        }
        //Instantiate a clone of the given asset
        GameObject clone = Instantiate(m_peanutPrefab, offsetPosition, transform.rotation);

        //set the velocity of the clone
        clone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * m_velocity);

        if (peanutWasShot != null) {
            peanutWasShot.Invoke();
        }

        //Destroy clone after x seconds
        Destroy(clone, m_lifeSpan);


    }
}
