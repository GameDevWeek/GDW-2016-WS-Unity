using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour {

    [SerializeField]
    private Vector3 pulse;

    [SerializeField]
    private float frequency;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update ()
    {
        float x = (Mathf.Sin(frequency * Time.time) * 0.5f + 1f) * pulse.x;
        float y = (Mathf.Sin(frequency * Time.time) * 0.5f + 1f) * pulse.y;
        float z =( Mathf.Sin(frequency * Time.time) * 0.5f + 1f) * pulse.z;
        transform.localScale = startScale + new Vector3(x, y, z);
	}
}
