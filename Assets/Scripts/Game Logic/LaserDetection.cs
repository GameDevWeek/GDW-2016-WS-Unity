using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NoiseSource))]
[ExecuteInEditMode]
public class LaserDetection : MonoBehaviour {

    private LineRenderer m_laserBeam;

    private BoxCollider m_collider;

    private bool m_isActive = true;
    private NoiseSource noiseSource;

    public Vector3 direction = Vector3.forward;

    private void OnValidate() {
        Awake();
        this.gameObject.layer = GameLayer.Laser;
        this.direction.Normalize();
        this.m_collider.isTrigger = true;
        Start();


    }

    void Awake() {
        m_laserBeam = GetComponent<LineRenderer>();
        m_collider = GetComponent<BoxCollider>();
        noiseSource = GetComponent<NoiseSource>();
    }

    // Use this for initialization
	void Start () {
        m_laserBeam.material = new Material(Shader.Find("Mobile/Particles/Additive"));

        RaycastHit hit;
	    Vector3 localdir = transform.localToWorldMatrix * direction;
        if (Physics.Raycast(transform.position, localdir, out hit, maxDistance: 20, layerMask: ~GameLayer.LaserMask))
        {
            if (hit.collider)
            {

                m_laserBeam.SetPosition(0, transform.position);
                m_laserBeam.SetPosition(1, hit.point);

                var orth = Vector3.Cross(Vector3.up, direction).normalized;


                m_collider.size = direction * hit.distance + Vector3.up + orth * 0.25f;// (Vector3)(transform.localToWorldMatrix * Vector3.up) + localdir * hit.distance + orth * 0.0625f;
                m_collider.center = direction * (hit.distance / 2);
            }
        }
    }

    private IEnumerator SendAlarm() {

        for (int i = 0; i < 10; ++i) {
            noiseSource.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }

	// Update is called once per frame
	void Update () {
        // Adjust length of the rendered laser beam
        RaycastHit hit;
        Vector3 localdir = transform.localToWorldMatrix * direction;
        if (Physics.Raycast(transform.position, localdir, out hit, maxDistance: 20, layerMask: ~GameLayer.LaserMask))
        {
            if (hit.collider)
            {

                m_laserBeam.SetPosition(0, transform.position);
                m_laserBeam.SetPosition(1, hit.point);

                var orth = Vector3.Cross(Vector3.up, direction).normalized;


                m_collider.size = direction * hit.distance + Vector3.up + orth * 0.25f;// (Vector3)(transform.localToWorldMatrix * Vector3.up) + localdir * hit.distance + orth * 0.0625f;
                m_collider.center = direction * (hit.distance / 2);
            }
        }

        // Test code
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetActivation(!m_isActive);
        }
    }

    public void SetActivation(bool active)
    {
        m_isActive = active;

        if (active)
        {
            m_laserBeam.enabled = true;
            m_collider.enabled = true;
        }
        else
        {
            m_laserBeam.enabled = false;
            m_collider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_isActive)
        {
            Debug.Log("LASER ALARM ALARM");
            
            WantedLevel.Instance.TriggerLaserAlert();

            SendAlarm();

            // Create noise sound
        }
    }
}
