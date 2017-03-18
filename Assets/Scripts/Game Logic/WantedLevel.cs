using UnityEngine;

public class WantedLevel : Singleton<WantedLevel>
{
    // Current global wanted level
    private float m_currentWantedLevel;
    // Current global alert tier (0-3)
    private int m_currentAlertTier;
    [SerializeField]
    [Tooltip("How much the wanted level increases if player is seen by 1 guard for 1 second")]
    private float m_wantedDelta;

    // Is calculated from critical vigilant guard count and locked wanted level
    private float m_minWantedLvlAlert1;
    [SerializeField]
    [Tooltip("Wanted level required to start off alert tier 2")]
    private float m_minWantedLvlAlert2;
    [SerializeField]
    [Tooltip("Wanted level required to start off alert tier 3")]
    private float m_minWantedLvlAlert3;

    // How many guards are currently vigilant
    private int m_currentVigilantGuardCount;
    [SerializeField]
    [Tooltip("Amount of vigilant guards that trigger alert tier 1")]
    private int m_criticalVigilantGuardCount;
    [SerializeField]
    [Tooltip("How much wanted level gets locked in stage 0 by a vigilant guard")]
    private float m_vigilantGuardLockedWantedLevel;

    // If the player was not seen by any guard since last update
    private bool m_playerIsNotInGuardSight;
    [SerializeField]
    [Tooltip("How long the wanted level does not decrease after last guard lost sight on the player")]
    private float m_maxStagnationTime;
    // How long the wanted level is already stagnating since guards lost sight
    private float m_currentStagnationTime;
    // If the guards can't see the player and wanted level is still stagnating
    private bool m_wantedLevelIsStagnating;


    public int CurrentWantedStage {
        /* returns a value between 0 and 1 for the current wanted stage */
        get { return m_currentAlertTier;  }
    }

    private float CurrentWantedLimit {
        get {
            if (m_currentAlertTier == 0) return m_minWantedLvlAlert1;
            if (m_currentAlertTier == 1) return m_minWantedLvlAlert2;
            if (m_currentAlertTier == 2) return m_minWantedLvlAlert3;
            return 1;
        }
    }

    public float PercentCurrentWantedLevel {
        /* returns a value between 0 and 1 for the current wanted stage */
        get { return m_currentWantedLevel / CurrentWantedLimit; }
    }

    private void Start()
    {
        // Minimum wanted level for alert 1 depends on vigilant guards
        m_minWantedLvlAlert1 = m_vigilantGuardLockedWantedLevel * m_criticalVigilantGuardCount;

        // At beginning of the game no guard should see the player
        m_playerIsNotInGuardSight = true;
    }

    private void OnValidate()
    {
        //TODO Check if remaining values are valid
        if (m_wantedDelta < 0f)
        {
            m_wantedDelta = 0f;
            Debug.LogError("WantedDelta must be a positive value");
        }
        if (m_maxStagnationTime < 0f)
        {
            m_maxStagnationTime = 0f;
            Debug.LogError("MaxStagnationTime must be a positive value");
        }
    }

    private void Update()
    {
        // Decrease in wanted level is calculated in LateUpdate() to ensure that the AI is calculated before
    }

    public void RaiseWantedLevel()
    {
        m_playerIsNotInGuardSight = false;
        m_currentWantedLevel += Time.deltaTime * m_wantedDelta;

        // Check if wanted level is high enough to activate next alert tier
        if (m_currentWantedLevel >= m_minWantedLvlAlert3)
            SetAlertTier(3);
        else if (m_currentWantedLevel >= m_minWantedLvlAlert2)
            SetAlertTier(2);
        else if (m_currentWantedLevel >= m_minWantedLvlAlert1)
            SetAlertTier(1);
    }

