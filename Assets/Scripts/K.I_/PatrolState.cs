using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
	
    public void UpdateState()
    {
        Look();
        Patrol();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ToAlertState();
    }

    public void ToPatrolState()
    {
        Debug.Log("Kann nicht zum selben State wechseln (D'Uh...)");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }

    private void Look()
    {
        //Bounds test -------------------------------------
        Vector3[] boundPoints = new Vector3[3];
        RaycastHit boundsHit, boundsHit1, boundsHit2;

        var targetPoint = PlayerActor.Instance.collider.bounds.ClosestPoint(enemy.eyes.transform.position);
        var minPoint = PlayerActor.Instance.collider.bounds.min;
        var maxPoint = PlayerActor.Instance.collider.bounds.max;

        boundPoints[0] = targetPoint;
        boundPoints[1] = minPoint;
        boundPoints[2] = maxPoint;

        int oldlayer = enemy.gameObject.layer;
        enemy.gameObject.layer = GameLayer.IgnoreRaycast;
        if (Physics.Raycast(enemy.eyes.transform.position, boundPoints[0] - enemy.eyes.transform.position, out boundsHit, enemy.sightRange) &&
            Physics.Raycast(enemy.eyes.transform.position, boundPoints[1] - enemy.eyes.transform.position, out boundsHit1, enemy.sightRange) &&
            Physics.Raycast(enemy.eyes.transform.position, boundPoints[2] - enemy.eyes.transform.position, out boundsHit2, enemy.sightRange))
            Debug.Log(boundsHit.transform.gameObject);
            if (boundsHit.collider.CompareTag("Player") == PlayerActor.Instance.transform)
            {
                enemy.chaseTarget = boundsHit.transform;
                ToChaseState();
            }
        enemy.gameObject.layer = oldlayer;
    
        //-----------------------------------------------------
        //RaycastHit hit;

        //if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) &&
        //    (hit.collider.CompareTag("Player"))) //Eventuell CompareTag entfernen falls Layer angepasst werden
        //{
        //    enemy.chaseTarget = hit.transform;
        //    ToChaseState();
        //}
    }


    //TODO Abfahrfolge der Wegpunkte zufällig machen
    void Patrol()
    {
        enemy.meshRendererFlag.material.color = Color.green;  //Debugging tool
        enemy.navMeshAgent.destination = enemy.wayPoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending) //Durch jeden Waypoint laufen und nach dem letzten wieder zum ersten
        {
            nextWayPoint = (nextWayPoint + 1) % enemy.wayPoints.Length;
        }
    }
}
