using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.K.I_;
using UnityEngine;
using UnityEngine.AI;

public class FollowWaypoints : MonoBehaviour {

    public Waypoints wayPoints;
    private int currentWaypoint;
    private NavMeshAgent navMeshAgent;

    // Use this for initialization
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        navMeshAgent.destination = wayPoints.points[currentWaypoint];

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending) //Durch jeden Waypoint laufen und nach dem letzten wieder zum ersten
        {
            int nxt;
            wayPoints.GetNextPoint(currentWaypoint, out nxt);
            currentWaypoint = nxt;
        }
    }
}
