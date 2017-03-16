using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalInteraction : Interactable
{
    private CamouflageController _camouflageController;

    private CamouflageController GetCamouflageController()
    {
        if (_camouflageController != null) return _camouflageController;

        // try to get instance
        _camouflageController = PlayerActor.Instance.gameObject.GetComponent<CamouflageController>();

        if (_camouflageController == null)
        {
            Debug.LogError("PedestalInteraction: PlayerActor misses component CamouflageController.");
            this.interactionActive = false;
            return null;
        }
        
        return _camouflageController;
    }

    public override void Interact(Interactor interactor)
    {
        if (GetCamouflageController() == null) return;

        bool camouflageModeEntered = GetCamouflageController().TryEnterCamouflageMode();
        if (camouflageModeEntered)
        {
            this.interactionActive = false;
        }

    }
}
