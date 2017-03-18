using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour {
    public bool interactionActive = true;

    [SerializeField]
    private Vector3 m_positionOffset = new Vector3();
    [SerializeField]
    private bool m_useDefaultInteractionRange;
    [SerializeField]
    protected float m_interactionRange = 3.0f;

    [Tooltip("Line of sight and view angle of the interactor will be ignored.")]
    public bool useInteractionRangeOnly = false;

    public bool ignoreRaycastVisibility = false;

    public Vector3 positionOffset {
        get { return m_positionOffset; }
        set { m_positionOffset = value; }
    }

    public float GetInteractionRange(float defaultRange) {
        return m_useDefaultInteractionRange ? defaultRange : m_interactionRange;
    }

    public Vector3 position {
        get {
            return m_positionOffset + transform.position;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + positionOffset, 0.1f);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.3f);
        Gizmos.DrawSphere(position, m_interactionRange);
    }

    public abstract void Interact(Interactor interactor);
}