    private void LowerWantedLevel(float changeInWantedLevel)
    {
        m_currentWantedLevel -= changeInWantedLevel;

        // Wanted level can not decrease below current alert tier minimum (inlcuding locked wanted level)
        if (m_currentAlertTier == 0 && m_currentWantedLevel < m_currentVigilantGuardCount * m_vigilantGuardLockedWantedLevel)
        {
            m_currentWantedLevel = m_currentVigilantGuardCount * m_vigilantGuardLockedWantedLevel;
        }
        else if (m_currentAlertTier == 1 && m_currentWantedLevel < m_minWantedLvlAlert1)
        {
            m_currentWantedLevel = m_minWantedLvlAlert1;
        }
        else if (m_currentAlertTier == 2 && m_currentWantedLevel < m_minWantedLvlAlert2)
        {
            m_currentWantedLevel = m_minWantedLvlAlert2;
        }
    }

    /*
     * NOTE
     * In the current design of the wanted level it is not possible
     * to lower the alert tier. Just in case it will be decided to be
     * possible the deprecated code was not deleted     * 
     */
    private void SetAlertTier(int tier)
    {
        // Check if alarm was lowered or raised
        int tierChange = tier - m_currentAlertTier;

        m_currentAlertTier = Mathf.Clamp(tier, 0, 3);

        // Alert tier stage was raised
        if (tierChange > 0)
        {
            if (m_currentAlertTier == 1)
            {
                //TODO AI & UI
            }
            else if (m_currentAlertTier == 2)
            {
                //TODO AI & UI
            }
            // Game Over
            else if (m_currentAlertTier == 3)
            {
                //TODO End the game
            }
        }
        // Alert tier stage was lowered
        else if (tierChange < 0)
        {
            //TODO AI & UI
        }

        // If alert tier hasn't changed at all no action is necessary
    }

    // Tell the WantedLevel about a new guard becoming vigilant
    // Returns true, if critical amount of vigilant guards has been reached
    public bool GuardIsNowVigilant()
    {
        // Vigilant guard count only considered in alert stage 0
        if (m_currentAlertTier == 0)
        {
            // Vigilant guard locks a part of alert stage 0
            m_currentVigilantGuardCount++;

            // Vigilant guard increases the current wanted level
            m_currentWantedLevel += m_vigilantGuardLockedWantedLevel;

            if (m_currentWantedLevel >= m_minWantedLvlAlert1)
            {
                SetAlertTier(1);
                return true;
            }
        }
        return false;
    }

    private void LateUpdate()
    {
        // Wanted level decreases over time if player is not in sight of guards
        if (m_playerIsNotInGuardSight)
        {
            // Guards recently lost sight on the player or level recently started
            if (m_wantedLevelIsStagnating)
            {
                m_currentStagnationTime += Time.deltaTime;
                // Guards lost sight long enough to decrease wanted level from now on
                if (m_currentStagnationTime > m_maxStagnationTime)
                {
                    m_wantedLevelIsStagnating = false;
                    // Time that exceeds maxStagnationTime is used to calculate the wanted level decrease
                    LowerWantedLevel(m_wantedDelta * (m_currentStagnationTime - m_maxStagnationTime));

                    // Reset stagnation time
                    m_currentStagnationTime = 0f;

                    //TODO UI (e.g. blinking wanted level)
                }
            }
            // Guards have not seen player for a while
            else
                LowerWantedLevel(m_wantedDelta * Time.deltaTime);
        }
        // Player was seen by guards in the last AI update and wanted level stagnates for short time period
        else
            m_wantedLevelIsStagnating = true;

        // Reset flag to lower wanted level if no guard has seen the player
        m_playerIsNotInGuardSight = true;
    }

    public void TriggerLaserAlert()
    {
        // Laser alert raise the current alert to tier 2
        if(m_currentAlertTier < 2)
        {
            m_currentAlertTier = 2;
            m_currentWantedLevel = m_minWantedLvlAlert2;

            //TODO AI & UI
            //Guards will switch to vigilant state
        }
    }
}
