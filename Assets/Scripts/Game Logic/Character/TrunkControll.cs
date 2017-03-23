using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(ElephantMovement), typeof(ElephantControl))]
public class TrunkControll : MonoBehaviour {
    [SerializeField]
    private bool m_controllWithSlider = false;

    [Range(0,1)]
    public float desiredTrunkStiffness = 1.0f;

    [SerializeField]
    private Animator m_animatorForTrunk;

    private ElephantMovement m_elefantmovement;

    [SerializeField]
    private Transform m_trunkRoot;

    [SerializeField]
    private float m_transitionSpeed = 0.5f;
    private float m_trunkStiffness = 0.0f;

    private Cooldown m_stiffnessChangeCooldown = new Cooldown(0.1f);
    private float m_rotationSpeedEpsilon = 0.1f;
    private ElephantControl m_elephantControl;

    // Use this for initialization
    void Start () {
        m_elefantmovement = GetComponent<ElephantMovement>();

        foreach (var collisionNotfier in m_trunkRoot.GetComponentsInChildren<CollisionNotifier>()) {
            collisionNotfier.OnTriggerStayNotification += OnTriggerStayNotification;
        }

        m_elephantControl = GetComponent<ElephantControl>();
    }

    private void OnDestroy() {
        foreach (var collisionNotfier in m_trunkRoot.GetComponentsInChildren<CollisionNotifier>()) {
            collisionNotfier.OnTriggerStayNotification -= OnTriggerStayNotification;
        }
    }

    private void OnTriggerStayNotification(Collider other) {
        if (other.isTrigger) {
            return;
        }
        m_stiffnessChangeCooldown.Start();
    }

    void Update () {
        if (m_elefantmovement.IsInStonePose()) {
            return;
        }

        if (m_controllWithSlider) {
            m_animatorForTrunk.SetFloat("TrunkStiffness", desiredTrunkStiffness);
            return;
        }

        m_stiffnessChangeCooldown.Update(Time.deltaTime);

        if (m_stiffnessChangeCooldown.IsOver() && Mathf.Abs(m_elefantmovement.GetRotationSpeed()) > m_rotationSpeedEpsilon ||
            m_elephantControl.aiming) {
            desiredTrunkStiffness = 1.0f;
        } else {
            desiredTrunkStiffness = 0.0f;
        }

        float direction = 0.0f;
        if (m_trunkStiffness < desiredTrunkStiffness - Mathf.Epsilon) {
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
