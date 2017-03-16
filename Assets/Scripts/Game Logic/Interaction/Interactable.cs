using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour {
    public bool interactionActive = true;

    [SerializeField]
    private Vector3 m_positionOffset = new Vector3();

    public Vector3 positionOffset {
        get { return m_positionOffset; }
        set { m_positionOffset = value; }
    }

    public Vector3 position {
        get {
            return m_positionOffset + transform.position;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + positionOffset, 0.1f);
    }

    public abstract void Interact(Interactor interactor);
}
