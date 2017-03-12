using UnityEngine;
using System.Collections;

public class SpringLookAtCamera : AbstractSpringCamera {
    [SerializeField]
    private float m_maxDistance = 8.0f;

    [SerializeField]
    private bool m_followAtMaxDistance = true;

    override protected void Start() {
        base.Start();
        m_cameraDestination = transform.position;
        m_targetPos = m_target.position + m_targetOffset;
    }

    protected override void UpdateTarget(float deltaTime) {
        m_targetPos = m_target.position + m_targetOffset;

        if (m_followAtMaxDistance) {
            Vector3 distance = m_targetPos - m_cameraDestination;
            Vector3 distanceOnXZPlane = new Vector3(distance.x, 0.0f, distance.z);

            if (distanceOnXZPlane.magnitude > m_maxDistance) {
                Vector3 d = distanceOnXZPlane - distanceOnXZPlane.normalized * m_maxDistance;
                m_cameraDestination += d;
            }
        }
    }
}
