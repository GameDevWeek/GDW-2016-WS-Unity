using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour {

    private LineRenderer m_laserBeam;

    private BoxCollider m_collider;

    private bool m_isActive = true;

	// Use this for initialization
	void Start () {
        m_laserBeam = GetComponent<LineRenderer>();
        m_laserBeam.material = new Material(Shader.Find("Mobile/Particles/Additive"));

        m_collider = GetComponent<BoxCollider>();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit))
        {
            if (hit.collider)
            {
                Debug.Log("HitDistance = " + hit.distance);

                m_laserBeam.SetPosition(0, transform.position);
                m_laserBeam.SetPosition(1, hit.point);
                
                m_collider.size = new Vector3(m_collider.size.x, hit.distance / transform.lossyScale.y, m_collider.size.z);                
                m_collider.center = new Vector3(m_collider.center.x, (hit.distance / transform.lossyScale.y) / 2f, m_collider.center.z);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up, out hit))
        {
            if(hit.collider)
            {
                m_laserBeam.SetPosition(0, transform.position);
                m_laserBeam.SetPosition(1, hit.point);

                m_collider.size = new Vector3(m_collider.size.x, hit.distance / transform.lossyScale.y, m_collider.size.z);
                m_collider.center = new Vector3(m_collider.center.x, (hit.distance / transform.lossyScale.y) / 2f, m_collider.center.z);
            }
        }
        else
        {
            m_laserBeam.SetPosition(0, transform.position);
            m_laserBeam.SetPosition(1, transform.position + transform.up * 20f);

            m_collider.size = new Vector3(m_collider.size.x, 20f / transform.lossyScale.y, m_collider.size.z);
            m_collider.center = new Vector3(m_collider.center.x, (20f / transform.lossyScale.y) / 2f, m_collider.center.z);
        }

        // Test code
        if(Input.GetKeyDown(KeyCode.Space))
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

            // Create noice sound
        }
    }
}
