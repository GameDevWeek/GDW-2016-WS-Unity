using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FadeInOut))]
public class FadeAndDestroy : MonoBehaviour {
    private Coroutine coroutine;

    private float currentLifeTime;

    public void FadeInOutAndDestroy(float fadeInDuration, float lifetime, float fadeOutDuration)
    {
        if(coroutine == null)
        {
            coroutine = StartCoroutine(FadeCoroutine(fadeInDuration, lifetime, fadeOutDuration));
        }
        else
        {
            Debug.LogWarning("Destroy already in process!");
        }
    }

    public void ResetDuration()
    {
        currentLifeTime = 0f;
    }

    private IEnumerator FadeCoroutine(float fadeInDuration, float lifetime, float fadeOutDuration)
    {
        FadeInOut fade = GetComponent<FadeInOut>();
        currentLifeTime = 0f;
        fade.FadeIn(fadeInDuration);
        yield return new WaitForSeconds(fadeInDuration);
        while(currentLifeTime < lifetime)
        {
            yield return null;
            currentLifeTime += Time.deltaTime;
        }
        fade.FadeOut(fadeOutDuration);
        yield return new WaitForSeconds(fadeInDuration);
        Destroy(gameObject);
    }
}
