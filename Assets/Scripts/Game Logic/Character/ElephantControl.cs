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

    [SerializeField]
    private bool m_mouseMovement = true;
    private bool m_sprinting = false;

    private void Start() {
        m_sprintCooldown.End();

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

    // Fixed update is called in sync with physics
    private void FixedUpdate() {
        m_sprintCooldown.Update(Time.fixedDeltaTime);
        m_sprintDurationAfterSprintStopped.Update(Time.fixedDeltaTime);

        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        if (m_cam != null) {
            // calculate camera relative direction to move:
            m_camForward = Vector3.Scale(m_cam.forward, new Vector3(1, 0, 1)).normalized;
            m_move = v * m_camForward + h * m_cam.right;
        } else {
            // we use world-relative directions in the case of no main camera
            m_move = v * Vector3.forward + h * Vector3.right;
        }

        Vector2 mp = Input.mousePosition;
        var screenLookDir = (new Vector2(Screen.width, Screen.height) * 0.5f - mp).normalized;
        var desiredLookDir = new Vector3(-screenLookDir.x, 0.0f, -screenLookDir.y);
        
        if (m_mouseMovement && Input.GetMouseButton(0)) {
            m_move = desiredLookDir;
        }

        if (!m_sprinting) {
            if (m_mouseMovement) {
                m_character.LookTowards(desiredLookDir);
            } else {
                m_character.LookTowards(m_move);
            }
        }

        if (m_sprintCooldown.IsOver() || !m_sprintDurationAfterSprintStopped.IsOver()) {
            bool sprint = Input.GetButton("Jump");
            bool sprintJustStarted = !m_sprinting && sprint;
            bool sprintJustStopped = m_sprinting && !sprint;
            m_sprinting = sprint;

            if (sprintJustStopped) {
                m_sprintCooldown.Start();
                m_sprintDurationAfterSprintStopped.Start();
            }

            if (sprintJustStarted) {
                m_sprintDirection = desiredLookDir;

                if (Camera.main.GetComponent<CameraShake>()) {
                    Camera.main.GetComponent<CameraShake>().Shake();
                }
            }

            if (sprint && m_sprintCooldown.IsOver() || !m_sprintDurationAfterSprintStopped.IsOver()) {
                m_character.LookTowards(m_sprintDirection);

                float t = m_sprintDurationAfterSprintStopped.IsOver() ? 0.0f : m_sprintDurationAfterSprintStopped.progress;
                m_character.Sprint(transform.forward, t);
            } else {
                m_character.Move(m_move, crouch);
            }
        } else {
            m_character.Move(m_move, crouch);
        }
    }

    public Cooldown sprintCooldown {
        get {
            return m_sprintCooldown;
        }
    }
}
