using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cooldown {
    private float m_timeLeftInSeconds = 0.0f;
    [SerializeField]
    private float m_timeInSeconds = 0.0f;

    public float timeInSeconds {
        get { return m_timeInSeconds; }
        set { m_timeInSeconds = value; }
    }

    public float timeLeftInSeconds {
        get { return m_timeLeftInSeconds; }
    }

    public Cooldown() {

    }

    public Cooldown(float timeInSeconds) {
        m_timeInSeconds = timeInSeconds;
        m_timeLeftInSeconds = m_timeInSeconds;
    }

    /**
     * <summary>Sets the cooldown to 0.</summary>
     */
    public void End() {
        m_timeLeftInSeconds = 0.0f;
    }

    public void Start() {
        m_timeLeftInSeconds = m_timeInSeconds;
    }

    public void Update(float deltaTime) {
        if (m_timeLeftInSeconds > 0.0f) {
            m_timeLeftInSeconds -= deltaTime;
        }
    }

    public bool IsOver() {
        return m_timeLeftInSeconds <= 0.0f;
    }
}
