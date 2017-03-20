using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField]
    private bool m_use2DDistance = true;
    [SerializeField]
    private SpriteRenderer interactionIconPrefab;

    private SpriteRenderer interactionIcon;

    public Vector3 position {
        get {
            return transform.position + m_positionOffset;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        if(! this.enabled) return;
        Handles.color = new Color(1.0f, 0.0f, 1.0f, 0.3f);
        Handles.DrawSolidArc(position, Vector3.up, 
            Quaternion.AngleAxis(-m_interactionDegree * 0.5f, Vector3.up) * transform.forward, 
            m_interactionDegree, m_interactionRange);
    }
#endif

    private bool IsVisible(Interactable interactable) {
        if (!InRange(interactable)) {
            return false;
        }

        if (interactable.ignoreRaycastVisibility) {
            return true;
        }

        float distance = DistanceTo(interactable);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(position, (interactable.position - position).normalized, 
            out hitInfo, interactable.GetInteractionRange(m_interactionRange), m_interactionBlockerMask);

        return !hit ||
            (hitInfo.collider.GetComponent<Interactable>() == interactable || 
             hitInfo.distance >= distance);
    }

    private float DistanceTo(Interactable interactable) {
        if (m_use2DDistance) {
            return (Vector3.ProjectOnPlane(interactable.position, Vector3.up) - 
                Vector3.ProjectOnPlane(position, Vector3.up)).magnitude;
            
        }

        return (interactable.position - position).magnitude;
    }

    private bool InRange(Interactable interactable) {
        return DistanceTo(interactable) <= interactable.GetInteractionRange(m_interactionRange);
    }

    Interactable FindMinInteractable() {
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRange, m_interactableLayerMask);

        float minDegree = float.MaxValue;
        GameObject minInteractable = null;

        // Find closest match (smallest degree to interactable)
        foreach (Collider collider in colliders) {
            var interactable = collider.GetComponent<Interactable>();
            if (interactable == null || !collider.gameObject || !interactable.CanInteract(this)) {
                continue;
            }
            
            if (interactable.useInteractionRangeOnly) {
                if (InRange(interactable)) {
                    return interactable;
                }
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

    private void LateUpdate()
    {
        UpdateInteractionIcon();
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

    private void UpdateInteractionIcon()
    {
        if (m_curInteractable)
        {
            if (!interactionIcon)
            {
                interactionIcon = Instantiate(interactionIconPrefab.gameObject).GetComponent<SpriteRenderer>();
            }
            interactionIcon.transform.position = Camera.main.transform.position + ((m_curInteractable.position + new Vector3(0.9f, 0, 0.9f)) - Camera.main.transform.position).normalized * 1f;
            interactionIcon.sprite = m_curInteractable.Icon;
            interactionIcon.transform.localScale = interactionIconPrefab.transform.localScale * m_curInteractable.IconScale;
        }
        else
        {
            if (interactionIcon)
            {
                Destroy(interactionIcon.gameObject);
                interactionIcon = null;
            }
        }
    }
}
