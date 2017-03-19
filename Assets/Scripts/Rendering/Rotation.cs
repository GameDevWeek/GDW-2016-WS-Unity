using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    [SerializeField]
    private Vector3 rotationSpeed;
	
	void Update () {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationSpeed * Time.deltaTime);
	}
}
