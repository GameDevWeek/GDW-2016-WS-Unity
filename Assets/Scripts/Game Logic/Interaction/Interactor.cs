using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Interactor : MonoBehaviour {
    [SerializeField]
    private float m_interactionDegree = 45.0f;
    
    [SerializeField]
    private Vector3 m_positionOffset = new Vector3();

    [SerializeField]
    private LayerMask m_interactionBlockerMask = ~0;
    [SerializeField]
    private LayerMask m_interactableLayerMask = ~0;
    [SerializeField]
    private float m_interactionRadius = 3.0f;
    private Interactable m_curInteractable;

    public Vector3 origin {
        get {
            return transform.position + m_positionOffset;
        }
    }

    void OnDrawGizmosSelected() {
        Handles.color = new Color(1.0f, 0.0f, 1.0f, 0.3f);
        Handles.DrawSolidArc(origin, Vector3.up, 
            Quaternion.AngleAxis(-m_interactionDegree * 0.5f, Vector3.up) * transform.forward, 
            m_interactionDegree, m_interactionRadius);
    }

    private bool IsVisible(Interactable interactable) {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(origin, (interactable.position - origin).normalized, 
            out hitInfo, m_interactionRadius, m_interactionBlockerMask);

        return hit && hitInfo.collider.GetComponent<Interactable>() == interactable;
    }

    Interactable FindMinInteractable() {
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_interactableLayerMask);

        float minDegree = float.MaxValue;
        GameObject minInteractable = null;

        // Find closest match (smallest degree to interactable)
        foreach (Collider collider in colliders) {
            var interactable = collider.GetComponent<Interactable>();
            if (interactable == null || !collider.gameObject) {
                continue;
            }

            float degree = DegreeTo(collider.gameObject);
            if (degree < m_interactionDegree * 0.5f && degree < minDegree && 
                interactable.interactionActive && IsVisible(interactable)) {
                minDegree = degree;
                minInteractable = collider.gameObject;
            }
        }

        if (minInteractable) {
            return minInteractable.GetComponent<Interactable>();
        }

        return null;
    }

    void Update() {
        m_curInteractable = FindMinInteractable();

        if (m_curInteractable) {
            Debug.DrawLine(origin, m_curInteractable.transform.position, Color.red);
        }
    }

    public void InteractionRequest() {
        if (m_curInteractable == null) {
            return;
        }

        m_curInteractable.Interact(this);
    }

    private float DegreeTo(GameObject go) {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 otherPos = go.transform.position + go.GetComponent<Interactable>().positionOffset;

        Vector3 delta = Vector3.ProjectOnPlane(otherPos - origin, Vector3.up);

        return Vector3.Angle(transform.forward, delta);
    }
}
