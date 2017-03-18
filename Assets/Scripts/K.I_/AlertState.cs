using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlertState : IEnemyState {

    private readonly StatePatternEnemy enemy;
    private float searchTimer;
    private WantedLevel m_wantedLevel;

    void OnStart() {
        m_wantedLevel = GameObject.FindObjectOfType<WantedLevel>();
    }
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Search();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToAlertState()
    {
        Debug.Log("Kann nicht zum selben State wechseln(D'uh..).");
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        searchTimer = 0f;
        enemy.navMeshAgent.speed = enemy.standartSpeed;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
        enemy.navMeshAgent.speed = enemy.chaseSpeed;
        enemy.camouflageInRange();

        if(enemy.enterChase.Any())
            enemy.audioSource.PlayOneShot(Util.RandomElement(enemy.enterChase));
    }



    private void Look()
    {
        RaycastHit hit;
        if (enemy.canSeePlayer(out hit))
        {
            enemy.chaseTarget = hit.transform;

            enemy.navMeshAgent.SetDestination(enemy.targetPos);
            m_wantedLevel.RaiseWantedLevel();
            ToChaseState();
        }
    }

    private void Search()
    {
        if(enemy.navMeshAgent.remainingDistance < 1f) {
            enemy.navMeshAgent.Stop();
            if(enemy.GetComponent<Animator>()!=null)
            enemy.GetComponent<Animator>().SetFloat("BlendSpeed", -1);

            enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
            searchTimer += Time.deltaTime;

            if (searchTimer >= enemy.searchingDuration)
            {
                enemy.viewCone.setAlarmed(false, 0);
                ToPatrolState();
            }
        }

        
    }
}
