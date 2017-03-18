using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

	[SerializeField]
	private AudioClip[] m_music;

	[SerializeField]
	private float m_crossFadeTime = 1;

	private AudioSource[] m_audioSources;

	private int m_currentSource = 0;
	private int m_lastSource = 1;

	private float m_crossFadeTimer;

	private WantedLevel m_wantedLevel;


	void Start () {
		m_audioSources = GetComponents<AudioSource> ();
		if (m_audioSources.Length < 2) {
			Debug.LogError ("To few AudioSources on MusicController. Crossfade is disabled.");
		}

		m_wantedLevel = GameObject.FindObjectOfType<WantedLevel> ();

		m_crossFadeTimer = m_crossFadeTime;
	}

	void Update () {
		// if last crossfade done => check for new one
		if (m_crossFadeTimer >= m_crossFadeTime) {
			CheckForMusicChange ();
		}

		if (m_audioSources.Length <= 1) {
			return;
		}

		if (m_crossFadeTimer < m_crossFadeTime) {
			m_crossFadeTimer += Time.unscaledDeltaTime;

			CrossFade (Mathf.Sin(m_crossFadeTimer / m_crossFadeTime * Mathf.PI*0.5f));
		}
	}

	private void CheckForMusicChange() {
		if (m_audioSources.Length <= 0) {
			return;
		}

		if (m_wantedLevel == null || m_wantedLevel.currentWantedStage < m_music.Length) {
			FadeTo( m_music[m_wantedLevel.currentWantedStage] );
		}
	}

	private void FadeTo(AudioClip clip) {
		if (m_audioSources [m_currentSource].clip != clip) {

			if (m_audioSources.Length >= 2) {
				var tmp = m_lastSource;
				m_lastSource = m_currentSource;
				m_currentSource = tmp;

				m_crossFadeTimer = 0;
			}

			m_audioSources [m_currentSource].clip = clip;
			m_audioSources [m_currentSource].volume = 0f;
			m_audioSources [m_currentSource].Play ();
		}
	}

	private void CrossFade(float alpha) {
		if (m_audioSources.Length < 2) {
			return;
		}

		if (alpha >= 0.9999f) {
			alpha = 1f;
		}

		m_audioSources [m_currentSource].volume = alpha;
		m_audioSources [m_lastSource].volume = 1f-alpha;
	}

}
