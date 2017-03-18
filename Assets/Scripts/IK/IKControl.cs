using UnityEngine;
using System.Collections;

public class IKControl : MonoBehaviour
{
    [SerializeField]
    public bool ikActive = false;

    [SerializeField]
    public Transform rightHandObj = null;

    [SerializeField]
    public Transform leftHandObj = null;

    [SerializeField]
    public Transform lookObj = null;

    private Animator m_animator;

    public float lerpTimeLeftHand;
    public float lerpTimeRightHand;
    public float lerpTimeLookAt;

    private Vector3 m_curRightHandPos;
    private Vector3 m_curLeftHandPos;
    private Vector3 m_curLookAtPos;

    public float lerpTimeMultiplierLeftHand = 1.0f;
    public float lerpTimeMultiplierRightHand = 1.0f;
    public float lerpTimeMultiplierLookAt = 1.0f;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_curLookAtPos = m_animator.GetBoneTransform(HumanBodyBones.Head).localPosition;
        m_curRightHandPos = m_animator.GetBoneTransform(HumanBodyBones.RightHand).localPosition;
        m_curLeftHandPos = m_animator.GetBoneTransform(HumanBodyBones.LeftHand).localPosition;
    }

    void OnAnimatorIK()
    {
        if (!m_animator)
            return;

        if (ikActive)
        {
            m_animator.applyRootMotion = false;

            if (lookObj != null)
            {
                lerpTimeLookAt += Time.deltaTime * lerpTimeMultiplierLookAt;
                m_animator.SetLookAtWeight(1);
                Vector3 localPos = transform.InverseTransformPoint(lookObj.position);
                m_curLookAtPos = Vector3.Lerp(m_curLookAtPos, localPos, lerpTimeLookAt);
                m_animator.SetLookAtPosition(transform.TransformPoint(m_curLookAtPos));
            }
            else
            {
                m_curLookAtPos = m_animator.GetBoneTransform(HumanBodyBones.Head).localPosition;
            }

            float weight = 1.0f;
            if (rightHandObj != null)
            {
                lerpTimeRightHand += Time.deltaTime * lerpTimeMultiplierRightHand;
                m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
                Vector3 localPos = transform.InverseTransformPoint(rightHandObj.position);
                m_curRightHandPos = Vector3.Lerp(m_curRightHandPos, localPos, lerpTimeRightHand);
                m_animator.SetIKPosition(AvatarIKGoal.RightHand, transform.TransformPoint(m_curRightHandPos));
                m_animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
            else
            {
                m_curRightHandPos = m_animator.GetBoneTransform(HumanBodyBones.RightHand).localPosition;
            }

            if (leftHandObj != null)
            {
                lerpTimeLeftHand += Time.deltaTime * lerpTimeMultiplierLeftHand;
                m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                Vector3 localPos = transform.InverseTransformPoint(leftHandObj.position);
                m_curLeftHandPos = Vector3.Lerp(m_curLeftHandPos, localPos, lerpTimeLeftHand);
                m_animator.SetIKPosition(AvatarIKGoal.LeftHand, transform.TransformPoint(m_curLeftHandPos));
                m_animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }
            else
            {
                m_curLeftHandPos = m_animator.GetBoneTransform(HumanBodyBones.LeftHand).localPosition;
            }
        }
        else
        {
            // Reset weights
            m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            m_animator.SetLookAtWeight(0);
        }
    }
}
