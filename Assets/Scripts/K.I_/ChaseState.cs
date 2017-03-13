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
        Chase();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToPatrolState()
    {

    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {

    }

    private void Look()
    {
        Vector3[] boundPoints = new Vector3[3];
        RaycastHit boundsHit, boundsHit1, boundsHit2;

        var targetPoint = PlayerActor.Instance.collider.bounds.ClosestPoint(enemy.eyes.transform.position);
        var minPoint = PlayerActor.Instance.collider.bounds.min;
        var maxPoint = PlayerActor.Instance.collider.bounds.max;

        boundPoints[0] = targetPoint;
        boundPoints[1] = minPoint;
        boundPoints[2] = maxPoint;

        if (Physics.Raycast(enemy.eyes.transform.position, boundPoints[0] - enemy.eyes.transform.position, out boundsHit, enemy.sightRange) &&
            Physics.Raycast(enemy.eyes.transform.position, boundPoints[1] - enemy.eyes.transform.position, out boundsHit1, enemy.sightRange) &&
            Physics.Raycast(enemy.eyes.transform.position, boundPoints[2] - enemy.eyes.transform.position, out boundsHit2, enemy.sightRange))
            if (boundsHit.collider.CompareTag("Player"))
            {
                enemy.chaseTarget = boundsHit.transform;
            }
        else
            {
                ToAlertState();
            }

        //RaycastHit hit;
        //Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        //if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        //{
        //    enemy.chaseTarget = hit.transform;
        //}
        //else
        //{
        //    ToAlertState();
        //}
    }

    private void Chase()
    {
        enemy.meshRendererFlag.material.color = Color.red;  //Debugging tool
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();
    }
}
