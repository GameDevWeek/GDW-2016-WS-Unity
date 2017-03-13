using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamouflageController : MonoBehaviour
{

    public EventHandler<EventArgs> OnElephantShocked;

    public EventHandler<EventArgs> OnElephantEntersCamouflageMode;
    public EventHandler<EventArgs> OnElephantExitsCamouflageMode;

    [SerializeField]
    private string _pedestalTag;
    [SerializeField]
    private string _mouseTag;

    private List<Collider> _pedestalsInRange;
    private List<Collider> _miceInRange;

    public bool CamouflageModeActive { get; private set; }

    private void Awake()
    {
        _pedestalsInRange = new List<Collider>();
        _miceInRange = new List<Collider>();
    }

    // Update is called once per frame
    private void Update () {

	    if (Input.GetButtonDown("Camouflage"))
	    {
            ToggleCamouflageMode();
	    }

    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns>true if mode could be applied</returns>
    public bool TryEnterCamouflageMode()
    {
        if (!CamouflagePossible())   return false;

        if (OnElephantEntersCamouflageMode != null)
        {
            OnElephantEntersCamouflageMode.Invoke(this, new EventArgs());
        }

        CamouflageModeActive = true;
        //Debug.Log("camouflage on");
        return true;
    }
    
    public void ExitCamouflageMode()
    {
        if (OnElephantExitsCamouflageMode != null)
        {
            OnElephantExitsCamouflageMode(this, new EventArgs());
        }

        CamouflageModeActive = false;
        //Debug.Log("camouflage off");
    }

    private bool CamouflagePossible()
    {
        if (_pedestalsInRange.Count > 0 
            && _miceInRange.Count == 0)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider coll)
    {
        
        if (_pedestalTag == coll.tag && !_pedestalsInRange.Contains(coll))
        {
            _pedestalsInRange.Add(coll);
            //Debug.Log("pedestal enter");


        }
        if (_mouseTag == coll.tag && !_miceInRange.Contains(coll))
        {
            _miceInRange.Add(coll);
            //Debug.Log("mouse enter");

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
        if (_pedestalTag == coll.tag)
        {
            _pedestalsInRange.Remove(coll);
            //Debug.Log("pedestal exit");
        }
        if (_mouseTag == coll.tag)
        {
            _miceInRange.Remove(coll);
            //Debug.Log("mouse exit");
        }


    }


}
