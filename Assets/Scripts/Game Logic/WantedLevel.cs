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
                RaiseAlarmToStage1();
            else if (m_currentAlertStage == 1 && m_currentAttention >= m_minAttentionAlarm2)
                RaiseAlarmToStage2();
            else if (m_currentAlertStage == 2 && m_currentAttention >= m_minAttentionAlarm3)
                RaiseAlarmToStage3();
        }
        // Not in sight of an enemy
        else
        {
            m_currentAttention -= m_attentionDelta * Time.deltaTime;

            if (m_currentAlertStage == 1 && m_currentAttention < m_minAttentionAlarm1)
                LowerAlarmToStage0();
            else if (m_currentAlertStage == 2 && m_currentAttention < m_minAttentionAlarm2)
                LowerAlarmToStage1();
            // only possible if AlertStage3 is not instant GameOver
            else if (m_currentAlertStage == 3 && m_currentAttention < m_minAttentionAlarm3)
                LowerAlarmToStage2();
            
            // minimum attention of 0
            if (m_currentAttention < 0f)
            {
                m_currentAttention = 0f;
            }
        }
	}

    private void RaiseAlarmToStage1()
    {
        m_currentAlertStage = 1;
        //TODO AI&UI
    }

    private void RaiseAlarmToStage2()
    {
        m_currentAlertStage = 2;
        //TODO AI&UI
    }

    private void RaiseAlarmToStage3()
    {
        m_currentAlertStage = 3;
        //TODO AI&UI

        //TODO GameOver
    }

    private void LowerAlarmToStage0()
    {
        m_currentAlertStage = 0;
        //TODO AI&UI
    }

    private void LowerAlarmToStage1()
    {
        m_currentAlertStage = 0;
        //TODO AI&UI
    }

    private void LowerAlarmToStage2()
    {
        m_currentAlertStage = 0;
        //TODO AI&UI
    }

    private bool IsInSightOfEnemy()
    {
        return m_amountOfGuardsSeeingMe > 0;
    }

    //TODO Check if the guard has already increased the amount
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
