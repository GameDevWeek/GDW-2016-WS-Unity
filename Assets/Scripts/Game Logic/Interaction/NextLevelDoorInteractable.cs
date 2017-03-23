using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelCompletion))]
public class NextLevelDoorInteractable : Interactable {
    public Transform cameraFocusTarget;

    public override void Interact(Interactor interactor) {
        GetComponent<LevelCompletion>().CompleteLevel();

        var camControl = FindObjectOfType<CameraController>();
        if (camControl && cameraFocusTarget) {
            camControl.EnableSpectatorCam(cameraFocusTarget, false);
        }
    }
}
