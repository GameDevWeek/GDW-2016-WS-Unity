using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Interactable
{
    [SerializeField]
    public int ScoreValue;

    public AudioClip collectionSound;

    public struct CollectableEventData
    {
        public GameObject collected;

        public CollectableEventData(GameObject collected)
        {
            this.collected = collected;
        }
    }

    public delegate void CollectableEvent(CollectableEventData data);
    public static event CollectableEvent OnCollect;


    public override void Interact(Interactor interactor)
    {
        if(OnCollect != null)
        OnCollect.Invoke(new CollectableEventData(gameObject));
        if(collectionSound != null)
            interactor.GetComponent<AudioSource>().PlayOneShot(collectionSound);
        Destroy(gameObject);
    }

   
}
