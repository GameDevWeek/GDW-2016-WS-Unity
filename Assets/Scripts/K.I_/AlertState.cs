using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : IEnemyState {

    private readonly StatePatternEnemy enemy;
    private float searchTimer;

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
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
    }

    

    private void Look()
    {
        RaycastHit hit;
        if (enemy.canSeePlayer(out hit))
        {
            enemy.chaseTarget = hit.transform;

            enemy.navMeshAgent.SetDestination(enemy.targetPos);
            WantedLevel.Instance.RaiseWantedLevel();
            enemy.camouflageInRange(hit);
            ToChaseState();
        }
    }

    private void Search()
    {
        if(enemy.navMeshAgent.remainingDistance < 1f) {
            enemy.navMeshAgent.Stop();
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
