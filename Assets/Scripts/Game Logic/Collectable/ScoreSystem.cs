using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreSystem: Singleton<ScoreSystem>
{

    private Dictionary<string, int> m_collectableScores = new Dictionary<string, int>();

    private void Update()
    {
    }

    // Use this for initialization
    void Start()
    {
        Collectable.OnCollect += OnCollect;

        Collectable[] collectables = FindObjectsOfType<Collectable>();

        foreach (Collectable c in collectables)
        {
            if(!m_collectableScores.ContainsKey(c.getCollectableName()))
            m_collectableScores.Add(c.getCollectableName(), 0);
        }
    }

    private void OnCollect(Collectable.CollectableEventData data)
    {
        String collectableName = data.collected.GetComponent<Collectable>().getCollectableName();
        int oldValue = m_collectableScores[collectableName];
        m_collectableScores[collectableName] += data.collected.GetComponent<Collectable>().ScoreValue;

        Debug.Log("[ScoreSystem] update Score for "+ collectableName + " from " + oldValue + " to " + m_collectableScores[collectableName]);
    }
}

