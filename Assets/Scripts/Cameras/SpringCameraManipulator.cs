using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SpringCameraManipulator : MonoBehaviour {
    [SerializeField]
    private AbstractSpringCamera m_springCamera;

    [SerializeField]
    private bool m_forwardToAzimuthOnTriggerEnter = false;

    [SerializeField]
    private bool m_changePositionOnTriggerEnter = false;

    [SerializeField]
    private bool m_setPositionToSpotChildOnTriggerEnter = false;

    [SerializeField]
    private bool m_setDestinationToSpotChildOnTriggerEnter = false;

    [SerializeField]
    private bool m_changeAzimuthOnTriggerEnter = false;

    [SerializeField]
    private bool m_changeElevationOnTriggerEnter = false;

    [SerializeField]
    private bool m_changeDistanceOnTriggerEnter = false;

    [SerializeField]
    private float m_azimuthOnTriggerEnter;

    [SerializeField]
    private float m_elevationOnTriggerEnter;

    [SerializeField]
    private float m_distanceOnTriggerEnter;

    [SerializeField]
    private Vector3 m_positionOnTriggerEnter;

    private Transform m_spot;

    // Use this for initialization
    void Start() {
        if (!m_springCamera)
            m_springCamera = Camera.main.GetComponent<AbstractSpringCamera>();

        if (m_springCamera) {
            // Get an active spring camera
            var springCams = m_springCamera.GetComponents<AbstractSpringCamera>();
            foreach (var cam in springCams)
                if (cam.enabled) {
                    m_springCamera = cam;
                    break;
                }
        }

        m_spot = Util.FindChildInHierarchy<Transform>(transform, "Spot");
    }

    public void OnTriggerEnter(Collider other) {
        if (!m_springCamera || other.isTrigger)
            return;

        if (m_changeAzimuthOnTriggerEnter)
            m_springCamera.azimuth = Mathf.Deg2Rad * m_azimuthOnTriggerEnter;

        if (m_changeElevationOnTriggerEnter)
            m_springCamera.elevation = Mathf.Deg2Rad * m_elevationOnTriggerEnter;

        if (m_changeDistanceOnTriggerEnter)
            m_springCamera.distance = m_distanceOnTriggerEnter;

        if (m_forwardToAzimuthOnTriggerEnter) {
            var polar = Util.VectorToPolar(transform.forward);
            m_springCamera.azimuth = polar.azimuth;
        }

        if (m_changePositionOnTriggerEnter) {
            m_springCamera.transform.position = m_positionOnTriggerEnter;
            m_springCamera.destination = m_positionOnTriggerEnter;
        }

        if (m_setPositionToSpotChildOnTriggerEnter && m_spot) {
            m_springCamera.transform.position = m_spot.position;
            m_springCamera.destination = m_spot.position;
        }

        if (m_setDestinationToSpotChildOnTriggerEnter && m_spot) {
            m_springCamera.destination = m_spot.position;
        }
    }
}
