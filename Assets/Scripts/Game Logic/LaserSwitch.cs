using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : MonoBehaviour
{

    [SerializeField]
    private GameObject[] m_Laser;

    [SerializeField]
    private Color m_switchOnColor;
    [SerializeField]
    private Color m_switchOffColor;

    [SerializeField]
    private bool m_isSwitchOn;

    private Renderer rend;

    // Use this for initialization
    void Start ()
    {
        ChangeSwitchColor(m_isSwitchOn ? m_switchOnColor : m_switchOffColor);
    }

    // Update is called once per frame
	void Update () {
		
        //TODO: insert the interactive method

	}

    public void ChangeSwitchColor(Color c)
    {
        gameObject.GetComponent<Renderer>().material.color = c;

    }

    public void ToggleSwitch(bool b)
    {
        m_isSwitchOn = b;
        ChangeSwitchColor(m_isSwitchOn ? m_switchOnColor : m_switchOffColor);

        SwitchLaser(m_isSwitchOn);
    }


    public void SwitchLaser(bool b)
    {
       
        foreach (var NULL in m_Laser)
        {
            //TODO: insert here the switch Method from Laser Object
        }
    }
}
