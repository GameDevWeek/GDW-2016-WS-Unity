using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(ElephantMovement))]
public class TrunkControll : MonoBehaviour {
    [SerializeField]
    private bool m_controllWithSlider = false;

    [SerializeField, Range(0,1)]
    private float m_trunkStiffness = 1;

    [SerializeField]
    private Animator m_animatorForTrunk;

    private Animator m_animator;
    private ElephantMovement m_elefantmovement;

    private float m_value = 0;

    [SerializeField]
    private float m_damping = 1.0f;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_elefantmovement = GetComponent<ElephantMovement>();
    }
	
	// Update is called once per frame
	void Update () {

        m_value -= m_damping * Time.deltaTime;
        m_value += Mathf.Abs(m_elefantmovement.GetRotationSpeed()) / 45.0f;
        m_value = Mathf.Clamp(m_value, 0, 2);

        if (!m_controllWithSlider)
        {
            m_animatorForTrunk.SetFloat("TrunkStiffness", m_animator.GetFloat("Forward") + m_value);
        }else
        {
            m_animatorForTrunk.SetFloat("TrunkStiffness", m_trunkStiffness);
        }

    }
}
