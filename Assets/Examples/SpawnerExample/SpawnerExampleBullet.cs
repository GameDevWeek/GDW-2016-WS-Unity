using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpawnerExampleBullet : MonoBehaviour {
    private bool m_hitSomething = false;

    private void OnEnable() {
        m_hitSomething = false;
    }

    public void Shoot(Vector3 velocity) {
        GetComponent<Rigidbody>().velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision) {
        if (m_hitSomething) {
            return;
        }

        Spawner.DeSpawn(gameObject);
        Spawner.Spawn("ParticleEffect", collision.contacts[0].point, 0.0f);
        m_hitSomething = true;
    }
}

