﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageController : MonoBehaviour
{
    public struct ShockEventData
    {
        public List<GameObject> MiceInRange;

        public ShockEventData(List<GameObject> miceInRange)
        {
            MiceInRange = miceInRange;
        }
    }

    /// <summary>
    /// Event invoked if elephant is shocked because of mice in range.
    /// </summary>
    public static event ShockEvent OnElephantShocked;
    public delegate void ShockEvent(ShockEventData e);
    /// <summary>
    /// Event invoked if elephant is falling from pedestal if max time is over.
    /// </summary>
    public static event FallFromPedestalEvent OnElephantFallFromPedestal;
    public delegate void FallFromPedestalEvent();

    /// <summary>
    /// Event invoked if camouflage mode was entered.
    /// </summary>
    public static event EntersCamouflageModeEvent OnElephantEntersCamouflageMode;
    public delegate void EntersCamouflageModeEvent();
    /// <summary>
    /// Event invoked if camouflage mode exits (will be invoked every time and before OnElephantShocked/OnElephantFallFromPedestal is invoked).
    /// </summary>
    public static event ExitsCamouflageModeEvent OnElephantExitsCamouflageMode;
    public delegate void ExitsCamouflageModeEvent();

    public delegate void StunnedCooldownOver();
    public static event StunnedCooldownOver OnStunnedCooldownOver;

    [SerializeField]
    private string _mouseTag = GameTag.Mouse;
    [SerializeField]
    private int _mouseReactDistance = 5;
    [SerializeField]
    [Range(0, 20000)]
    [Tooltip("Milliseconds: OnElephantShock cooldown.")]
    private int _shockedCooldownMS = 10000;
    [SerializeField]
    [Tooltip("Offset from transform.position to calculate mouse in range raycast.")]
    private Vector3 _elephantEyeOffset = Vector3.up * 0.5f;
    [SerializeField]
    [Range(0, 20000)]
    [Tooltip("Milliseconds: Maximum duration in ms for player in camouflage mode. If time is exceeded elephant will fall from pedestal.")]
    private int _camouflageMaxDurationMS = 7000;

    private int milliseconds_left = 0;

    private Coroutine _camouflageTimeExceededChecker;

    private Cooldown _shockCooldown;
    private List<GameObject> _enemiesInRange;

    public bool CamouflageModeActive { get; private set; }

    [SerializeField]
    private Cooldown m_stunnedCooldown = new Cooldown(2.0f);
    private bool m_stunned = false;

    public float PercentTimeLeft {
        get { return (milliseconds_left / (float)_camouflageMaxDurationMS); }
    }

    public Cooldown stunnedCooldown {
        get {
            return m_stunnedCooldown;
        }
    }

    private void Awake()
    {
        this._enemiesInRange = new List<GameObject>();
        this._shockCooldown = new Cooldown();
        this.CamouflageModeActive = false;
        m_stunnedCooldown.Start();
    }

    private void Update()
    {
        if (m_stunned) {
            m_stunnedCooldown.Update(Time.deltaTime);

            if (m_stunnedCooldown.IsOver() && OnStunnedCooldownOver != null) {
                OnStunnedCooldownOver();
                m_stunned = false;
                m_stunnedCooldown.Start();
            }
        }

        if (!_shockCooldown.IsOver())
        {
            _shockCooldown.Update(Time.deltaTime);
            if (!CamouflageModeActive)
            {
                return;
            }
        }

        List<GameObject> miceInRange;
        if (IsMouseInRange(out miceInRange))
        {
            if (CamouflageModeActive)
            {
                ExitCamouflageMode();
            }
            Debug.Log("OnElephantShocked()");
            if (OnElephantShocked != null)
            {
                OnElephantShocked(new ShockEventData(miceInRange));
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f,1.0f,0.0f,0.2f);
        Gizmos.DrawSphere(transform.position + _elephantEyeOffset, _mouseReactDistance);
    }
#endif

    public void EnemyInRange(GameObject enemy)
    {
        if (!_enemiesInRange.Contains(enemy))
        {
            _enemiesInRange.Add(enemy);
        }
    }

    public void EnemyOutOfRange(GameObject enemy)
    {
        if (_enemiesInRange.Contains(enemy))
        {
            _enemiesInRange.Remove(enemy);
        }
    }

    /// <summary>
    /// Toggles camouflage mode. Enables camouflage mode only if now possible.
    /// </summary>
    /// <returns>true if camouflage mode is now active</returns>
    public bool ToggleCamouflageMode()
    {
        if (CamouflageModeActive)
        {
            ExitCamouflageMode();
        }
        else
        {
            TryEnterCamouflageMode();
        }
        return CamouflageModeActive;
    }

    /// <summary>
    /// Tries to enter camouflage mode if camouflage mode is possible now.
    /// </summary>
    /// <returns>true if mode could be applied</returns>
    public bool TryEnterCamouflageMode()
    {
        if (!CamouflagePossible()) return false;

        Debug.Log("OnElephantEntersCamouflageMode");
        if (OnElephantEntersCamouflageMode != null)
        {
            OnElephantEntersCamouflageMode();
        }

        _camouflageTimeExceededChecker = StartCoroutine(CamouflageTimeExceededChecker());

        return CamouflageModeActive = true;
    }

    /// <summary>
    /// Exits camouflage mode without any checks. (Never invokes OnElephantShocked/OnElephantFallFromPedestal) 
    /// </summary>
    public void ExitCamouflageMode()
    {
        ExitCamouflageMode(false);
    }

    private void ExitCamouflageMode(bool calledFromCoroutine)
    {
        if (!calledFromCoroutine) StopCoroutine(_camouflageTimeExceededChecker);

        Debug.Log("OnElephantExitsCamouflageMode");
        if (OnElephantExitsCamouflageMode != null)
        {
            OnElephantExitsCamouflageMode();
        }

        CamouflageModeActive = false;
    }

    private bool CamouflagePossible()
    {
        if (!IsMouseInRange()
            && _enemiesInRange.Count == 0)
        {
            return true;
        }

        return false;
    }

    private bool IsMouseInRange()
    {
        List<GameObject> miceInRange;
        return IsMouseInRange(out miceInRange);
    }

    private bool IsMouseInRange(out List<GameObject> miceInRange)
    {
        miceInRange = new List<GameObject>();

        bool result = false;
        Collider[] hits = Physics.OverlapSphere(transform.position + _elephantEyeOffset, _mouseReactDistance);
        foreach (Collider coll in hits)
        {
            if (coll.gameObject.CompareTag(_mouseTag))
            {
                RaycastHit hit;
                Vector3 direction = coll.gameObject.transform.position - (transform.position + _elephantEyeOffset);
                bool hitSomething = Physics.Raycast(transform.position + _elephantEyeOffset, direction, out hit, _mouseReactDistance);
                if (hitSomething && hit.collider.gameObject.CompareTag(_mouseTag))
                {
#if UNITY_EDITOR
                    Debug.DrawLine(transform.position + _elephantEyeOffset,
                        hit.collider.gameObject.transform.position, Color.white);
#endif
                    _shockCooldown.timeInSeconds = _shockedCooldownMS / 1000.0f;
                    _shockCooldown.Start();
                    miceInRange.Add(coll.gameObject);
                    result = true;
                }
#if UNITY_EDITOR
                else if (hitSomething)
                {
                    Debug.DrawLine(transform.position + _elephantEyeOffset, hit.point, Color.red);
                }
#endif
            }
        }
        return result;
    }

    private IEnumerator CamouflageTimeExceededChecker()
    {
        milliseconds_left = _camouflageMaxDurationMS;
        while (milliseconds_left > 0) {
            milliseconds_left -= (int) (Time.deltaTime * 1000f);
            yield return null;
        }
        milliseconds_left = 0;

        CamouflageTimeExceeded();
    }

    private void CamouflageTimeExceeded()
    {
        if (!CamouflageModeActive) return;

        ExitCamouflageMode();
        Debug.Log("OnElephantFallFromPedestal");

        if (OnElephantFallFromPedestal != null)
        {
            Debug.Log("OnElephantFallFromPedestal");
            OnElephantFallFromPedestal();
            m_stunned = true;
        }
    }


}
