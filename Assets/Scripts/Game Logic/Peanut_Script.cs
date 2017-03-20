using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peanut_Script : MonoBehaviour {

    private bool m_destroyOnCollision;
    public bool destroyOnCollision {
        get { return m_destroyOnCollision; }
        set { m_destroyOnCollision = value; }
    }

    private bool callNoiseEmitterFlag = true;
    private NoiseSource m_noiseSource;


    // Use this for initialization
    void Awake() {
        m_noiseSource = GetComponent<NoiseSource>();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject && collision.gameObject.tag == "Player") {
            return;
        }

        //Call the noise emitter once per peanut
        if (callNoiseEmitterFlag) {
            callNoiseEmitterFlag = false;

            m_noiseSource.Play();

            if (m_destroyOnCollision)
                DestroyObject(this.gameObject);
        }
    }
}
