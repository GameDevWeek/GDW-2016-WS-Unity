using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyInteractable : Interactable {

    public struct StunEventData {
        public GameObject stunner, stunned;

        public StunEventData(GameObject stunner, GameObject stunned) {
            this.stunner = stunner;
            this.stunned = stunned;
        }
    }

    public delegate void StunEvent(StunEventData data);
    public static event StunEvent OnStun;

    [Range(0, 10)]
    public float Distance = 1.0f;

    public float HighOffeset = 0.0f;

    [TagSelector]
    public string TagToBeHitted;

    [SerializeField]
    private Cooldown m_cooldown = new Cooldown(0.5f);
    private Vector3 m_vectorOffset;

    [SerializeField]
    private float m_backAngle = 30.0f;

    private void OnValidate() {
        m_vectorOffset = new Vector3(0, HighOffeset, 0);
    }

    void Update() {
        m_cooldown.Update(Time.deltaTime);
    }

    [Obsolete]
    private bool StunEnemyInFront() {
        RaycastHit info = new RaycastHit();
        Ray myRay = new Ray(transform.position + m_vectorOffset, transform.forward);
        Physics.SphereCast(myRay, 0.25f, out info, Distance);

        if (info.rigidbody != null && info.rigidbody.tag == TagToBeHitted) {
            if (Vector3.Dot(info.rigidbody.transform.forward, transform.forward) > 0) //Check if the Object in Front of this one, is directed with the back to this. 
            {
                if (OnStun != null) {
                    OnStun(new StunEventData(this.gameObject, info.rigidbody.gameObject));
                }
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    //private void OnDrawGizmos() {
    //    UnityEditor.Handles.Label(transform.position + transform.forward * Distance + m_vectorOffset, "StunEnemy | CoolDown:" + ((int)(m_cooldown.timeLeftInSeconds * 100)) / 100.0f);
    //    UnityEditor.Handles.color = Color.green;
    //    UnityEditor.Handles.DrawLine(transform.position + m_vectorOffset, transform.position + transform.forward * Distance + m_vectorOffset);
    //}

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        Handles.DrawSolidArc(position, Vector3.up,
            Quaternion.AngleAxis(-m_backAngle * 0.5f, Vector3.up) * -transform.forward,
            m_backAngle, m_interactionRange);
    }
#endif

    public override void Interact(Interactor interactor) {
        if (!IsStunPossible(interactor)) {
            return;
        }

        if (OnStun != null) {
            OnStun(new StunEventData(this.gameObject, interactor.gameObject));
        }

        Debug.Log("Stun");
        // TODO: Stun logic
    }

    public bool IsStunPossible(Interactor stunner) {
        return Vector3.Angle(stunner.transform.forward, transform.forward) < m_backAngle &&
            m_cooldown.IsOver();
    }
}
