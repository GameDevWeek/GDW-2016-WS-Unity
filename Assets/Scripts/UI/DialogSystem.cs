using System;
using UnityEngine;
using UnityEngine.UI;

public enum Speaker {
	player = 0
}

public class DialogSystem : MonoBehaviour {

	[SerializeField]
	private GameObject[] m_showOnDialog;

	[SerializeField]
	private GameObject[] m_hideOnDialog;

	[SerializeField]
	private Text m_textBox;

	[SerializeField]
	private Image m_speakerImageBg;

	[SerializeField]
	private Image m_speakerImageFg;

	[SerializeField]
	private AudioClip m_characterSound;

	[SerializeField]
	private int m_charactersPerSecond = 1;

	[SerializeField]
	private float m_spaceDelay = 0.5f;

	[SerializeField]
	private float m_newlineDelay = 0.5f;

	[SerializeField]
	private float m_initialDelay = 1;

	[SerializeField]
	private float m_finalDelay = 4;

	[SerializeField]
	private Sprite[] m_speakerSpritesBg;

	[SerializeField]
	private Sprite[] m_speakerSpritesFg;

	[SerializeField]
	[TextArea(3,10)]
	private String m_initialText;

	[SerializeField]
	private Speaker m_initialSpeaker = Speaker.player;

	private String m_text;

	private AudioSource m_audioSource;

	private int m_currentPosition;

	private float m_characterDelayLeft;

	private bool m_skipBlocked = true;


	private void Start() {
		m_audioSource = GetComponent<AudioSource>();

		if (m_initialText != null && m_initialText.Length>0) {
			StartDialog (m_initialText, m_initialSpeaker);
		}
	}

	private void Update() {
		if (Input.anyKey) {
			if (!m_skipBlocked) {
				Skip ();
			}
			m_skipBlocked = true;
		} else {
			m_skipBlocked = false;
		}

		if (m_text.Length > m_currentPosition) {
			m_characterDelayLeft -= Time.unscaledDeltaTime;

			if (m_characterDelayLeft <= 0f) {
				m_textBox.text += m_text [m_currentPosition];
				m_currentPosition++;
				m_characterDelayLeft = 1f / m_charactersPerSecond;

				if (m_text.Length > m_currentPosition) {
					if(m_text [m_currentPosition]=='\n') {
						m_characterDelayLeft += m_newlineDelay;

					} else if (Char.IsWhiteSpace (m_text [m_currentPosition])) {
						m_characterDelayLeft += m_spaceDelay;

					} else if (m_audioSource != null && m_characterSound != null) {
						m_audioSource.PlayOneShot (m_characterSound);
					}

				} else { // reached end of string
					m_characterDelayLeft = m_finalDelay;
				}
			}

		} else if(m_characterDelayLeft>0f) {
			m_characterDelayLeft -= Time.unscaledDeltaTime;
			if (m_characterDelayLeft <= 0f) {
				OnDialogDone ();
			}
		}
	}

	public void StartDialog(String msg, Speaker speaker) {
		foreach (var go in m_hideOnDialog) {
			go.SetActive (false);
		}
		foreach (var go in m_showOnDialog) {
			go.SetActive (true);
		}

		m_text = msg;
		m_textBox.text = "";
		m_characterDelayLeft = m_initialDelay;
		m_currentPosition = 0;
		m_skipBlocked = true;

		SetSprite (speaker, m_speakerImageBg, m_speakerSpritesBg);
		SetSprite (speaker, m_speakerImageFg, m_speakerSpritesFg);
	}

	private void SetSprite(Speaker speaker, Image image, Sprite[] sprites) {
		if (image == null) {
			return;
		}

		if (sprites.Length > (int)speaker) {
			image.sprite = sprites [(int)speaker];
			image.enabled = image.sprite!=null;
		} else {
			image.enabled = false;
		}
	}

	public void Skip() {
		if (m_text.Length > m_currentPosition) {
			m_textBox.text = m_text;
			m_currentPosition = m_text.Length;
			m_characterDelayLeft = m_finalDelay;

		} else {
			OnDialogDone ();
		}
	}

	private void OnDialogDone() {
		foreach (var go in m_showOnDialog) {
			go.SetActive (false);
		}
		foreach (var go in m_hideOnDialog) {
			go.SetActive (true);
		}
	}

}
