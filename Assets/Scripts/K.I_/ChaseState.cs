using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    //private Vector3 m_chasePos;
    // public Vector3 chasePos { get { return m_chasePos; } set { m_chasePos = value; } }

    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
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
    }

    public void ToChaseState()
    {
        Debug.Log("Kann nicht zum selben State wechseln (D'Uh...)");
    }

    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            WantedLevel.Instance.RaiseWantedLevel();
            enemy.camouflageInRange(hit);
        }
        else
        {
            enemy.camouflageNotInRange();
            ToAlertState();
        }
    }

    private void Chase()
    {
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();

    }
}
