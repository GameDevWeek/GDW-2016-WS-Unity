using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CamouflageController : MonoBehaviour
{

    /// <summary>
    /// Event invoked if elephant is shocked because of mice in range.
    /// </summary>
    public EventHandler<EventArgs> OnElephantShocked;
    /// <summary>
    /// Event invoked if elephant is falling from pedestal if max time is over.
    /// </summary>
    public EventHandler<EventArgs> OnElephantFallFromPedestal;

    /// <summary>
    /// Event invoked if camouflage mode was entered.
    /// </summary>
    public EventHandler<EventArgs> OnElephantEntersCamouflageMode;
    /// <summary>
    /// Event invoked if camouflage mode exits (will be invoked even if OnElephantShocked/OnElephantFallFromPedestal is invoked).
    /// </summary>
    public EventHandler<EventArgs> OnElephantExitsCamouflageMode;

    [SerializeField]
    private string _pedestalTag;
    [SerializeField]
    private string _mouseTag;
    [SerializeField] [Range(0,20000)] [Tooltip("Milliseconds: Maximum duration in ms for player in camouflage mode. If time is exceeded elephant will fall from pedestal.")]
    private int _camouflageMaxDurationMS;

    private Coroutine _camouflageTimeExceededChecker;

    private List<Collider> _pedestalsInRange;
    private List<Collider> _miceInRange;
    private int _enemyInRangeCounter;

    public bool CamouflageModeActive { get; private set; }

    private void Awake()
    {
        this._pedestalsInRange = new List<Collider>();
        this._miceInRange = new List<Collider>();
        this._enemyInRangeCounter = 0;
        this.CamouflageModeActive = false;
    }

    public void EnemyInRange(GameObject enemy)
    {
        _enemyInRangeCounter++;
    }

    public void EnemyOutOfRange(GameObject enemy)
    {
        _enemyInRangeCounter--;
        if (_enemyInRangeCounter < 0) _enemyInRangeCounter = 0;
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
        if (!CamouflagePossible())   return false;

        if (OnElephantEntersCamouflageMode != null)
        {
            OnElephantEntersCamouflageMode.Invoke(this, new EventArgs());
        }

        _camouflageTimeExceededChecker = StartCoroutine(CamouflageTimeExceededChecker());
        CamouflageModeActive = true;
        return true;
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

        if (OnElephantExitsCamouflageMode != null)
        {
            OnElephantExitsCamouflageMode(this, new EventArgs());
        }

        CamouflageModeActive = false;
    }

    private bool CamouflagePossible()
    {
        if (_pedestalsInRange.Count > 0 
            && _miceInRange.Count == 0
            && _enemyInRangeCounter == 0)
        {
            return true;
        }

        return false;
    }

    private IEnumerator CamouflageTimeExceededChecker()
    {
        yield return new WaitForSeconds(_camouflageMaxDurationMS/1000.0f);
        
        CamouflageTimeExceeded();
    }

    private void CamouflageTimeExceeded()
    {
        if (!CamouflageModeActive) return;

        ExitCamouflageMode();

        if (OnElephantFallFromPedestal != null)
        {
            OnElephantFallFromPedestal.Invoke(this, new EventArgs());
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        
        if (_pedestalTag == coll.tag && !_pedestalsInRange.Contains(coll))
        {
            _pedestalsInRange.Add(coll);
        }
        if (_mouseTag == coll.tag && !_miceInRange.Contains(coll))
        {
            _miceInRange.Add(coll);

            if (CamouflageModeActive)
            {
                ExitCamouflageMode();
            }

            if (OnElephantShocked != null)
            {
                OnElephantShocked.Invoke(this, new EventArgs());
            }
        }


    }

    private void OnTriggerExit(Collider coll)
    {
        if (_pedestalTag == coll.tag && _pedestalsInRange.Contains(coll))
        {
            _pedestalsInRange.Remove(coll);
        }
        if (_mouseTag == coll.tag && _miceInRange.Contains(coll))
        {
            _miceInRange.Remove(coll);
        }


    }


}
