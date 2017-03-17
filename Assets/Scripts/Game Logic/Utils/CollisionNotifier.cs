using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionNotifier : MonoBehaviour {
    public delegate void CollisionEnter(Collision collision);
    public event CollisionEnter OnCollisionEnterNotification;

    public delegate void CollisionStay(Collision collision);
    public event CollisionStay OnCollisionStayNotification;

    public delegate void TriggerStay(Collider other);
    public event TriggerStay OnTriggerStayNotification;

    public delegate void TriggerEnter(Collider other);
    public event TriggerEnter OnTriggerEnterNotification;

    public delegate void TriggerExit(Collider other);
    public event TriggerExit OnTriggerExitNotification;

    private void OnCollisionEnter(Collision collision) {
        if (OnCollisionEnterNotification != null) {
            OnCollisionEnterNotification(collision);
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (OnCollisionStayNotification != null) {
            OnCollisionStayNotification(collision);
        }
    }

    public void OnTriggerStay(Collider other) {
        if (OnTriggerStayNotification != null) {
            OnTriggerStayNotification(other);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (OnTriggerExitNotification != null) {
            OnTriggerExitNotification(other);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (OnTriggerEnterNotification != null) {
            OnTriggerEnterNotification(other);
        }
    }
}
