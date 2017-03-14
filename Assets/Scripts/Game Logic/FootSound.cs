using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootSound : MonoBehaviour {

    [System.Serializable]
    public struct FootSoundPair
    {
        [TagSelector]
        public string GroundTag;
        public AudioClip FootLeft, FootRight;
    }


    public FootSoundPair[] FootSoundPairs;

    private AudioSource m_SourceLeft;
    private AudioSource m_SourceRight;
    private Animator m_Animator;
    private AudioClip CurrentFootLeft, CurrentFootRight;

    private void Start()
    {
        m_SourceLeft = gameObject.AddComponent<AudioSource>();
        m_SourceRight = gameObject.AddComponent<AudioSource>();
        m_Animator = this.GetComponent<Animator>();
    }

    public void FootSoundLeft(float intensity)
    {
        Mathf.Clamp01(intensity);
        if (!m_SourceLeft.isPlaying)
        {
            float forwardPower = m_Animator.GetFloat("Forward"); //Value 0-1
            m_SourceLeft.volume = 1 * forwardPower;
            m_SourceLeft.PlayOneShot(CurrentFootLeft, intensity);
        }
    }

    public void FootSoundRight(float intensity)
    {
        Mathf.Clamp01(intensity);
        if (!m_SourceRight.isPlaying) {
            float forwardPower = m_Animator.GetFloat("Forward"); //Value 0-1
            m_SourceRight.volume = 1 * forwardPower;
            m_SourceRight.PlayOneShot(CurrentFootRight, intensity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (FootSoundPair fsp in FootSoundPairs)
        {
            if (fsp.GroundTag == collision.gameObject.tag)
            {
                CurrentFootLeft = fsp.FootLeft;
                CurrentFootRight = fsp.FootRight;
                return;
            }
        }
    }
}
