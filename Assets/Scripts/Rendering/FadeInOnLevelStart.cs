using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOnLevelStart : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        var fadeSystem = FindObjectOfType<FadeEffectSystem>();
        if (fadeSystem) {
            fadeSystem.FadeIn();
        }
	}
}
