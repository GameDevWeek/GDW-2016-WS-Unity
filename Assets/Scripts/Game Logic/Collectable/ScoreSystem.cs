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
    private Dictionary<string, int> m_destroyableScores = new Dictionary<string, int>();

    private float m_levelTime = 0;
    private bool  m_countTime = true;

    private void Update()
    {
        if(m_countTime)
        m_levelTime += Time.deltaTime;
    }

    // Use this for initialization
    void Start()
    {
        m_levelTime = 0;
        m_countTime = true;

        Collectable.OnCollect += OnCollect;
        LevelCompleteZone.OnLevelComplete += OnLevelComplete;
        DestructibleObject.OnDestruction += OnDestruction;

        Collectable[] collectables = FindObjectsOfType<Collectable>();
        DestructibleObject[] destructibles = FindObjectsOfType<DestructibleObject>();

        foreach (Collectable c in collectables)
        {
            if(!m_collectableScores.ContainsKey(c.getCollectableName()))
            m_collectableScores.Add(c.getCollectableName(), 0);
        }

        foreach (DestructibleObject d in destructibles)
        {
            if (!m_destroyableScores.ContainsKey(d.GetDestructableName()))
                m_destroyableScores.Add(d.GetDestructableName(), 0);
        }
    }

    private void OnCollect(Collectable.CollectableEventData data)
    {
        String collectableName = data.collected.GetComponent<Collectable>().getCollectableName();
        int oldValue = m_collectableScores[collectableName];
        m_collectableScores[collectableName] += data.collected.GetComponent<Collectable>().ScoreValue;

        Debug.Log("[ScoreSystem] update Score for "+ collectableName + " from " + oldValue + " to " + m_collectableScores[collectableName]);
    }

    private void OnDestruction(DestructibleObject.DestructionEventData data)
    {
        String destructibleName = data.destroyed.GetComponent<DestructibleObject>().GetDestructableName();
        int oldValue = m_destroyableScores[destructibleName];
        m_destroyableScores[destructibleName] += 1;

        Debug.Log("[ScoreSystem] update Score for " + destructibleName + " from " + oldValue + " to " + m_destroyableScores[destructibleName]);
    }

    private void OnLevelComplete(LevelCompleteZone.LevelCompleteZoneEventData data)
    {
        m_countTime = false;
    }
}

