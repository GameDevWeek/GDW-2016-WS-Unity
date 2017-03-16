using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoiseSource : MonoBehaviour {
    [SerializeField]
    private float m_affectedRange = 3.0f;
    [SerializeField]
    private AudioClip[] m_audioClips;
    [SerializeField]
    private LayerMask m_affectedLayer;

    private AudioSource m_audioSource;

    public float affectedRange {
        get { return m_affectedRange; }
        set { m_affectedRange = value; }
    }

    public void OnDrawGizmosSelected() {
        //Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        //Handles.DrawSolidDisc(transform.position, Vector3.up, m_affectedRange);

        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
        Gizmos.DrawSphere(transform.position, m_affectedRange);
    }

    public void Play() {
        var particleSystem = global::Spawner.Spawn("Sound Particle System", transform.position, Quaternion.Euler(90, 0, -45));
        if (particleSystem!= null)
        {
            particleSystem.GetComponent<ParticleSystem>().startLifetime = m_affectedRange / 10;
            Spawner.DeSpawn(particleSystem, m_affectedRange / 10);
        }

        if (m_audioClips.Length > 0) {
            m_audioSource.clip = Util.RandomElement(m_audioClips);
            
        }

        m_audioSource.Play();
        var colliders = Physics.OverlapSphere(transform.position, m_affectedRange, m_affectedLayer);
        foreach (var c in colliders) {
            if (c.GetComponent<INoiseListener>() != null) {
                c.GetComponent<INoiseListener>().Inform(new NoiseSourceData(gameObject, transform.position));
            }
        }
    }

    void Awake() {
        m_audioSource = GetComponent<AudioSource>();
    }
}
