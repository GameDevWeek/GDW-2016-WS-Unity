using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class SpringFreeLookCamera : AbstractSpringCamera {
    [SerializeField]
    private float m_minElevation = 45.0f;
    [SerializeField]
    private float m_maxElevation = 75.0f;
    [SerializeField]
    private float m_lookSpeedJoypad = 60.0f;
    [SerializeField]
    private float m_lookSpeedMouse = 0.5f;

    public float horizontal;
    public float vertical;

    protected override void UpdateTarget(float deltaTime) {
        // Need deltaTime for joystick to make it framerate independent
        float dt = Time.deltaTime * m_lookSpeedJoypad;

        // TODO: Supposed to be controlled by a different script. Just here for quick tests.
        horizontal = -CrossPlatformInputManager.GetAxis("Mouse X") * m_lookSpeedMouse;
        vertical = -CrossPlatformInputManager.GetAxis("Mouse Y") * m_lookSpeedMouse;

        var x = horizontal * dt;
        var y = vertical * dt;

        m_elevation = Mathf.Clamp(m_elevation + y * m_rotationSpeed, -Mathf.Deg2Rad * m_minElevation, Mathf.Deg2Rad * m_maxElevation);
        m_azimuth += x * m_rotationSpeed;

        Vector3 polar = Util.PolarToVector(m_actualDistance, m_elevation, m_azimuth);

        m_targetPos = m_target.position + m_targetOffset;
        m_cameraDestination = m_target.position + polar + m_targetOffset;
    }
}
