using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour {

    [SerializeField]
    private float m_transitionTime = 1.0f;

    [SerializeField]
    private string m_nextScene;

    public static event Action OnLevelComplete;

    public void CompleteLevel() {
        //Time.timeScale = 0;
        if (OnLevelComplete != null)
            OnLevelComplete();

        var fadeEffectSystem = FindObjectOfType<FadeEffectSystem>();
        if (fadeEffectSystem) {
            fadeEffectSystem.FadeOut(m_transitionTime);
        }

        StunAllGuards();
        StartCoroutine(loadNextLevel());
    }

    private IEnumerator loadNextLevel() {
        yield return new WaitForSecondsRealtime(m_transitionTime);

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(m_nextScene);
    }

    private void StunAllGuards() {
        var guards = FindObjectsOfType<EnemyInteractable>();
        foreach (var guard in guards) {
            guard.GetComponent<NavMeshAgent>().enabled = false;
            guard.GetComponent<StatePatternEnemy>().StopMovement();
            guard.GetComponent<StatePatternEnemy>().enabled = false;
            guard.StopStunRoutine();
            guard.ShowStunIndicator();
        }
    }
}
