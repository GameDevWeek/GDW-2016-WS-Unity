using System;
using Assets.Scripts.K.I_;
using UnityEngine;
using UnityEngine.AI;

public class StatePatternEnemy : MonoBehaviour, INoiseListener {

    public float standartSpeed = 2;
    public float chaseSpeed = 3;
    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 10f;
    public float toleratedSightrange = 5f;      //Muss kleiner sein als sightRange!!!
    public float fieldOfView = 90;
    public float stoppingTime = 2f;

    public Waypoints wayPoints;
    [HideInInspector] public int currentWaypoint;

    public Transform eyes;
    public ViewCone viewCone;
    public LayerMask viewBlockingLayers;

    private int highestPriority = 0;
    private CamouflageController camouflage;

    [HideInInspector] public Vector3 targetPos; 
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public IEnemyState currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public AlertState alertState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private Animator enemyAnimator;
    

    private void Awake()
    {
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = standartSpeed;

        enemyAnimator = GetComponent<Animator>();
    }

    void OnValidate() {
        currentState = patrolState;
    }

    void Start()
    {
        currentState = patrolState;

        viewCone.MainViewRadius = toleratedSightrange;
        viewCone.FullViewRadius = sightRange;
    }

    void Update() {
        currentState.UpdateState();
        if (currentState != alertState)
        {
            enemyAnimator.SetFloat("BlendSpeed", (float) (navMeshAgent.velocity.magnitude/chaseSpeed));
        }


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

    private void OnDrawGizmos() {
        if(currentState == this.alertState)
            Gizmos.color = Color.yellow;
        else if(currentState == this.chaseState)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position + Vector3.up, Vector3.one * 0.25f);
    }

    public void Inform(NoiseSourceData data)    //Wenn ich im PatrolState etwas höre laufe ich auf
    {
        if (currentState == patrolState)
        {
            highestPriority = 0;        //Wenn ich irgendwann mal wieder in den patrolState komme ist jede Noise highestPrio
            navMeshAgent.SetDestination(data.initialPosition);
            viewCone.setAlarmed(true, 1f);
            currentState = alertState;
        }
        else if (currentState == alertState && data.priority>=highestPriority)
        {
            highestPriority = data.priority;
            navMeshAgent.SetDestination(data.initialPosition);
            navMeshAgent.Resume();
        }
        
    }


    public void camouflageInRange(RaycastHit hit)
    {
        camouflage = PlayerActor.Instance.GetComponent<CamouflageController>();
        if (camouflage != null)
        {
            camouflage.EnemyInRange(this.gameObject);
            Debug.Log("Can't camouflage!");
        }
    }

    public void camouflageNotInRange()
    {
        if (camouflage != null)
        {
            camouflage.EnemyOutOfRange(this.gameObject);
            Debug.Log("Can camouflage now!");
        }
    }

    public bool canSeePlayer(out RaycastHit hit)
    {
        Vector3 enemyToPlayer = PlayerActor.Instance.transform.position - transform.position;
        float playerDistance = enemyToPlayer.magnitude;
        if (playerDistance <= sightRange)
        {
            float angleDistance = Vector3.Angle(enemyToPlayer, transform.forward);
            if (angleDistance < fieldOfView * 0.5f)
            {
                float playerWidth = ((CapsuleCollider)PlayerActor.Instance.collider).radius;

                Vector3 enemyToPlayerOrtho = new Vector3(enemyToPlayer.z, enemyToPlayer.y, -enemyToPlayer.x).normalized;
                Vector3 playerPosLeft = PlayerActor.Instance.transform.position - enemyToPlayerOrtho * 0.5f * playerWidth;
                Vector3 playerPosRight = PlayerActor.Instance.transform.position + enemyToPlayerOrtho * 0.5f * playerWidth;

                Debug.DrawLine(transform.position, playerPosRight, Color.green);
                Debug.DrawLine(transform.position, playerPosLeft, Color.green);
                Debug.DrawLine(transform.position, PlayerActor.Instance.transform.position, Color.green);

                if ((Physics.Raycast(eyes.transform.position, enemyToPlayer.normalized, out hit, sightRange, viewBlockingLayers) &&
                    (hit.collider.CompareTag("Player"))))
                    return true;
                if (Physics.Raycast(eyes.transform.position, (playerPosLeft - transform.position).normalized, out hit, sightRange, viewBlockingLayers) &&
                     (hit.collider.CompareTag("Player")))
                    return true;
                if (Physics.Raycast(eyes.transform.position, (playerPosRight - transform.position).normalized, out hit, sightRange, viewBlockingLayers) &&
                    (hit.collider.CompareTag("Player")))
                    return true;


            }

        }
        hit = new RaycastHit();
        return false;
    }
}
