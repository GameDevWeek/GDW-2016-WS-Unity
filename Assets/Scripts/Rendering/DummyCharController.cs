using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCharController : MonoBehaviour {

    private Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) < 0.025) x = 0;
        if (Mathf.Abs(z) < 0.025) z = 0;
        rigid.velocity = new Vector3(x * 6, 0, z * 6);

        transform.LookAt(transform.position + rigid.velocity * 2);
    }
}
