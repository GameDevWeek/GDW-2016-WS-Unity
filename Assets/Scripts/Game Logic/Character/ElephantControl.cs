using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ElephantMovement))]
public class ElephantControl : MonoBehaviour {
    private ElephantMovement m_character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_camForward;             // The current forward direction of the camera
    private Vector3 m_move;

    [SerializeField]
    private Cooldown m_sprintCooldown = new Cooldown(2.0f);
    [SerializeField]
    private Cooldown m_sprintDurationAfterSprintStopped = new Cooldown(0.5f);

    private Vector3 m_sprintDirection;

    public bool m_useController = false;

    private bool m_sprinting = false;
    private bool m_sprintJustStarted = false;
    private bool m_sprintJustEnded = false;

    private void Start() {
        m_sprintCooldown.End();
        m_sprintDurationAfterSprintStopped.End();

        // get the transform of the main camera
        if (Camera.main != null) {
            m_cam = Camera.main.transform;
        } else {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_character = GetComponent<ElephantMovement>();
    }

    private void HandleControllerMovement() {
        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        bool crouch = CrossPlatformInputManager.GetButton("Stealth");

        // calculate move direction to pass to character
        if (m_cam != null) {
            // calculate camera relative direction to move:
            m_camForward = Vector3.Scale(m_cam.forward, new Vector3(1, 0, 1)).normalized;
            m_move = v * m_camForward + h * m_cam.right;
        } else {
            // we use world-relative directions in the case of no main camera
            m_move = v * Vector3.forward + h * Vector3.right;
        }

        if (m_move.magnitude > 0.001f) {
            UpdateSprint(m_move);
        } else {
            UpdateSprint(transform.forward);
        }
        
        if (!m_sprinting && m_sprintDurationAfterSprintStopped.IsOver()) {
            m_character.Move(m_move, crouch);
            m_character.LookTowards(m_move);
        }
    }

    private void HandleMouseKeyboardMovement() {
        m_move = Vector3.zero;
        bool crouch = CrossPlatformInputManager.GetButton("Stealth");

        Vector2 mp = Input.mousePosition;
        var screenLookDir = (new Vector2(Screen.width, Screen.height) * 0.5f - mp).normalized;
        var desiredLookDir = new Vector3(-screenLookDir.x, 0.0f, -screenLookDir.y);

        if (Input.GetMouseButton(0)) {
            m_move = desiredLookDir;
        }

        UpdateSprint(desiredLookDir);

        if (!m_sprinting && m_sprintDurationAfterSprintStopped.IsOver()) {
            m_character.LookTowards(desiredLookDir);
            m_character.Move(m_move, crouch);
        }
    }

    private void UpdateSprint(Vector3 direction) {
        bool sprint = Input.GetButton("Sprint");
        m_sprintJustStarted = !m_sprinting && sprint;
        m_sprintJustEnded = m_sprinting && !sprint;
        m_sprinting = sprint;

        if (m_sprintCooldown.IsOver() || !m_sprintDurationAfterSprintStopped.IsOver()) {
            if (m_sprintJustEnded) {
                m_sprintCooldown.Start();
                m_sprintDurationAfterSprintStopped.Start();
            }

            if (m_sprintJustStarted) {
                m_sprintDirection = direction;

                if (Camera.main.GetComponent<CameraShake>()) {
                    Camera.main.GetComponent<CameraShake>().Shake();
                }
            }

            if (m_sprinting && m_sprintCooldown.IsOver() || !m_sprintDurationAfterSprintStopped.IsOver()) {
                m_character.LookTowards(m_sprintDirection);

                float t = m_sprintDurationAfterSprintStopped.IsOver() ? 0.0f : m_sprintDurationAfterSprintStopped.progress;
                m_character.Sprint(transform.forward, t);
            }
        }
    }

    private void FixedUpdate() {
        m_sprintCooldown.Update(Time.fixedDeltaTime);
        m_sprintDurationAfterSprintStopped.Update(Time.fixedDeltaTime);

        if (m_useController) {
            HandleControllerMovement();
        } else {
            HandleMouseKeyboardMovement();
        }
    }

    public Cooldown sprintCooldown {
        get {
            return m_sprintCooldown;
        }
    }
}
