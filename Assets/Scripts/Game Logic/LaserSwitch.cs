using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : Interactable
{
    [SerializeField]
    private LaserDetection[] m_laserBarriers;

    [SerializeField]
    private Material m_switchOnMaterial;
    [SerializeField]
    private Material m_switchOffMaterial;

    [SerializeField]
    private bool m_switchPoweredOn = true;

    [SerializeField]
    private Renderer m_renderer;

    // Use this for initialization
    void Start ()
    {
        // Set color of switch light based on initial status
        m_renderer.material = m_switchPoweredOn ? m_switchOnMaterial : m_switchOffMaterial;
        // Set activation of related laser barriers based on initial status
        //SetBarrierActivation();
        //ToggleSwitch();
    }

    // Update is called once per frame
	void Update ()
    {
        // Test code
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleSwitch();
        }
    }

    public void ToggleSwitch()
    {
        // Toggle between on & off 
        m_switchPoweredOn = !m_switchPoweredOn;
        // Change color of switch light based on current status
        m_renderer.material = m_switchPoweredOn ? m_switchOnMaterial : m_switchOffMaterial;
        // Set the activation of the associated laser barriers
        SetBarrierActivation();
    }
    
    private void SetBarrierActivation()
    {
        foreach (var laser in m_laserBarriers)
        {
            if (!laser) {
                continue;
            }

            laser.Toggle();
            //laser.SetActivation(m_switchPoweredOn);
        }
    }

    public override void Interact(Interactor interactor)
    {
        ToggleSwitch();
    }
}
