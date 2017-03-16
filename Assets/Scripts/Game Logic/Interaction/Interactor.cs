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
    [Tooltip("Note that interactables can have their own interaction range.")]
    [SerializeField]
    private float m_interactionRange = 3.0f;
    private Interactable m_curInteractable;

    public Vector3 position {
        get {
            return transform.position + m_positionOffset;
        }
    }

    void OnDrawGizmosSelected() {
        if(! this.enabled) return;
        Handles.color = new Color(1.0f, 0.0f, 1.0f, 0.3f);
        Handles.DrawSolidArc(position, Vector3.up, 
            Quaternion.AngleAxis(-m_interactionDegree * 0.5f, Vector3.up) * transform.forward, 
            m_interactionDegree, m_interactionRange);
    }

    private bool IsVisible(Interactable interactable) {
        if (DistanceTo(interactable) > interactable.GetInteractionRange(m_interactionRange)) {
            return false;
        }

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(position, (interactable.position - position).normalized, 
            out hitInfo, interactable.GetInteractionRange(m_interactionRange), m_interactionBlockerMask);

        return hit && hitInfo.collider.GetComponent<Interactable>() == interactable;
    }

    private float DistanceTo(Interactable interactable) {
        return (interactable.position - position).magnitude;
    }

    Interactable FindMinInteractable() {
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRange, m_interactableLayerMask);

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
            Debug.DrawLine(position, m_curInteractable.transform.position, Color.red);
        }
    }

    public void InteractionRequest() {
        if (m_curInteractable == null) {
            return;
        }

        m_curInteractable.Interact(this);
    }

    private float DegreeTo(GameObject go) {
        Vector3 otherPos = go.transform.position + go.GetComponent<Interactable>().positionOffset;

        Vector3 delta = Vector3.ProjectOnPlane(otherPos - position, Vector3.up);

        return Vector3.Angle(transform.forward, delta);
    }
}