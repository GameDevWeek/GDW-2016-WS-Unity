using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticlePool : MonoBehaviour {

	private static SoundParticlePool m_instance;
	public static SoundParticlePool Instance
	{
		get { return m_instance; }
	}

	private GameObject[] m_particleSystems;
	private bool[] m_isActive;
	[SerializeField]
	private GameObject m_prefab;
	[SerializeField]
	private int m_poolSize;
	private int m_current;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < m_poolSize; ++i)
		{
			ParticleSystem system = m_particleSystems[i].GetComponent<ParticleSystem>();
			if (m_isActive[i] && !system.isPlaying)
				m_isActive[i] = false;
		}
	}

	void Awake() {
		m_instance = this;

		m_particleSystems = new GameObject[m_poolSize];
		m_isActive = new bool[m_poolSize];
		for(int i = 0; i < m_poolSize; ++i) {
			m_particleSystems[i] = Instantiate(m_prefab);
			m_isActive[i] = false;
		}
	}

	public void Play(Vector3 startPosition, float volume)
	{
		if(m_isActive[m_current])
			Debug.Log("SoundParticlePool.m_poolSize zu klein");
		
		ParticleSystem system = m_particleSystems[m_current].GetComponent<ParticleSystem>();
		system.transform.position = startPosition;
		system.startLifetime = volume / 10;
		system.Play();
		m_isActive[m_current] = true;
		m_current = (m_current + 1) % m_poolSize;
	}
}
