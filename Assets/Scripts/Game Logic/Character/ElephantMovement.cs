using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ElephantMovement : MonoBehaviour {
    [SerializeField]
    float m_movingTurnSpeed = 360;
    [SerializeField]
    float m_stationaryTurnSpeed = 180;
    [SerializeField]
    float m_moveSpeedMultiplier = 1f;
    [SerializeField]
    float m_animSpeedMultiplier = 1f;
    [SerializeField]
    float m_groundCheckDistance = 0.1f;
    [SerializeField]
    private float m_sprintSpeed = 2.0f;
    [SerializeField]
    private float m_sprintAnimSpeedMultiplier = 2.0f;

    Rigidbody m_rigidbody;
    Animator m_animator;
    const float k_half = 0.5f;

    public float m_turnAmount { get; private set; }
    float m_forwardAmount;
    Vector3 m_groundNormal;
    float m_capsuleHeight;
    Vector3 m_capsuleCenter;
    [SerializeField]
    CapsuleCollider m_sprintCapsule;
    bool m_crouching;

    void Start() {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();

        m_rigidbody.constraints = 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY | 
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezePositionY;
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

    public void StopSprint() {
        m_animator.SetBool("Sprint", false);
        SetCapsuleForSprint(false);
    }

    public void Sprint(Vector3 dir) {
        Move(dir, m_sprintSpeed, m_sprintAnimSpeedMultiplier);
        m_animator.SetBool("Sprint", true);
        SetCapsuleForSprint(true);
    }
    
    public void Sprint(Vector3 dir, float slowDownProgress) {
        Move(dir, Mathf.Lerp(m_sprintSpeed, 0.0f, slowDownProgress), 
            Mathf.Lerp(m_sprintAnimSpeedMultiplier, 0.0f, slowDownProgress));
        m_animator.SetBool("Sprint", true);
        SetCapsuleForSprint(true);
    }

    private void Move(Vector3 dir, float speed, float animSpeed) {
        // Convert the world relative dir vector into a local-relative forward amount
        dir = transform.InverseTransformDirection(dir);
        CheckGroundStatus();
        dir = Vector3.ProjectOnPlane(dir, m_groundNormal);
        m_forwardAmount = dir.z * speed;

        // Send input and other state parameters to the animator
        UpdateAnimator(dir, animSpeed);
    }

    public void Move(Vector3 move, bool crouch) {
        Move(move.normalized, move.magnitude, m_animSpeedMultiplier);
    }

    void SetCapsuleForSprint(bool sprint) {
        m_sprintCapsule.enabled = sprint;
    }

    void UpdateAnimator(Vector3 move, float animSpeed) {
        // update the animator parameters
        m_animator.SetFloat("Forward", m_forwardAmount, 0.1f, Time.deltaTime);

        //m_rigidbody.velocity = transform.forward * m_forwardAmount * animSpeed;
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
        m_animator.SetBool("Crouch", m_crouching);
        m_animator.SetBool("OnGround", true);

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (move.magnitude > 0) {
            m_animator.speed = animSpeed;
        } else {
            m_animator.speed = 1;
        }
    }

    void ApplyExtraTurnRotation() {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_forwardAmount);
        transform.Rotate(0, m_turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    public float GetRotationSpeed()
    {
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_forwardAmount);
        return m_turnAmount * turnSpeed * Time.deltaTime;
    }

    public void OnAnimatorMove() {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (Time.deltaTime > 0) {
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
            m_animator.applyRootMotion = true;
        } else {
            m_groundNormal = Vector3.up;
            m_animator.applyRootMotion = false;
        }
    }
}

