using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteZone : MonoBehaviour
{

    [SerializeField]
    private float m_time = 1.0F;

    [SerializeField]
    private string m_scene;

    [SerializeField]
    private bool m_iterateSceneID;

    /// <summary>
    /// If m_iterateSceneID is true, m_scene will ignored and scene with the next build Id will be loaded.
    /// </summary>

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public struct LevelCompleteZoneEventData
    {
        public GameObject gameObject;

        public LevelCompleteZoneEventData(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }

    public delegate void LevelCompleteZoneEvent(LevelCompleteZoneEventData data);
    public static event LevelCompleteZoneEvent OnLevelComplete;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            Invoke("loadNextLevel", m_time);
            if (OnLevelComplete != null)
                OnLevelComplete.Invoke(new LevelCompleteZoneEventData(other.gameObject));

        }
    }

    private void loadNextLevel()
    {
        //todo: save data
        if (m_iterateSceneID)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(m_scene);
        }


    }

}
