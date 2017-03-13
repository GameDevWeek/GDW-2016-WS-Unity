using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NoiseSource))]
public sealed class ShelfActor : MonoBehaviour {
    /*
     * Diese Komponente sorgt dafür, dass ein Regal umgestoßen werden kann
     */

    [Header("used components")]
    public new Rigidbody rigidbody;
    public new Collider collider;
    public NoiseSource noiseSource;

    [Header("config")]
    [SerializeField] private bool forward;
    [SerializeField] private bool backward;


    [Header("debug")]
    private bool fallingForward = true;
    [Range(-1, 1)] private float amount = 0;

    private Vector3 hingePosition;
    private Vector3 hingeXTranslation;

    private Vector3 originalPosition;
    private Quaternion originalRotation;


    private void Start() {
        hingePosition = new Vector3(transform.position.x, 0, transform.position.z);
        hingeXTranslation = transform.right * (collider.bounds.size.x/2);

        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
    }

    private void OnValidate() {
        // falls die komponenten nicht zugewiesen sind, tu dies automatisch
        rigidbody = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<Collider>();
        noiseSource = this.GetComponent<NoiseSource>();
        this.enabled = false;
    }

    private void OnEnable() {
        amount = 0;
    }

    private void Update() {
        const float PI2 = Mathf.PI * 0.5f;

        var delta = 1+Mathf.Tan(PI2 * Mathf.Abs(amount));
        amount = fallingForward ? amount + delta* Time.deltaTime : amount - delta* Time.deltaTime;


        if (amount >= 1 || amount <= -1) {
            EndFalling();
        }

        this.transform.position = originalPosition;    // this is not rly optimized ...
        this.transform.rotation = originalRotation;


        var hinge = amount > 0 ? hingePosition + hingeXTranslation : hingePosition - hingeXTranslation;
        this.transform.RotateAround(hinge, transform.forward, -amount*90);
    }

    private void EndFalling() {
        amount = Mathf.Clamp(amount, -1, 1);
        this.enabled = false;
        noiseSource.Play();
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {

        Gizmos.DrawSphere(hingePosition-hingeXTranslation, 0.1f);

        var pos = new Vector3(transform.position.x, collider.bounds.size.x / 2, transform.position.z);
        var x_translatioin = transform.right * (collider.bounds.size.y / 2 + collider.bounds.size.x/2); // TODO: change to right

        if(forward)
            Gizmos.DrawWireCube(pos + x_translatioin, new Vector3(collider.bounds.size.y, collider.bounds.size.x, collider.bounds.size.z));
        if(backward)
            Gizmos.DrawWireCube(pos - x_translatioin, new Vector3(collider.bounds.size.y, collider.bounds.size.x, collider.bounds.size.z));
    }
    #endif

    private void OnCollisionEnter(Collision col) {
        const float sensitivity = 0.5f;

        Debug.Log("test");

        // TODO: check if player

        if(amount > Single.Epsilon || amount < -Single.Epsilon) return;

        Debug.DrawRay(col.contacts[0].point, -col.contacts[0].normal, Color.red, 2);

        if (Vector3.Dot(col.contacts[0].normal, transform.right) > sensitivity && forward) {
            fallingForward = true;
        }else if (Vector3.Dot(col.contacts[0].normal, transform.right) < -sensitivity && backward) {
            fallingForward = false;
        }
        else {
            return;
        }
        this.enabled = true;
    }

}
