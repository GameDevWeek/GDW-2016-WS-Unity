using UnityEngine;
using System.Collections;

// Based on Unity Standard Assets AbstractTargetFollower
public abstract class AbstractCamera : MonoBehaviour {
    // The available methods of updating are:
    public enum UpdateType {
        FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
        LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
        ManualUpdate, // user must call to update camera
    }

    [SerializeField]
    protected Transform m_target;            // The target object to follow

    [SerializeField]
    private bool m_autoTargetPlayer = true;  // Whether the rig should automatically target the player.

    [SerializeField]
    protected UpdateType m_updateType;         // stores the selected update type

    protected Rigidbody targetRigidbody;


    protected virtual void Start() {
        // if auto targeting is used, find the object tagged "Player"
        // any class inheriting from this should call base.Start() to perform this action!
        if (m_autoTargetPlayer && m_target == null) {
            FindAndTargetPlayer();
        }

        if (m_target == null)
            return;

        targetRigidbody = m_target.GetComponent<Rigidbody>();
    }


    private void FixedUpdate() {
        // we update from here if updatetype is set to Fixed, or in auto mode,
        // if the target has a rigidbody, and isn't kinematic.
        if (m_autoTargetPlayer && (m_target == null || !m_target.gameObject.activeSelf)) {
            FindAndTargetPlayer();
        }
        if (m_updateType == UpdateType.FixedUpdate) {
            FollowTarget(Time.deltaTime);
        }
    }


    private void LateUpdate() {
        // we update from here if updatetype is set to Late, or in auto mode,
        // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
        if (m_autoTargetPlayer && (m_target == null || !m_target.gameObject.activeSelf)) {
            FindAndTargetPlayer();
        }
        if (m_updateType == UpdateType.LateUpdate) {
            FollowTarget(Time.deltaTime);
        }
    }


    public void ManualUpdate() {
        // we update from here if updatetype is set to Late, or in auto mode,
        // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
        if (m_autoTargetPlayer && (m_target == null || !m_target.gameObject.activeSelf)) {
            FindAndTargetPlayer();
        }
        if (m_updateType == UpdateType.ManualUpdate) {
            FollowTarget(Time.deltaTime);
        }
    }

    protected abstract void FollowTarget(float deltaTime);

    public void FindAndTargetPlayer() {
        // auto target an object tagged player, if no target has been assigned
        var targetObj = GameObject.FindGameObjectWithTag("Player");
        if (targetObj) {
            SetTarget(targetObj.transform);
        }
    }


    public virtual void SetTarget(Transform newTransform) {
        m_target = newTransform;
    }


    public Transform Target {
        get { return m_target; }
    }
}
