using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Collectable : Interactable {
    [SerializeField, Tooltip("Collected for the first time.")]
    private bool m_firstEncounter = false;

    [SerializeField]
    public int ScoreValue;

    public AudioClip collectionSound;

    [SerializeField]
    private string m_collectableName;

    public struct CollectableEventData {
        public GameObject collected;
        public bool winItem;
        public bool firstTime;

        public CollectableEventData(GameObject collected, bool winItem, bool firstTime) {
            this.collected = collected;
            this.winItem = winItem;
            this.firstTime = firstTime;
        }
    }

    public delegate void CollectableEvent(CollectableEventData data);
    public static event CollectableEvent OnCollect;
    [SerializeField]
    public bool isWinItem = false;

    private void OnValidate() {
        if (string.IsNullOrEmpty(m_collectableName)) {
            Debug.LogError("[" + name + "] Collectable Name is Empty!", this.gameObject);
        }
    }

    public String getCollectableName() {
        return m_collectableName;
    }


    public override void Interact(Interactor interactor) {
        if (OnCollect != null)
            OnCollect.Invoke(new CollectableEventData(gameObject, isWinItem, m_firstEncounter));
        if (collectionSound != null)
            interactor.GetComponent<AudioSource>().PlayOneShot(collectionSound);

        if (m_firstEncounter) {
            OnFirstEncounter();

        }

        Destroy(gameObject);
    }

    void OnFirstEncounter() {
        // Set all first encounter flags of that type to false
        m_firstEncounter = false;
        var type = GetType();
        foreach (var c in FindObjectsOfType<Collectable>()) {
            if (c.GetType().Equals(type)) {
                c.m_firstEncounter = false;
            }
        }

        // Show speech bubble if available
        GetComponent<SpeechBubbles>().activateDialog();
    }
}
