using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootSound : MonoBehaviour {

    [System.Serializable]
    public struct FootSoundPair
    {
        [TagSelector]
        public string GroundTag;
        public AudioClip FootLeft, FootRight;
        public float NoiseRange;
    }


    public FootSoundPair[] FootSoundPairs;

    private AudioSource m_SourceLeft;
    private AudioSource m_SourceRight;
    private Animator m_Animator;
    private AudioClip CurrentFootLeft, CurrentFootRight;
    private NoiseSource m_NoiseSource;

    private void Start()
    {
        m_SourceLeft = gameObject.AddComponent<AudioSource>();
        m_SourceRight = gameObject.AddComponent<AudioSource>();
        m_Animator = this.GetComponent<Animator>();
        m_NoiseSource = GetComponent<NoiseSource>();
    }

    public void FootSoundLeft(float intensity)
    {
        Mathf.Clamp01(intensity);
        if (!m_SourceLeft.isPlaying)
        {
            float forwardPower = m_Animator.GetFloat("Forward"); //Value 0-1
            m_SourceLeft.volume = 1 * forwardPower;
            m_SourceLeft.PlayOneShot(CurrentFootLeft, intensity);
            m_NoiseSource.Play();
        }
    }

    public void FootSoundRight(float intensity)
    {
        Mathf.Clamp01(intensity);
        if (!m_SourceRight.isPlaying) {
            float forwardPower = m_Animator.GetFloat("Forward"); //Value 0-1
            m_SourceRight.volume = 1 * forwardPower;
            m_SourceRight.PlayOneShot(CurrentFootRight, intensity);
            m_NoiseSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (FootSoundPair fsp in FootSoundPairs)
        {
            if (collision.gameObject.CompareTag(fsp.GroundTag))
            {
                CurrentFootLeft = fsp.FootLeft;
                CurrentFootRight = fsp.FootRight;
                m_NoiseSource.affectedRange = fsp.NoiseRange;
                return;
            }
        }
    }
}
