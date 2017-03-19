using System;
using UnityEngine;
using System.Collections;

public class LookAtInteractable : Interactable {

	[SerializeField]
	private String m_message;

	private DialogSystem m_dialogSystem;

	private CameraController m_cameraController;


	private void Start() {
		m_cameraController = GameObject.FindObjectOfType<CameraController> ();
	}

	public override void Interact(Interactor interactor) {
		if (m_dialogSystem == null) {
			m_dialogSystem = GameObject.FindObjectOfType<DialogSystem> ();
		}

		StartCoroutine (InteractRoutine());
	}

	IEnumerator InteractRoutine() {
		m_cameraController.EnableSpectatorCam (transform, false);

		yield return new WaitForSecondsRealtime(1);

		m_dialogSystem.StartDialog (m_message, Speaker.player);

		while(m_dialogSystem.IsDialogInProgress()) {
			yield return new WaitForSecondsRealtime(0.5f);
		}

		m_cameraController.EnableNormalCam ();
	}

}
