using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowScore : MonoBehaviour {

    public string m_scoreName;

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = ScoreSystem.GetScore(m_scoreName)+"";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
