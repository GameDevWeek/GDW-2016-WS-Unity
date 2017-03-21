using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbleSystem : MonoBehaviour {
    private DialogSystem m_dialogSystem;
    private ElephantMovement m_elephantMovement;

	void Start () {
        m_dialogSystem = FindObjectOfType<DialogSystem>();
        m_elephantMovement = FindObjectOfType<ElephantMovement>();
    }

    private void SetPlayMode() {
        Camera.main.GetComponent<CameraController>().EnableNormalCam();
        m_elephantMovement.allowToMove = true;
    }

    public void StartSpeech() {
        StartCoroutine(SpeechRoutine());
    }

    IEnumerator SpeechRoutine() {
        while (m_dialogSystem.IsDialogInProgress()) {
            yield return null;
        }

        SetPlayMode();
    }
}
