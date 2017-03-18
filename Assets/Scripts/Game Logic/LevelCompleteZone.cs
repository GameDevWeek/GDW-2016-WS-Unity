using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

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


#if UNITY_EDITOR
    [SerializeField] private SceneAsset scene;

    private void OnValidate() {
        m_scene = scene.name;
    }

#endif

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

        if (other.gameObject.CompareTag(GameTag.Player)) {

            Time.timeScale = 0;

            //Invoke("loadNextLevel", m_time);
            StartCoroutine(loadNextLevel());
            if (OnLevelComplete != null)
                OnLevelComplete.Invoke(new LevelCompleteZoneEventData(other.gameObject));
        }
    }

    private IEnumerator loadNextLevel()
    {
        yield return new WaitForSecondsRealtime(m_time);

        Time.timeScale = 1;
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
