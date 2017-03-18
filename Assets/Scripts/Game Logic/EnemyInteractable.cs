using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(StatePatternEnemy), typeof(NavMeshAgent))]
public class EnemyInteractable : Interactable {

    public struct StunEventData {
        public GameObject stunner, stunned;

        public StunEventData(GameObject stunner, GameObject stunned) {
            this.stunner = stunner;
            this.stunned = stunned;
        }
    }

    public delegate void StunEvent(StunEventData data);
    public static event StunEvent OnStun;

    private float time_stunned = 0f;
    private static float maxStunTime;
    public static event Action<float> stunTimeArchieved;

    [SerializeField]
    private Cooldown m_cooldown = new Cooldown(0.5f);
    [SerializeField]
    private Cooldown m_stunDuration = new Cooldown(5.0f);

    [SerializeField]
    private float m_backAngle = 30.0f;
    private Coroutine m_stunRoutine;

    public AudioClip stunEnemyHitSound;

    void Update() {
        m_cooldown.Update(Time.deltaTime);

        if (Stunned()) {
            time_stunned += Time.deltaTime;

            if (time_stunned > maxStunTime) {
                maxStunTime = time_stunned;
                if (stunTimeArchieved != null) stunTimeArchieved.Invoke(maxStunTime);
            }


        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        Handles.DrawSolidArc(position, Vector3.up,
            Quaternion.AngleAxis(-m_backAngle * 0.5f, Vector3.up) * -transform.forward,
            m_backAngle, m_interactionRange);
    }
#endif

    public override void Interact(Interactor interactor) {
        if (!IsStunPossible(interactor)) {
            return;
        }

        m_stunRoutine = StartCoroutine(StunRoutine(interactor));
    }

    IEnumerator StunRoutine(Interactor interactor) {
        ElephantMovement movement = interactor.GetComponent<ElephantMovement>();
        if (movement) {
            movement.Punch();
        }

        if (OnStun != null) {
            OnStun(new StunEventData(interactor.gameObject, gameObject));
        }

        var stateEnemy = GetComponent<StatePatternEnemy>();
        var navMeshAgent = GetComponent<NavMeshAgent>();
        stateEnemy.StopMovement();
        stateEnemy.enabled = false;
        navMeshAgent.enabled = false;
        interactor.GetComponent<AudioSource>().PlayOneShot(stunEnemyHitSound);
        while (!m_stunDuration.IsOver()) {
            m_stunDuration.Update(Time.deltaTime);
            yield return null;
        }

        m_stunDuration.Start();
        stateEnemy.enabled = true;
        navMeshAgent.enabled = true;
        m_stunRoutine = null;
    }

    public bool IsStunPossible(Interactor stunner) {
        return Vector3.Angle(stunner.transform.forward, transform.forward) < m_backAngle &&
            m_cooldown.IsOver() && m_stunRoutine == null && stunner.GetComponent<ElephantMovement>().CanMove();
    }

    public bool Stunned() {
        return m_stunRoutine != null;
    }

    public Cooldown stunDuration {
        get {
            return m_stunDuration;
        }
    }
}
