using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour
{
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
        
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        
        float currentAlpha = CurrentAlpha();
        bool fadingIn = targetAlpha > currentAlpha;
        if (!fadingIn)
        {
            fadingSpeed = -fadingSpeed;
        }

        while ((fadingIn && currentAlpha < targetAlpha) || (!fadingIn && currentAlpha > targetAlpha))
        {
            currentAlpha += Time.deltaTime * fadingSpeed;

            for (int i = 0; i < rendererObjects.Length; i++)
            {
                var color = rendererObjects[i].material.color;
                rendererObjects[i].material.color = new Color(color.r, color.g, color.b, Mathf.Clamp(currentAlpha, 0.0f, 1.0f));
                //Color newColor = rendererObjects[i].material.color;
                //newColor.a = Mathf.Clamp(currentAlpha, 0.0f, 1.0f);
                //rendererObjects[i].material.SetColor("_Color", newColor);
            }

            yield return null;
        }
    }
    
    private float CurrentAlpha()
    {
        float maxAlpha = 0.0f;
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererObjects)
        {
            maxAlpha = Mathf.Max(maxAlpha, item.material.color.a);
        }
        return maxAlpha;
    }
}