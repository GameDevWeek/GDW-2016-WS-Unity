using System;
using Assets.Scripts.K.I_;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class StatePatternEnemy : MonoBehaviour, INoiseListener {
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject && collision.gameObject.GetComponent<PlayerActor>()) {
            caughtPlayer();
            return;
        }
    }

    public class CaughtPlayerState : IEnemyState {
        private readonly StatePatternEnemy enemy;

        public CaughtPlayerState(StatePatternEnemy statePatternEnemy) {
            enemy = statePatternEnemy;
        }
        public void OnTriggerEnter(Collider other) {
        }

        public void ToAlertState() {
        }

        public void ToChaseState() {
        }

        public void ToPatrolState() {
        }

        public void UpdateState() {
            enemy.LookTowards((enemy.playerActor.transform.position - enemy.transform.position).normalized);
        }
    }

    public float standartSpeed = 2;
    public float chaseSpeed = 3;
    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 10f;
    public float toleratedSightrange = 5f;      //Muss kleiner sein als sightRange!!!
    public float fieldOfView = 90;
    public float stoppingTime = 2f;
    public float playerIsCaughtDistance = 4f;

    public Waypoints wayPoints;
    [HideInInspector]
    public int currentWaypoint;

    public Transform eyes;
    public ViewCone viewCone;
    public LayerMask viewBlockingLayers;

    private int highestPriority = 0;
    private CamouflageController camouflage;

    [HideInInspector]
    public Vector3 targetPos;
    [HideInInspector]
    public Transform chaseTarget;
    [HideInInspector]
    public IEnemyState currentState;
    [HideInInspector]
    public ChaseState chaseState;
    [HideInInspector]
    public AlertState alertState;
    [HideInInspector]
    public PatrolState patrolState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    private CaughtPlayerState m_caughtPlayerState;

    private Animator enemyAnimator;
    private ElephantMovement elephantMovement;
    private bool caughtThePlayer;
    private bool firstWantedUp;
    public PlayerActor playerActor;
    public WantedLevel wantedLevel;

    public AudioSource audioSource;
    public AudioClip[] enterChase;
    public AudioClip[] exitChase;

    //---Caught event stuff-----
    public struct CaughtEventData {
        public bool caught;
        public CaughtEventData(bool caught) {
            this.caught = caught;
        }
    }

    public delegate void CaughtEvent(CaughtEventData data);
    public static event CaughtEvent OnCaught;

    //----------------------------

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        playerActor = GameObject.FindObjectOfType<PlayerActor>();
        wantedLevel = GameObject.FindObjectOfType<WantedLevel>();

        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        m_caughtPlayerState = new CaughtPlayerState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = standartSpeed;

        enemyAnimator = GetComponent<Animator>();
        elephantMovement = playerActor.GetComponent<ElephantMovement>();

        if (wayPoints == null)
            wayPoints = new Waypoints() {
                points = new[] { transform.position },
                pairs = new[] { new Pair() }
            };
    }

    void OnValidate() {
        currentState = patrolState;
    }

    void Start() {
        currentState = patrolState;

        viewCone.MainViewRadius = toleratedSightrange;
        viewCone.FullViewRadius = sightRange;
        viewCone.FieldOfView = fieldOfView;

        OnCaught += StatePatternEnemy_OnCaught;
    }

    private void StatePatternEnemy_OnCaught(CaughtEventData data) {
        GetComponent<NavMeshAgent>().enabled = false;

        currentState = m_caughtPlayerState;
    }
    void OnDestroy() {
        OnCaught -= StatePatternEnemy_OnCaught;
    }

    void Update() {
        currentState.UpdateState();

        //Stuff für den Animator
        if (currentState != alertState && enemyAnimator != null) {
            enemyAnimator.SetFloat("BlendSpeed", (float)(navMeshAgent.velocity.magnitude / chaseSpeed));
        }

        //WantedLvl abfrage
        if (wantedLevel.currentWantedStage > 0 && !firstWantedUp) {
            WantedLvlUp();
            firstWantedUp = true;
        }
    }

    public void StopMovement() {
        enemyAnimator.SetFloat("BlendSpeed", 0.0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerActor>()) {
            caughtPlayer();
            return;
        }
        currentState.OnTriggerEnter(other);
    }

    public void WantedLvlUp() {
        searchingTurnSpeed += 20f;
        sightRange += 5f;
        toleratedSightrange += 5f;
        viewCone.FieldOfView += 30f;
        viewCone.FullViewRadius += 5f;
        viewCone.MainViewRadius += 5f;
    }

    public void WantedLvlDown() {
        searchingTurnSpeed -= 20f;
        sightRange -= 5f;
        toleratedSightrange -= 5f;
        viewCone.FieldOfView -= 30f;
        viewCone.FullViewRadius -= 5f;
        viewCone.MainViewRadius -= 5f;
    }

    private void OnDrawGizmos() {
        if (currentState == this.alertState)
            Gizmos.color = Color.yellow;
        else if (currentState == this.chaseState)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position + Vector3.up, Vector3.one * 0.25f);
    }

    public void Inform(NoiseSourceData data)    //Wenn ich im PatrolState etwas höre laufe ich auf
    {
        if (!enabled) {
            return;
        }

        if (currentState == patrolState) {
            highestPriority = 0;        //Wenn ich irgendwann mal wieder in den patrolState komme ist jede Noise highestPrio
            navMeshAgent.SetDestination(data.initialPosition);
            viewCone.setAlarmed(true, 1f);
            currentState = alertState;
        } else if (currentState == alertState && data.priority >= highestPriority) {
            highestPriority = data.priority;
            navMeshAgent.SetDestination(data.initialPosition);
            navMeshAgent.Resume();
        }

    }


    public void camouflageInRange() {
        camouflage = playerActor.GetComponent<CamouflageController>();
        if (camouflage != null) {
            camouflage.EnemyInRange(this.gameObject);
            // Debug.Log("Can't camouflage!");
        }
    }

    public void camouflageNotInRange() {
        if (camouflage != null) {
            camouflage.EnemyOutOfRange(this.gameObject);
            //Debug.Log("Can camouflage now!");
        }
    }

    public bool canSeePlayer(out RaycastHit hit) {
        Vector3 enemyToPlayer = playerActor.transform.position - transform.position;
        float playerDistance = enemyToPlayer.magnitude;
        if (playerDistance <= sightRange && !elephantMovement.IsInStonePose()) {
            float angleDistance = Vector3.Angle(enemyToPlayer, transform.forward);
            if (angleDistance < fieldOfView * 0.5f) {
                float playerWidth = ((CapsuleCollider)playerActor.collider).radius;

                Vector3 enemyToPlayerOrtho = new Vector3(enemyToPlayer.z, enemyToPlayer.y, -enemyToPlayer.x).normalized;
                Vector3 playerPosLeft = playerActor.transform.position - enemyToPlayerOrtho * 0.5f * playerWidth;
                Vector3 playerPosRight = playerActor.transform.position + enemyToPlayerOrtho * 0.5f * playerWidth;

                Debug.DrawLine(transform.position, playerPosRight, Color.green);
                Debug.DrawLine(transform.position, playerPosLeft, Color.green);
                Debug.DrawLine(transform.position, playerActor.transform.position, Color.green);

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

    public void caughtPlayer() {
        caughtThePlayer = true;
        if (OnCaught != null) {
            OnCaught(new CaughtEventData(caughtThePlayer));
        }
    }

    public void LookTowards(Vector3 direction) {
        Vector3 d = Vector3.ProjectOnPlane(transform.InverseTransformDirection(direction), Vector3.up);
        float turnAmount = Mathf.Atan2(d.x, d.z);
        float turnSpeed = 180.0f;
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }
}
