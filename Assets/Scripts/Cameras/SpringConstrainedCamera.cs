using UnityEngine;
using System.Collections;
using System;

public class SpringConstrainedCamera : AbstractSpringCamera {
    override protected void Start() {
        base.Start();
        m_cameraDestination = transform.position;
        m_targetPos = m_target.position + m_targetOffset;

        Vector3 distance = m_cameraDestination - m_targetPos;
        Polar polar = Util.VectorToPolar(distance);
        m_elevation = polar.elevation;
        m_azimuth = polar.azimuth;
    }

    protected override void UpdateTarget(float deltaTime) {
        Vector3 polar = Util.PolarToVector(m_actualDistance, m_elevation, m_azimuth);

        m_targetPos = m_target.position + m_targetOffset;
        m_cameraDestination = m_target.position + polar + m_targetOffset;
    }
}
