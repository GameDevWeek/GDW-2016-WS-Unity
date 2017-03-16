using System;
using Assets.Scripts.K.I_;
using UnityEngine;
using UnityEngine.AI;

public class StatePatternEnemy : MonoBehaviour, INoiseListener {

    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 10f;
    public float toleratedSightrange = 5f;      //Muss kleiner sein als sightRange!!!
    public float stoppingTime = 2f;

    public Waypoints wayPoints;
    public int currentWaypoint;

    public Transform eyes;
    public Vector3 offset = new Vector3(0, .5f, 0);  //Damit man nicht auf die Schuhe des Spielers schaut
    public MeshRenderer meshRendererFlag;

    private int highestPriority = 0;

    [HideInInspector] public Vector3 targetPos; 
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

    public void WantedLvlUp()
    {
        searchingTurnSpeed += 20f;
        sightRange += 5f;

    }

    public void WantedLvlDown()
    {
        searchingTurnSpeed -= 20f;
        sightRange -= 5f;
    }

    public void Inform(NoiseSourceData data)    //Wenn ich im PatrolState etwas höre laufe ich auf
    {
        if (currentState == patrolState)
        {
            highestPriority = 0;        //Wenn ich irgendwann mal wieder in den patrolState komme ist jede Noise highestPrio
            navMeshAgent.SetDestination(data.initialPosition);
            currentState = alertState;
        }
        else if (currentState == alertState && data.priority>=highestPriority)
        {
            highestPriority = data.priority;
            navMeshAgent.SetDestination(data.initialPosition);
            navMeshAgent.Resume();
        }
        
    }
}
