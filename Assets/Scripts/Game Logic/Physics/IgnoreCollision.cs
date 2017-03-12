using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class IgnoreCollision : MonoBehaviour {
    [SerializeField]
    private List<Collider> m_ignoreColliders = new List<Collider>();

	void Start () {
        var colliders = GetComponents<Collider>();
        foreach (var myCollider in colliders) {
            foreach (var otherCollider in m_ignoreColliders) {
                Physics.IgnoreCollision(myCollider, otherCollider);
            }
        }
	}
}
