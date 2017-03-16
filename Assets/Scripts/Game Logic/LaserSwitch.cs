using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [SerializeField]
    private LaserDetection[] m_laser;

    [SerializeField]
    private Color m_switchOnColor;
    [SerializeField]
    private Color m_switchOffColor;

    [SerializeField]
    private bool m_isSwitchPowered;

    private Renderer rend;

    // Use this for initialization
    void Start ()
    {
        ChangeColor(m_isSwitchPowered ? m_switchOnColor : m_switchOffColor);

    }

    // Update is called once per frame
	void Update () {
		
        //TODO: insert the interactive method

	}

    public void ChangeColor(Color c)
    {
        gameObject.GetComponent<Renderer>().material.color = c;

    }

    public void ToggleSwitch(bool powered)
    {
        m_isSwitchPowered = powered;
        ChangeColor(m_isSwitchPowered ? m_switchOnColor : m_switchOffColor);

        SwitchLaser(m_isSwitchPowered);
    }


    public void SwitchLaser(bool b)
    {
        foreach (var laser in m_laser)
        {
            laser.SetActivation(b);
        }
    }
}
