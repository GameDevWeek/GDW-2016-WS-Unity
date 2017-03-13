using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class AbstractSpringCamera : AbstractCamera {
    [SerializeField]
    protected float m_elevation = Mathf.PI / 4.0f;

    [SerializeField]
    protected float m_azimuth = 0.0f;

    [SerializeField]
    protected Vector3 m_targetOffset = new Vector3(0, 1, 0);

    [SerializeField]
    protected float m_distance = 3.0f;
    protected float m_actualDistance;

    [SerializeField]
    protected float m_distSpringConstant = 50.0f;

    [SerializeField]
    private string m_defaultExcludeFromWallClipTag = "Player";
    [SerializeField]
    private List<string> m_excludeFromWallClipTags = new List<string>();
    [SerializeField]
    private bool m_protectFromWallClip = true;
    [SerializeField]
    private float m_sphereCastRadius = 0.1f;
    [SerializeField]
    private float m_minDistance = 0.05f;

    [SerializeField]
    protected float m_rotationSpeed = 0.05f;

    private Vector3 m_curVelocity = new Vector3();
    private Vector3 m_lookVelocity = new Vector3();
    private Vector3 m_curLookAtPos = new Vector3();

    protected Vector3 m_cameraDestination;
    protected Vector3 m_targetPos;

    public float azimuth {
        set { m_azimuth = value; }
        get { return m_azimuth; }
    }

    public float elevation {
        set { m_elevation = value; }
        get { return m_elevation; }
    }

    public float distance {
        set { m_distance = value; }
        get { return m_distance; }
    }

    public Vector3 destination {
        set { m_cameraDestination = value; }
        get { return m_cameraDestination; }
    }

	override protected void Awake() {
		base.Awake();
        m_actualDistance = m_distance;

        if (m_defaultExcludeFromWallClipTag != "" && !m_excludeFromWallClipTags.Contains(m_defaultExcludeFromWallClipTag))
            m_excludeFromWallClipTags.Add(m_defaultExcludeFromWallClipTag);
    }

    protected override void FollowTarget(float deltaTime) {
        if (Time.timeScale < float.Epsilon)
            return;

        if (m_protectFromWallClip)
            ProtectFromWallClip();

        UpdateTarget(deltaTime);
        UpdatePosition(m_cameraDestination, deltaTime);
        UpdateLookAt(m_targetPos, deltaTime);
    }

    protected abstract void UpdateTarget(float deltaTime);

    private void ProtectFromWallClip() {
        m_actualDistance = m_distance;

        var targetPos = m_targetPos;
        var direction = m_cameraDestination - targetPos;


        //var hits = Physics.RaycastAll(targetPos, direction, m_distance);
        var hits = Physics.SphereCastAll(targetPos, m_sphereCastRadius, direction, m_distance);
        Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        foreach (var hit in hits) {
            bool skip = false;
            foreach (var excludeTag in m_excludeFromWallClipTags)
                if (hit.collider.CompareTag(excludeTag)) {
                    skip = true;
                    break;
                }

            if (skip)
                continue;

            m_actualDistance = Mathf.Max(m_minDistance, hit.distance - 0.1f);
            Debug.DrawLine(hit.point, targetPos);
            break;
        }
    }

    private void UpdateLookAt(Vector3 target, float deltaTime) {
        Vector3 lookAccel = ComputeSpringAccel(m_curLookAtPos, target, m_lookVelocity);
        m_lookVelocity += lookAccel * deltaTime;
        m_curLookAtPos += m_lookVelocity * deltaTime;
        GetComponent<Transform>().LookAt(m_curLookAtPos);
    }

    private void UpdatePosition(Vector3 target, float deltaTime) {
        Vector3 accel = ComputeSpringAccel(transform.position, target, m_curVelocity);
        m_curVelocity += accel * deltaTime;
        transform.position += m_curVelocity * deltaTime;
    }

    // Computes the acceleration using a critical damping spring model
    protected Vector3 ComputeSpringAccel(Vector3 pos, Vector3 target, Vector3 velocity) {
        float dampingCoefficient = 2.0f * Mathf.Sqrt(m_distSpringConstant);

        Vector3 deltaAccel = (target - pos) * m_distSpringConstant;
        Vector3 dampingAccel = -velocity * dampingCoefficient;

        return deltaAccel + dampingAccel;
    }
}
