using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbles : MonoBehaviour {
    [SerializeField]
    private GameObject m_mainCamera;
    [SerializeField]
    private GameObject m_dialogTarget;
    [SerializeField]
    private GameObject m_player;
    [SerializeField]
    [TextArea(3, 10)]
    private string m_initalTextGerman;
    [SerializeField]
    [TextArea(3, 10)]
    private string m_initalTextEnglish;
    [SerializeField]
    private bool english_German;
    [SerializeField]
    private Speaker m_speaker;
    [SerializeField]
    private bool m_rotateAroundDialogTarget = true;

    public bool invokeOnTriggerEnter = true;
    private ElephantMovement elephantMovement;
    private CameraController cameraController;
    public bool focusOnTarget = false;
    private SpeechBubbleSystem m_speechBubbleSystem;

    public GameObject mainCamera {
        get {
            return m_mainCamera;
        }
    }

    public GameObject dialogTarget {
        get {
            return m_dialogTarget;
        }
    }

    public GameObject player {
        get {
            return m_player;
        }
    }

    void Start() {
        m_speechBubbleSystem = FindObjectOfType<SpeechBubbleSystem>();

        if (!m_player) {
            var pa = FindObjectOfType<PlayerActor>();
            if (pa) {
                m_player = pa.gameObject;
            }
        }

        if (!m_dialogTarget) {
            m_dialogTarget = m_player;
        }

        if (!m_mainCamera) {
            m_mainCamera = Camera.main.gameObject;
        }

        elephantMovement = m_player.GetComponent<ElephantMovement>();
        cameraController = m_mainCamera.GetComponent<CameraController>();
    }

    private void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "SpeechBubbleGizmo.png", false);
    }

    void OnTriggerEnter(Collider other) {
        if (invokeOnTriggerEnter) {
            if (other.tag == "Player") {
                activateDialog();
                invokeOnTriggerEnter = false;
            }
        }
    }

    public void activateDialog() {

        string initalText;
        if (english_German) {
            initalText = m_initalTextEnglish;
        } else {
            initalText = m_initalTextGerman;
        }

        elephantMovement.allowToMove = false;

        cameraController.EnableSpectatorCam(m_dialogTarget.transform, m_rotateAroundDialogTarget);
        GameObject.FindObjectOfType<DialogSystem>().StartDialog(initalText, m_speaker);
        focusOnTarget = true;
        m_speechBubbleSystem.StartSpeech();
    }
}



