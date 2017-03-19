using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class FootSound : MonoBehaviour {

    [System.Serializable]
    public struct FootSoundPair
    {
        [TagSelector]
        public string GroundTag;
        public AudioClip FootLeft, FootRight;
    }

    public FootSoundPair[] FootSoundPairs;
    private AudioSource m_audioSource;
    private Animator m_Animator;
    private AudioClip CurrentFootLeft, CurrentFootRight;
    private NoiseSource m_NoiseSource;

    private void Start()
    {
        m_audioSource = gameObject.GetComponent<AudioSource>();
        m_Animator = this.GetComponent<Animator>();
        m_NoiseSource = GetComponent<NoiseSource>();
        if (FootSoundPairs.Length > 0)
        {
            CurrentFootLeft = FootSoundPairs[0].FootLeft;
            CurrentFootRight = FootSoundPairs[0].FootRight;
        }
    }

    public void FootSoundLeft(float intensity)
    {
      /*  Mathf.Clamp01(intensity);
        m_audioSource.volume = intensity;
        m_audioSource.PlayOneShot(CurrentFootLeft);
        m_NoiseSource.Play();*/
    }

    public void FootSoundRight(float intensity)
    {
        Mathf.Clamp01(intensity);
        m_audioSource.volume = intensity;
        m_audioSource.PlayOneShot(CurrentFootRight);
        if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
            m_NoiseSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (FootSoundPair fsp in FootSoundPairs)
        {
            if (collision.gameObject.CompareTag(fsp.GroundTag))
            {
                CurrentFootLeft = fsp.FootLeft;
                CurrentFootRight = fsp.FootRight;
                return;
            }
        }
    }
}
