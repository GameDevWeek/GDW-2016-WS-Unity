using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{


    private int m_peanuts;

    public int peanuts
    {
        get { return m_peanuts; }
        set { m_peanuts = value; }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addPeanuts(int p)
    {
        m_peanuts += p;
    }
}
