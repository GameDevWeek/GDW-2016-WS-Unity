using System;
using UnityEngine;

public class WantedLevel : Singleton<WantedLevel>
{

    [SerializeField]
    [Tooltip("Minimum attention required to start Alert 1")]
    private float m_minAttentionAlarm1;
    [SerializeField]
    [Tooltip("Minimum attention required to start Alert 2")]
    private float m_minAttentionAlarm2;
    [SerializeField]
    [Tooltip("Minimum attention required to start Alert 3")]
    private float m_minAttentionAlarm3;

    // How much attention the player has caused
    private float m_currentAttention;
    // Current alert stage of 4 possible (stage 0 = no alarm; stage 3 = GameOver)
    private int m_currentAlertStage;
    [SerializeField]
    [Tooltip("How much attention is caused by 1 guard seeing the player in 1 second")]
    private float m_attentionDelta;

    // If the player was not seen by any guard since last update
    private bool m_playerIsNotInGuardSight;
    [SerializeField]
    [Tooltip("How long the attention does not decrease after last guard lost sight on the player")]
    private float m_maxStagnationTime;
    // How long the attention is already stagnating since guards lost sight
    private float m_currentStagnationTime;
    // If the guards can't see the player and attention is still stagnating
    private bool m_attentionIsStagnating;

    private float lastMax = -1;
    public static event Action<float> newMaxLevel;

    // Use this for initialization
    void Start()
    {
        // At beginning of the game no guard should see the player
        m_playerIsNotInGuardSight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Attention decreases over time if player not in sight
        if (m_playerIsNotInGuardSight)
        {
            // Guards recently lost sight on the player
            if (m_attentionIsStagnating)
            {
                m_currentStagnationTime += Time.deltaTime;
                // Guards lost sight long enough to decrease attention from now on
                if (m_currentStagnationTime > m_maxStagnationTime)
                {
                    m_attentionIsStagnating = false;
                    // Time that exceeds maxStagnationTime is used to calculate the attention decrease
                    m_currentAttention -= m_attentionDelta * (m_currentStagnationTime - m_maxStagnationTime);

                    // Reset stagnation time
                    m_currentStagnationTime = 0f;

                    //TODO UI (e.g. blinking wanted level)
                }
            }
            // Guards have not seen player for a while
            else
            {
                m_currentAttention -= m_attentionDelta * Time.deltaTime;

                if (m_currentAlertStage == 1 && m_currentAttention < m_minAttentionAlarm1)
                    SetAlarmStage(1);
                else if (m_currentAlertStage == 2 && m_currentAttention < m_minAttentionAlarm2)
                    SetAlarmStage(2);
                // only required if AlertStage3 should not result in instant GameOver
                else if (m_currentAlertStage == 3 && m_currentAttention < m_minAttentionAlarm3)
                    SetAlarmStage(3);

                // minimum attention of 0
                if (m_currentAttention < 0f)
                    m_currentAttention = 0f;
            }
        }
    }

    private void SetAlarmStage(int stage)
    {
        // check if alarm was lowered or raised
        int alarmChange = stage - m_currentAlertStage;

        m_currentAlertStage = stage;
        m_currentAlertStage = Mathf.Clamp(stage, 0, 3);

        // alarm stage was raised
        if (alarmChange > 0)
        {
            //TODO AI & UI

            // Game Over
            if (m_currentAlertStage == 3)
            {

            }
        }
        // alarm stage was lowered
        else if (alarmChange < 0)
        {
            //TODO AI & UI
        }

        // if alarm hasn't changed at all no action is necessary
    }

    public void RaiseAttentionOnPlayer()
    {
        m_playerIsNotInGuardSight = false;
        m_currentAttention += Time.deltaTime * m_attentionDelta;

        if(lastMax < m_currentAttention) {
            lastMax = m_currentAttention;
            if (newMaxLevel != null) newMaxLevel.Invoke(lastMax);
        }



        // Check if attention is high enough to activate next alert stage
        if (m_currentAttention >= m_minAttentionAlarm3)
            SetAlarmStage(3);
        else if (m_currentAttention >= m_minAttentionAlarm2)
            SetAlarmStage(2);
        else if (m_currentAttention >= m_minAttentionAlarm1)
            SetAlarmStage(1);
    }

    private void LateUpdate()
    {
        // Player escaped guards sight and the attention stagnates
        if (!m_playerIsNotInGuardSight)
            m_attentionIsStagnating = true;

        // Reset flag to lower attention if no guard has seen the player
        m_playerIsNotInGuardSight = true;
    }
}
