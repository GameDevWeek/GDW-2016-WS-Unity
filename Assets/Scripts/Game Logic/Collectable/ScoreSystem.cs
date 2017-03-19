using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreSystem: Singleton<ScoreSystem>
{

    private static Dictionary<string, int> m_collectableScores = new Dictionary<string, int>();
    private static Dictionary<string, int> m_destroyableScores = new Dictionary<string, int>();

    private float m_levelTime = 0;
    private bool  m_countTime = true;

    private void Update()
    {
        if(m_countTime)
        m_levelTime += Time.deltaTime;
    }

    public static int GetCollectableScore(string scoreName)
    {
        if (m_collectableScores.ContainsKey(scoreName))
        {
            return m_collectableScores[scoreName];
        }
        return -1;
    }

    public static int GetDestructibleScore(string scoreName)
    {
        if (m_destroyableScores.ContainsKey(scoreName))
        {
            return m_destroyableScores[scoreName];
        }
        return -1;
    }

    public static int GetScore(string scoreName)
    {
        int result = -1;

        result = GetCollectableScore(scoreName);

        if (result == -1)
        {
            result = GetDestructibleScore(scoreName);
        } else
        {
            return result;
        }

        if (result == -1)
        {
            
            Debug.LogError("[ScoreSystem]: " + scoreName + " not available or wrong typed!");
            Debug.LogError("[ScoreSystem]: available Keys" + m_collectableScores.Keys.ToString() + "\n" + m_destroyableScores.Keys.ToString());
            return 0;
        }

        return result;
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

