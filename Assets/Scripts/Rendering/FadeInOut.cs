using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class FadeInOut : MonoBehaviour {

    private Renderer[] rendererObjects = null;


    public void FadeIn(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1.0f, duration));
    }

    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(0.0f, duration));
    }
    
    private IEnumerator Fade(float targetAlpha, float fadeTime)
    {
        float fadingSpeed = 1.0f / fadeTime;
        
        if (rendererObjects == null) rendererObjects = GetComponentsInChildren<Renderer>();

		while(rendererObjects.Count()>0)
        {
			foreach (var renderer in rendererObjects)
			{
				var color = renderer.material.color;
				var fadeStep = fadingSpeed * Mathf.Sign (targetAlpha - color.a);
				color.a = Mathf.Clamp(color.a + Time.deltaTime * fadeStep, 0f, 1f);
				renderer.material.color = color;
            }

            yield return null;
        }
    }

}