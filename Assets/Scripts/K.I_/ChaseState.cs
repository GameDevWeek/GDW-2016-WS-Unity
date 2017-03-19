using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();

        if (!enemy.enabled) {
            return;
        }

        Chase();
    }

    public void OnTriggerEnter(Collider other)  //TODO: Wenn spieler in Reichweite ist GameOver
    {

    }

    public void ToPatrolState()
    {
        Debug.Log("Kann nicht vom Chasen zum Patrolen wechseln");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
        enemy.navMeshAgent.speed = enemy.standartSpeed;
        enemy.camouflageNotInRange();
    }

    public void ToChaseState()
    {
        Debug.Log("Kann nicht zum selben State wechseln (D'Uh...)");
    }

    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = enemy.chaseTarget.position - enemy.eyes.transform.position;
        if (enemy.canSeePlayer(out hit))
        {
            enemy.chaseTarget = hit.transform;
            enemy.wantedLevel.RaiseWantedLevel();

            Vector3 enemyToPlayer = enemy.playerActor.transform.position - enemy.transform.position;
            float playerDistance = enemyToPlayer.magnitude;
            //if (playerDistance <= enemy.playerIsCaughtDistance)
            //    enemy.caughtPlayer();


        }
        else
        {
            ToAlertState();
        }
    }

    private void Chase()
    {
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();

    }
}
