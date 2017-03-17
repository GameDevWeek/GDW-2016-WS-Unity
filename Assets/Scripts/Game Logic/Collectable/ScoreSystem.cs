using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScoreSystem: Singleton<ScoreSystem>
{
#if UNITY_EDITOR
    private Dictionary<String, int> m_collectableScores = new Dictionary<String, int>();

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
            if(!m_collectableScores.ContainsKey(PrefabUtility.GetPrefabParent(c.gameObject).name))
            m_collectableScores.Add(PrefabUtility.GetPrefabParent(c.gameObject).name, 0);
        }
    }

    private void OnCollect(Collectable.CollectableEventData data)
    {
        String prefabName = PrefabUtility.GetPrefabParent(data.collected).name;
        int oldValue = m_collectableScores[prefabName];
        m_collectableScores[prefabName] += data.collected.GetComponent<Collectable>().ScoreValue;

        Debug.Log("[ScoreSystem] update Score for "+ PrefabUtility.GetPrefabParent(data.collected).name + " from " + oldValue + " to " + m_collectableScores[prefabName]);
    }
#endif
}

