using System;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ElephantMovement), typeof(Interactor))]
public class ElephantControl : MonoBehaviour {
    public enum ControlsMode {
        Controller,
        KeyboardMouse
    }

    private ElephantMovement m_character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_camForward;             // The current forward direction of the camera

    [SerializeField]
    private Cooldown m_sprintCooldown = new Cooldown(2.0f);
    [SerializeField]
    private Cooldown m_sprintDurationAfterSprintStopped = new Cooldown(0.5f);

    private Vector3 m_sprintDirection;

    public ControlsMode controlsMode = ControlsMode.Controller;

    private bool m_sprinting = false;
    private bool m_sprintJustStarted = false;
    private bool m_sprintJustEnded = false;

    [SerializeField]
    private float m_walkRadius = 0.4f;
    private Shoot_Peanuts m_shootPeanuts;
    private Interactor m_interactor;

    private void Start() {
        m_sprintCooldown.End();
        m_sprintDurationAfterSprintStopped.End();
        m_shootPeanuts = GetComponent<Shoot_Peanuts>();

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
        m_interactor = GetComponent<Interactor>();
    }

    private bool IsCrouching() {
        return CrossPlatformInputManager.GetButton("Stealth");
    }

    private void HandleControllerMovement() {
        var move = GetAxisMove();

        if (move.magnitude > 0.001f) {
            UpdateSprint(move);
        } else {
            UpdateSprint(transform.forward);
        }
        
        if (!m_sprinting && m_sprintDurationAfterSprintStopped.IsOver()) {
            m_character.Move(move, IsCrouching());
            m_character.LookTowards(move);
        }
    }

    private Vector3 GetAxisMove() {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        Vector3 move;

        // calculate move direction to pass to character
        if (m_cam != null) {
            // calculate camera relative direction to move:
            m_camForward = Vector3.Scale(m_cam.forward, new Vector3(1, 0, 1)).normalized;
            move = v * m_camForward + h * m_cam.right;
        } else {
            // we use world-relative directions in the case of no main camera
            move = v * Vector3.forward + h * Vector3.right;
        }

        return move;
    }

    private Vector3 desiredMouseLookDelta {
        get {
            Vector2 mp = Input.mousePosition;
            Vector2 center = Camera.main.WorldToScreenPoint(transform.position);
            var screenLookDelta = (center - mp);
            var desiredLookDelta = new Vector3(-screenLookDelta.x, 0.0f, -screenLookDelta.y);
            desiredLookDelta /= Screen.height * 0.5f;

            return desiredLookDelta;
        }
    }

    private Vector3 desiredMouseLookDir {
        get {
            return desiredMouseLookDelta.normalized;
        }
    }

    private void HandleMouseKeyboardMovement() {
        var move = Vector3.zero;

        if (Input.GetMouseButton(0)) {
            move = desiredMouseLookDelta / m_walkRadius;
            if (move.magnitude > 1.0f) {
                move.Normalize();
            }
        }

        UpdateSprint(desiredMouseLookDir);

        if (!m_sprinting && m_sprintDurationAfterSprintStopped.IsOver()) {
            m_character.LookTowards(desiredMouseLookDir);
            m_character.Move(move, IsCrouching());
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

        switch (controlsMode) {
            case ControlsMode.Controller:
                if (Input.GetButton("TargetMode")) {
                    HandleAiming();
                } else {
                    HandleControllerMovement();
                }
                break;
            case ControlsMode.KeyboardMouse:
                HandleMouseKeyboardMovement();
                break;
        }
    }

    private void Update() {
        if (m_shootPeanuts && Input.GetButtonDown("Shoot")) {
            m_shootPeanuts.Fire();
        }

        if (Input.GetButtonDown("Interact")) {
            m_interactor.InteractionRequest();
        }
    }

    private void HandleAiming() {
        m_character.Move(Vector3.zero, false);
        m_character.LookTowards(desiredMouseLookDir);
    }

    public Cooldown sprintCooldown {
        get {
            return m_sprintCooldown;
        }
    }
}
