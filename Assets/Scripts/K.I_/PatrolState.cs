using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;
    private float searchTimer;
    private bool isLooking;
    private bool firstChase = false;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
	
    public void UpdateState()
    {
        Look();
        if(enemy.currentState == this)  //War irgendwie nötig für den cone...
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
        searchTimer = 0f;

        if(enemy.enterChase.Any())
            enemy.audioSource.PlayOneShot(Util.RandomElement(enemy.enterChase));
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
        enemy.navMeshAgent.speed = enemy.chaseSpeed;
        enemy.camouflageInRange();
        
        if (!firstChase)
        {
            WantedLevel.Instance.GuardIsNowVigilant();
            firstChase = true;
        }
    }

    private void Look()
    {
        RaycastHit hit;
        if (enemy.canSeePlayer(out hit))
        {
            if (hit.distance >= enemy.toleratedSightrange)     //Wenn Spieler im Toleranzbereich erstmal stoppen und schauen
            {
                StopAndLook(hit);
            }
            else
            {
                enemy.chaseTarget = hit.transform;          //Wenn Spieler unterm Toleranzbereich ist direkt chasen
                searchTimer = 0f;
                isLooking = false;
                WantedLevel.Instance.RaiseWantedLevel();
                enemy.viewCone.setAlarmed(true, 1f);
                ToChaseState();
            }
        }
        else if (searchTimer >= enemy.stoppingTime && searchTimer != 0)
        {
            isLooking = false;
        }
        else if (searchTimer < enemy.stoppingTime && searchTimer > 0)
        {
            searchTimer += Time.deltaTime;
        }
    }
        
    //TODO Abfahrfolge der Wegpunkte zufällig machen
    void Patrol()
    {
        if (!isLooking) {
            searchTimer = 0f;
            enemy.viewCone.setAlarmed(false, 0f);
            enemy.navMeshAgent.destination = enemy.wayPoints.points[enemy.currentWaypoint];
            enemy.navMeshAgent.Resume();

            if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending) //Durch jeden Waypoint laufen und nach dem letzten wieder zum ersten
            {
                int nxt;
                enemy.wayPoints.GetNextPoint(enemy.currentWaypoint, out nxt);
                enemy.currentWaypoint = nxt;
            }
        }
    }

    void StopAndLook(RaycastHit hit)    
    {
        enemy.navMeshAgent.Stop();
        isLooking = true;
        searchTimer += Time.deltaTime;
        enemy.viewCone.setAlarmed(true, Mathf.Clamp(searchTimer / enemy.stoppingTime, 0f, 1f));
        if (searchTimer >= enemy.stoppingTime)
        {
            enemy.chaseTarget = hit.transform;
            searchTimer = 0f;
            isLooking = false;
            ToChaseState();
        }
    }
}
