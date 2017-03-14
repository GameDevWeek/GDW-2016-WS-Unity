﻿using Assets.Scripts.K.I_;
using UnityEngine;
using UnityEngine.AI;

public class StatePatternEnemy : MonoBehaviour {

    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 20f;

    public Waypoints wayPoints;
    public int currentWaypoint;

    public Transform eyes;
    public Vector3 offset = new Vector3(0, .5f, 0);  //Damit man nicht auf die Schuhe des Spielers schaut
    public MeshRenderer meshRendererFlag;

    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public IEnemyState currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public AlertState alertState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentState = patrolState;
    }

    void Update()
    {
        currentState.UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }
}
