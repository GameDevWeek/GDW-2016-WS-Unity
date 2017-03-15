using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructor : MonoBehaviour {
    public void OnCollisionEnter(Collision collision) {
        var destructible = collision.gameObject.GetComponent<DestructibleObject>();

        if (destructible) {
            destructible.DestroyObject(-collision.contacts[0].normal * 100.0f);
        }
    }
}
