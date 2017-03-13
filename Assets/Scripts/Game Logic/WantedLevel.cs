using UnityEngine;

public class WantedLevel : MonoBehaviour {

    /* 
     * Maximum cumulative times/wantedLevel allowed in alert stages. 
     * If this time/wantedLevel is exceed, the next stage or game over will be initialized
    */
    [SerializeField]
    [Header("Alert Wanted Levels")]
    [Tooltip("Minimum attention required to start Alert 1")]
    private float m_minAttentionAlarm1;
    [SerializeField]
    [Tooltip("Minimum attention required to start Alert 2")]
    private float m_minAttentionAlarm2;
    [SerializeField]
    [Tooltip("Minimum attention required to start Alert 3")]
    private float m_minAttentionAlarm3;

    // How much the player is wanted
    private float m_currentAttention;
    // Current stage of 4 possible (stage 0 = no alarm; stage 3 = GameOver)
    private int m_currentAlertStage;
    // How much the attention is altered withing 1 second by 1 guard
    private float m_attentionDelta;
    // How many guards can see the player
    private int m_amountOfGuardsSeeingMe;
    
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		if(IsInSightOfEnemy())
        {
            m_currentAttention += m_amountOfGuardsSeeingMe * m_attentionDelta * Time.deltaTime;

            if (m_currentAlertStage == 0 && m_currentAttention >= m_minAttentionAlarm1)
                RaiseAlarmToStage(1);
            else if (m_currentAlertStage == 1 && m_currentAttention >= m_minAttentionAlarm2)
                RaiseAlarmToStage(2);
            else if (m_currentAlertStage == 2 && m_currentAttention >= m_minAttentionAlarm3)
                RaiseAlarmToStage(3);
        }
        // Not in sight of an enemy
        else
        {
            m_currentAttention -= m_attentionDelta * Time.deltaTime;

            if (m_currentAlertStage == 1 && m_currentAttention < m_minAttentionAlarm1)
                LowerAlarmToStage(0);
            else if (m_currentAlertStage == 2 && m_currentAttention < m_minAttentionAlarm2)
                LowerAlarmToStage(1);
            // only possible if AlertStage3 is not instant GameOver
            else if (m_currentAlertStage == 3 && m_currentAttention < m_minAttentionAlarm3)
                LowerAlarmToStage(2);
            
            // minimum attention of 0
            if (m_currentAttention < 0f)
            {
                m_currentAttention = 0f;
            }
        }
	}

    private void RaiseAlarmToStage(int stage)
    {
        //Can raise to stage 1-3
        if (stage < 1 || stage > 3)
            return;

        m_currentAlertStage = stage;

        if(m_currentAlertStage == 3)
        {
            //TODO GameOver
        }

        //TODO AI & UI
    }

    private void LowerAlarmToStage(int stage)
    {
        //Can lower to stage 0-2
        if (stage < 0 || stage > 2)
            return;
        m_currentAlertStage = stage;

        //TODO AI & UI
    }

    private bool IsInSightOfEnemy()
    {
        return m_amountOfGuardsSeeingMe > 0;
    }

    //TODO Check if the guard has already altered the amount
    public void PlayerIsInSightOfGuard(bool isInSight)
    {
        if (isInSight)
            m_amountOfGuardsSeeingMe++;
        else
        {
            m_amountOfGuardsSeeingMe--;
            if (m_amountOfGuardsSeeingMe < 0)
                m_amountOfGuardsSeeingMe = 0;
        }
    }
}
