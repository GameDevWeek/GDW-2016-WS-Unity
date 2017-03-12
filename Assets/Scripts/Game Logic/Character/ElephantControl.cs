using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Elephant))]
public class ElephantControl : MonoBehaviour {
    private Elephant m_character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_camForward;             // The current forward direction of the camera
    private Vector3 m_move;
    private bool m_jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

    [SerializeField]
    private bool m_mouseMovement = true;

    private void Start() {
        // get the transform of the main camera
        if (Camera.main != null) {
            m_cam = Camera.main.transform;
        } else {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_character = GetComponent<Elephant>();
    }


    private void Update() {
        if (!m_jump) {
            m_jump = CrossPlatformInputManager.GetButtonDown("Jump");
            if (m_jump) {
                Camera.main.GetComponent<CameraShake>().Shake();
            }
        }
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate() {
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
#if !MOBILE_INPUT
        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) m_move *= 0.5f;
#endif

        Vector2 mp = Input.mousePosition;
        var screenLookDir = (new Vector2(Screen.width, Screen.height) * 0.5f - mp).normalized;
        var lookDir = new Vector3(-screenLookDir.x, 0.0f, -screenLookDir.y);
        m_character.LookTowards(lookDir);

        if (m_mouseMovement && Input.GetMouseButton(0)) {
            m_move = lookDir;
        }

        // pass all parameters to the character control script
        m_character.Move(m_move, crouch, m_jump);
        m_jump = false;
    }
}
