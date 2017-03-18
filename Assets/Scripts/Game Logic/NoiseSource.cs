using System.Linq;
using System.Runtime.CompilerServices;
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

    [SerializeField]
    private bool m_allowPlayingSounds = true;

    [Tooltip("Higher value means higher priority.")]
    public int priority = 0;

    public float affectedRange {
        get { return m_affectedRange; }
        set { m_affectedRange = value; }
    }

    public void Start() {
        m_audioSource.playOnAwake = false;
    }

    public void OnDrawGizmosSelected() {
        //Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        //Handles.DrawSolidDisc(transform.position, Vector3.up, m_affectedRange);

        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
        Gizmos.DrawSphere(transform.position, m_affectedRange);
    }

    public void Play() {
        Debug.Log("go: " + this.gameObject);
        var particleSystem = global::Spawner.Spawn("Sound Particle System", transform.position, Quaternion.Euler(90, 0, -45));
        if (particleSystem!= null)
        {
            particleSystem.GetComponent<ParticleSystem>().startLifetime = m_affectedRange / 10;
            Spawner.DeSpawn(particleSystem, m_affectedRange / 10);
        }

        if (m_allowPlayingSounds && m_audioClips.Any()) {
            var clip = Util.RandomElement(m_audioClips);
            Debug.Log(clip.name);
            m_audioSource.PlayOneShot(clip);
        }

        var colliders = Physics.OverlapSphere(transform.position, m_affectedRange, m_affectedLayer);
        foreach (var c in colliders) {
            if (c.GetComponent<INoiseListener>() != null) {
                c.GetComponent<INoiseListener>().Inform(new NoiseSourceData(gameObject, transform.position, priority));
            }
        }
    }

    void Awake() {
        m_audioSource = GetComponent<AudioSource>();
    }
}
