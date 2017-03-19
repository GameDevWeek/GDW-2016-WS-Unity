using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

    private ElephantMovement m_elephantMovement;
    public string gameOverScene = "GameOverScreen";
    private SceneLoaderScript m_sceneLoader;
    private Coroutine m_giveUpRoutine;
    [SerializeField]
    private float m_sceneChangeDelay = 5.0f;
    private WantedLevel m_wantedLevel;

    // Use this for initialization
    void Start() {
        m_elephantMovement = GameObject.FindObjectOfType<ElephantMovement>();
        m_sceneLoader = GameObject.FindObjectOfType<SceneLoaderScript>();
        m_wantedLevel = GameObject.FindObjectOfType<WantedLevel>();
        StatePatternEnemy.OnCaught += OnCaught;
    }

    void OnDestroy() {
        StatePatternEnemy.OnCaught -= OnCaught;
    }

    private void OnCaught(StatePatternEnemy.CaughtEventData data) {
        EndGame();
    }

    IEnumerator GiveUpRoutine() {
        m_elephantMovement.GiveUp();
        Camera.main.GetComponent<TopDownCamera>().enabled = false;
        Camera.main.GetComponent<DeathCamera>().enabled = true;

        yield return new WaitForSecondsRealtime(m_sceneChangeDelay);

        m_sceneLoader.LoadSceneSingle(gameOverScene);
    }

    public void EndGame() {
        if (m_giveUpRoutine != null) {
            return;
        }

        m_giveUpRoutine = StartCoroutine(GiveUpRoutine());
    }

    // Update is called once per frame
    void Update() {
        if (m_wantedLevel.currentWantedStage == 2) {
            EndGame();
        }
    }
}
