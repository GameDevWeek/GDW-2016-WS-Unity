using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(ElephantMovement))]
public class TrunkControll : MonoBehaviour {
    [SerializeField]
    private bool m_controllWithSlider = false;

    [Range(0,1)]
    public float desiredTrunkStiffness = 1.0f;

    [SerializeField]
    private Animator m_animatorForTrunk;

    private Animator m_animator;
    private ElephantMovement m_elefantmovement;

    private float m_value = 0;

    [SerializeField]
    private float m_damping = 1.0f;

    [SerializeField]
    private Transform m_trunkRoot;

    [SerializeField]
    private float m_transitionSpeed = 0.5f;
    private float m_trunkStiffness = 0.0f;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_elefantmovement = GetComponent<ElephantMovement>();

        foreach (var collisionNotfier in m_trunkRoot.GetComponentsInChildren<CollisionNotifier>()) {
            collisionNotfier.OnCollisionStayNotification += OnCollisionStayNotification;
        }
    }

    private void OnDestroy() {
        foreach (var collisionNotfier in m_trunkRoot.GetComponentsInChildren<CollisionNotifier>()) {
            collisionNotfier.OnCollisionStayNotification -= OnCollisionStayNotification;
        }
    }

    private void OnCollisionStayNotification(Collision collision) {
        desiredTrunkStiffness = 0.0f;
        Debug.Log("Yo");
    }

    // Update is called once per frame
    void Update () {
        //desiredTrunkStiffness = 1.0f;

        //m_value -= m_damping * Time.deltaTime;
        //m_value += Mathf.Abs(m_elefantmovement.GetRotationSpeed()) / 45.0f;
        //m_value = Mathf.Clamp(m_value, 0, 2);

        //if (!m_controllWithSlider)
        //{
        //    m_animatorForTrunk.SetFloat("TrunkStiffness", m_value); // m_animator.GetFloat("Forward") + 
        //} else
        //{
        //    m_animatorForTrunk.SetFloat("TrunkStiffness", trunkStiffness);
        //}

        float direction = 0.0f;
        if (m_trunkStiffness < desiredTrunkStiffness + Mathf.Epsilon) {
            direction = 1.0f;
        } else if (m_trunkStiffness > desiredTrunkStiffness + Mathf.Epsilon) {
            direction = - 1.0f;
        }

        m_trunkStiffness += m_transitionSpeed * Time.deltaTime * direction;
        Mathf.Clamp(m_trunkStiffness, desiredTrunkStiffness, 1.0f);
        Mathf.Clamp01(m_trunkStiffness);

        m_animatorForTrunk.SetFloat("TrunkStiffness", m_trunkStiffness);
    }
}
