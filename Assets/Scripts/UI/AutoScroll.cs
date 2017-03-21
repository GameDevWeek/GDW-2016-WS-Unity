using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour {
    private ScrollRect m_scrollRect;
    [Range(0.0f, 1.0f)]
    public float val = 0.0f;
	// Use this for initialization
	void Start () {
        m_scrollRect = GetComponent<ScrollRect>();

    }
	
	// Update is called once per frame
	void Update () {
        m_scrollRect.verticalNormalizedPosition = val;

    }
}
