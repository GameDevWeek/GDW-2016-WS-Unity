using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionNotifier : MonoBehaviour {
    public delegate void CollisionEnter(Collision collision);
    public event CollisionEnter OnCollisionEnterNotification;

    public delegate void CollisionStay(Collision collision);
    public event CollisionStay OnCollisionStayNotification;

    private void OnCollisionEnter(Collision collision) {
        if (OnCollisionEnterNotification != null) {
            OnCollisionEnterNotification(collision);
        }
        Debug.Log("hmm");
    }

    private void OnCollisionStay(Collision collision) {
        if (OnCollisionStayNotification != null) {
            OnCollisionStayNotification(collision);
        }
        
    }
}
