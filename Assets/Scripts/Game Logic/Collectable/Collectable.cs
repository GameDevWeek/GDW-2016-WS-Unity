using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Interactable
{
    [SerializeField]
    public int ScoreValue;

    [SerializeField]
    private string m_collectableName;

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

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_collectableName))
        {
            Debug.LogError("Collectable Name is Empty!");
        }
    }

    public String getCollectableName()
    {
        return m_collectableName;
    }


    public override void Interact(Interactor interactor)
    {
        if(OnCollect != null)
        OnCollect.Invoke(new CollectableEventData(gameObject));
        Destroy(gameObject);
    }

   
}
