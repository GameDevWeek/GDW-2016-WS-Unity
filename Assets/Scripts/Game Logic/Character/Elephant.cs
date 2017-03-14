using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[Obsolete]
public class Elephant : MonoBehaviour {
    [SerializeField]
    float m_movingTurnSpeed = 360;
    [SerializeField]
    float m_stationaryTurnSpeed = 180;
    [SerializeField]
    float m_jumpPower = 12f;
    [Range(1f, 4f)]
    [SerializeField]
    float m_gravityMultiplier = 2f;
    [SerializeField]
    float m_runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField]
    float m_moveSpeedMultiplier = 1f;
    [SerializeField]
    float m_animSpeedMultiplier = 1f;
    [SerializeField]
    float m_groundCheckDistance = 0.1f;

    Rigidbody m_rigidbody;
    Animator m_animator;
    bool m_isGrounded;
    float m_origGroundCheckDistance;
    const float k_half = 0.5f;
    float m_turnAmount;
    float m_forwardAmount;
    Vector3 m_groundNormal;
    float m_capsuleHeight;
    Vector3 m_capsuleCenter;
    CapsuleCollider m_capsule;
    bool m_crouching;


    void Start() {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
        m_capsuleHeight = m_capsule.height;
        m_capsuleCenter = m_capsule.center;

        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_origGroundCheckDistance = m_groundCheckDistance;
    }

    public void LookAt(Vector3 position) {
        LookTowards((position - transform.position).normalized);
    }

    public void LookTowards(Vector3 direction) {
        Vector3 d = Vector3.ProjectOnPlane(transform.InverseTransformDirection(direction), m_groundNormal);
        m_turnAmount = Mathf.Atan2(d.x, d.z);
        GetComponent<LineRenderer>().SetPositions(new Vector3[] { transform.position, transform.position + direction });
        ApplyExtraTurnRotation();
    }

    public void Move(Vector3 move, bool crouch, bool jump) {

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_groundNormal);
        //m_turnAmount = Mathf.Atan2(move.x, move.z);
        m_forwardAmount = move.z;

        //ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_isGrounded) {
            HandleGroundedMovement(crouch, jump);
        } else {
            HandleAirborneMovement();
        }

        ScaleCapsuleForCrouching(crouch);
        //PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }


    void ScaleCapsuleForCrouching(bool crouch) {
        if (m_isGrounded && crouch) {
            if (m_crouching) return;
            m_capsule.height = m_capsule.height / 2f;
            m_capsule.center = m_capsule.center / 2f;
            m_crouching = true;
        } else {
            m_capsule.height = m_capsuleHeight;
            m_capsule.center = m_capsuleCenter;
            m_crouching = false;
        }
    }

    void PreventStandingInLowHeadroom() {
        // prevent standing up in crouch-only zones
        if (!m_crouching) {
            Ray crouchRay = new Ray(m_rigidbody.position + Vector3.up * m_capsule.radius * k_half, Vector3.up);
            float crouchRayLength = m_capsuleHeight - m_capsule.radius * k_half;
            if (Physics.SphereCast(crouchRay, m_capsule.radius * k_half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                m_crouching = true;
            }
        }
    }


    void UpdateAnimator(Vector3 move) {
        // update the animator parameters
        m_animator.SetFloat("Forward", m_forwardAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
        m_animator.SetBool("Crouch", m_crouching);
        m_animator.SetBool("OnGround", m_isGrounded);
        if (!m_isGrounded) {
            m_animator.SetFloat("Jump", m_rigidbody.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_runCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_half ? 1 : -1) * m_forwardAmount;
        if (m_isGrounded) {
            m_animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_isGrounded && move.magnitude > 0) {
            m_animator.speed = m_animSpeedMultiplier;
        } else {
            // don't use that while airborne
            m_animator.speed = 1;
        }
    }


    void HandleAirborneMovement() {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_gravityMultiplier) - Physics.gravity;
        m_rigidbody.AddForce(extraGravityForce);

        m_groundCheckDistance = m_rigidbody.velocity.y < 0 ? m_origGroundCheckDistance : 0.01f;
    }


    void HandleGroundedMovement(bool crouch, bool jump) {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) {
            // jump!
            m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_jumpPower, m_rigidbody.velocity.z);
            m_isGrounded = false;
            m_animator.applyRootMotion = false;
            m_groundCheckDistance = 0.1f;
        }
    }

    void ApplyExtraTurnRotation() {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_forwardAmount);
        transform.Rotate(0, m_turnAmount * turnSpeed * Time.deltaTime, 0);
    }


    public void OnAnimatorMove() {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_isGrounded && Time.deltaTime > 0) {
            Vector3 v = (m_animator.deltaPosition * m_moveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = m_rigidbody.velocity.y;
            m_rigidbody.velocity = v;
        }
    }


    void CheckGroundStatus() {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_groundCheckDistance)) {
            m_groundNormal = hitInfo.normal;
            m_isGrounded = true;
            m_animator.applyRootMotion = true;
        } else {
            m_isGrounded = false;
            m_groundNormal = Vector3.up;
            m_animator.applyRootMotion = false;
        }
    }
}

