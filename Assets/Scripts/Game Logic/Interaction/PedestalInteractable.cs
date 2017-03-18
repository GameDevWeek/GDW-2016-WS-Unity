using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalInteractable : Interactable
{
    private CamouflageController _camouflageController;

    private CamouflageController GetCamouflageController()
    {
        if (_camouflageController != null) return _camouflageController;

        // try to get instance
        _camouflageController = PlayerActor.Instance.gameObject.GetComponent<CamouflageController>();

        if (_camouflageController == null)
        {
            Debug.LogError("PedestalInteractable: PlayerActor misses component CamouflageController.");
            this.interactionActive = false;
            return null;
        }

        return _camouflageController;
    }

    public override void Interact(Interactor interactor)
    {
        if (GetCamouflageController() == null) return;

        GetCamouflageController().ToggleCamouflageMode();

    }
}
