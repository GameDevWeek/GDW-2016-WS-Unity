using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public EventHandler<OnScoredEventArgs> OnScored;

    private Collectable _collectableInRange;

    public void Grab()
    {
        if (_collectableInRange == null) return;
        
        if (OnScored != null)
        {
            OnScored.Invoke(this, new OnScoredEventArgs(_collectableInRange));
        }

        DestroyImmediate(_collectableInRange.gameObject);
        _collectableInRange = null;
    }

    private void OnTriggerEnter(Collider coll)
    {
        Collectable collectable = coll.gameObject.GetComponent<Collectable>();

        if (collectable != null)
        {
            _collectableInRange = collectable;
        }


    }

    private void OnTriggerExit(Collider coll)
    {
        Collectable collectable = coll.gameObject.GetComponent<Collectable>();

        if (collectable != null && _collectableInRange == coll)
        {
            _collectableInRange = null;
        }


    }

    public class OnScoredEventArgs : EventArgs
    {
        public string CollectedObject { get; private set; }
        public int ScoreValue { get; private set; }

        public OnScoredEventArgs(string name, int scoreValue)
        {
            CollectedObject = name;
            ScoreValue = scoreValue;
        }

        public OnScoredEventArgs(Collectable collectable)
        {
            CollectedObject = collectable.Name;
            ScoreValue = collectable.ScoreValue;
        }
    }
}
