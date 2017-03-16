using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CamouflageController : MonoBehaviour
{

    /// <summary>
    /// Event invoked if elephant is shocked because of mice in range.
    /// </summary>
    public static EventHandler<EventArgs> OnElephantShocked;
    /// <summary>
    /// Event invoked if elephant is falling from pedestal if max time is over.
    /// </summary>
    public static EventHandler<EventArgs> OnElephantFallFromPedestal;

    /// <summary>
    /// Event invoked if camouflage mode was entered.
    /// </summary>
    public static EventHandler<EventArgs> OnElephantEntersCamouflageMode;
    /// <summary>
    /// Event invoked if camouflage mode exits (will be invoked even if OnElephantShocked/OnElephantFallFromPedestal is invoked).
    /// </summary>
    public static EventHandler<EventArgs> OnElephantExitsCamouflageMode;
    
    [SerializeField]
    private string _mouseTag = "Mouse";
    [SerializeField]
    private int _mouseDistanceToReact = 3;
    [SerializeField] [Range(0,20000)] [Tooltip("Milliseconds: Maximum duration in ms for player in camouflage mode. If time is exceeded elephant will fall from pedestal.")]
    private int _camouflageMaxDurationMS = 5000;

    private Coroutine _camouflageTimeExceededChecker;
    
    private List<Collider> _miceInRange;
    private List<GameObject> _enemiesInRange;

    public bool CamouflageModeActive { get; private set; }

    private void Awake()
    {
        this._miceInRange = new List<Collider>();
        this._enemiesInRange = new List<GameObject>();

        this.CamouflageModeActive = false;
    }

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
        if (!CamouflagePossible())   return false;

        if (OnElephantEntersCamouflageMode != null)
        {
            OnElephantEntersCamouflageMode.Invoke(this, new EventArgs());
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

        if (OnElephantExitsCamouflageMode != null)
        {
            OnElephantExitsCamouflageMode(this, new EventArgs());
        }

        CamouflageModeActive = false;
    }

    private bool CamouflagePossible()
    {
        int miceVisible = 0;
        foreach (Collider mouseCollider in _miceInRange)
        {
            if (Physics.Raycast(transform.position, mouseCollider.gameObject.transform.position - transform.position))
            {
                miceVisible++;
            }
        }

        if (miceVisible == 0
            && _enemiesInRange.Count == 0)
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
        if (_mouseTag == coll.tag && !_miceInRange.Contains(coll))
        {
            _miceInRange.Add(coll);
            
            // mouse triggered not visible
            if (!Physics.Raycast(transform.position, coll.gameObject.transform.position - transform.position))
                return;

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
        if (_mouseTag == coll.tag && _miceInRange.Contains(coll))
        {
            _miceInRange.Remove(coll);
        }


    }


}
