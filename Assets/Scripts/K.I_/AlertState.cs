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
        //Vector3[] boundPoints = new Vector3[3];
        //RaycastHit boundsHit, boundsHit1, boundsHit2;

        //var targetPoint = PlayerActor.Instance.collider.bounds.ClosestPoint(enemy.eyes.transform.position);
        //var minPoint = PlayerActor.Instance.collider.bounds.min;
        //var maxPoint = PlayerActor.Instance.collider.bounds.max;

        //boundPoints[0] = targetPoint;
        //boundPoints[1] = minPoint;
        //boundPoints[2] = maxPoint;

        //if (Physics.Raycast(enemy.eyes.transform.position, boundPoints[0] - enemy.eyes.transform.position, out boundsHit, enemy.sightRange) &&
        //    Physics.Raycast(enemy.eyes.transform.position, boundPoints[1] - enemy.eyes.transform.position, out boundsHit1, enemy.sightRange) &&
        //    Physics.Raycast(enemy.eyes.transform.position, boundPoints[2] - enemy.eyes.transform.position, out boundsHit2, enemy.sightRange))
        //    if (boundsHit.collider.CompareTag("Player"))
        //    {
        //        enemy.chaseTarget = boundsHit.transform;
        //        ToChaseState();
        //    }




        RaycastHit hit;
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) &&
            hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            //enemy.targetPos = hit.transform.position;

            enemy.navMeshAgent.SetDestination(enemy.targetPos);
            ToChaseState();
        }
    }

    private void Search()
    {
        enemy.meshRendererFlag.material.color = Color.yellow;  //Debugging tool

        if(enemy.navMeshAgent.remainingDistance < 1f) {
            enemy.navMeshAgent.Stop();
            enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
            searchTimer += Time.deltaTime;

            if (searchTimer >= enemy.searchingDuration)
            {
                ToPatrolState();
            }
        }

        
    }
}
