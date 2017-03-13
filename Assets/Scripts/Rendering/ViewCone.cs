using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCone : MonoBehaviour {

    [SerializeField]
    private float viewRadius = 10.0f;
    [SerializeField]
    private LayerMask viewBlockingLayers;
    [SerializeField]
    private string enemyTag;

    private void Update()
    {
        //Collider[] objectsInRange = Physics.OverlapSphere(transform.position, viewRadius, viewBlockingLayers);
    }
}
