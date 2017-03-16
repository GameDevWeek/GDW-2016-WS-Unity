using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CamouflageController : MonoBehaviour
{

    /// <summary>
    /// Event invoked if elephant is shocked because of mice in range.
    /// </summary>
    public static event ShockEvent OnElephantShocked;
    public delegate void ShockEvent();
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
    /// Event invoked if camouflage mode exits (will be invoked even if OnElephantShocked/OnElephantFallFromPedestal is invoked).
    /// </summary>
    public static event ExitsCamouflageModeEvent OnElephantExitsCamouflageMode;
    public delegate void ExitsCamouflageModeEvent();

    [SerializeField]
    private string _mouseTag = "Mouse";
    [SerializeField]
    private int _mouseReactDistance = 3;
    [SerializeField]
    [Tooltip("Offset from transform.position to calculate mouse in range raycast.")]
    private Vector3 _elephantEyeOffset = Vector3.up * 0.5f;
    [SerializeField]
    [Range(0, 20000)]
    [Tooltip("Milliseconds: Maximum duration in ms for player in camouflage mode. If time is exceeded elephant will fall from pedestal.")]
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

    private void FixedUpdate()
    {
        // TODO: Ist der Elefant immer geschockt, wenn er eine Maus sieht, oder nur, wenn der CamouflageMode on ist?

        if (CamouflageModeActive)
        {
            if (IsMouseInRange())
            {
                ExitCamouflageMode();
                Debug.Log("OnElephantShocked");
                if (OnElephantShocked != null)
                {
                    OnElephantShocked();
                }
            }
        }
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
        if (!CamouflagePossible()) return false;

        Debug.Log("CamouflageMode on");

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
        Debug.Log("CamouflageMode off");

        if (!calledFromCoroutine) StopCoroutine(_camouflageTimeExceededChecker);

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
                    Debug.Log("mouse visible");
                    return true;
                }
            }
        }
        //Debug.Log("NO mice visible");
        return false;
        //       
        //        bool hitSomething = Physics.Raycast(transform.position + _elephantEyeOffset, direction, out hit, _mouseReactDistance);
        //#if UNITY_EDITOR
        //        if (hitSomething)
        //        {
        //            Debug.DrawLine(transform.position + _elephantEyeOffset,
        //                transform.position + _elephantEyeOffset + direction, Color.white);
        //            Debug.DrawLine(transform.position + _elephantEyeOffset, hit.point, Color.red);
        //            Debug.Log("hitSomething");
        //        }
        //        else
        //        {
        //            Debug.Log("hitSomething");
        //            Debug.DrawLine(transform.position + _elephantEyeOffset,
        //                transform.position + _elephantEyeOffset + direction, Color.green);
        //        }
        //#endif
    }

    private IEnumerator CamouflageTimeExceededChecker()
    {
        yield return new WaitForSeconds(_camouflageMaxDurationMS / 1000.0f);

        CamouflageTimeExceeded();
    }

    private void CamouflageTimeExceeded()
    {
        if (!CamouflageModeActive) return;

        ExitCamouflageMode();
        Debug.Log("OnElephantFallFromPedestal");

        if (OnElephantFallFromPedestal != null)
        {
            OnElephantFallFromPedestal.Invoke();
        }
    }


}
