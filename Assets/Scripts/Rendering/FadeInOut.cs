using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class FadeInOut : MonoBehaviour {

    private Renderer[] rendererObjects;
    private ViewCone[] cones;

    private void Start()
    {
        rendererObjects = GetComponentsInChildren<Renderer>();
        cones = GetComponentsInChildren<ViewCone>();
    }

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

        if(rendererObjects != null)
        {
            var rendererList = rendererObjects.ToList();

		    while(rendererList.Count > 0)
            {
                List<bool> done = new List<bool>();
			    for(int i = 0; i < rendererList.Count; ++i)
			    {
                    for (int j = 0; j < rendererList[i].materials.Length; ++j)
                    {
                        if(rendererList[i].materials[j] == null)
                        {
                            continue;
                        }
                    
                        Color color = rendererList[i].materials[j].color;
                        float fadeStep = fadingSpeed * Mathf.Sign(targetAlpha - color.a);
                        color.a = Mathf.Clamp(color.a + Time.deltaTime * fadeStep, 0f, 1f);
                        rendererList[i].materials[j].color = color;
                    }
                    done.Add(rendererList[i].materials.All(mat => mat == null || Mathf.Abs(mat.color.a - targetAlpha) < 0.001f));
                }

                for(int i = done.Count - 1; i >= 0; --i)
                {
                    if(done[i])
                    {
                        rendererList.RemoveAt(i);
                    }
                }

                for(int i = 0; i < cones.Length; ++i)
                {
                    float fadeStep = fadingSpeed * Mathf.Sign(targetAlpha - cones[i].ConeAlpha);
                    cones[i].ConeAlpha = Mathf.Clamp(cones[i].ConeAlpha + Time.deltaTime * fadeStep, 0f, 1f);
                }

                yield return null;
            }
        }
    }

}