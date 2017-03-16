using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : Interactable
{
    [SerializeField]
    private LaserDetection[] m_laser;

    [SerializeField]
    private Color m_switchOnColor;
    [SerializeField]
    private Color m_switchOffColor;

    [SerializeField]
    private bool m_switchPoweredOn = true;

    // Use this for initialization
    void Start ()
    {
        ChangeColor(m_switchPoweredOn ? m_switchOnColor : m_switchOffColor);
    }

    // Update is called once per frame
	void Update () {

	}

    public void ChangeColor(Color c)
    {
        gameObject.GetComponent<Renderer>().material.color = c;
    }

    public void ToggleSwitch()
    {
        m_switchPoweredOn = !m_switchPoweredOn;
        ChangeColor(m_switchPoweredOn ? m_switchOnColor : m_switchOffColor);

        SwitchLaser(m_switchPoweredOn);
    }


    public void SwitchLaser(bool b)
    {
        foreach (var laser in m_laser)
        {
            laser.SetActivation(b);
        }
    }

    public override void Interact(Interactor interactor)
    {
        ToggleSwitch();
    }
}
