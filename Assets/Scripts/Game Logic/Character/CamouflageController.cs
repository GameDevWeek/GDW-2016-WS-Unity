﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageController : MonoBehaviour
{

    public const string PEDESTAL_TAG = "Pedestal";
    public const string MOUSE_TAG = "Mouse";

    private List<Collider> _pedestalsInRange;
    private List<Collider> _miceInRange;

    public bool CamouflageModeActive { get; private set; }

    void Awake()
    {
        _pedestalsInRange = new List<Collider>();
        _miceInRange = new List<Collider>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetButtonDown("Camouflage"))
	    {
            bool result = ToggleCamouflageMode();
            Debug.Log(result ? "camouflage on" : "camouflage off");
	    }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if camouflage mode is now active</returns>
    public bool ToggleCamouflageMode()
    {
        bool result = CamouflageModeActive ? ExitCamouflageMode() : EnterCamouflageMode();
        if (result) CamouflageModeActive = !CamouflageModeActive;
        return CamouflageModeActive;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if mode could be applied</returns>
    public bool ExitCamouflageMode()
    {


        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if mode could be applied</returns>
    public bool EnterCamouflageMode()
    {
        if (!CamouflagePossible())   return false;



        return true;
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

    void OnTriggerEnter(Collider coll)
    {
        if (PEDESTAL_TAG == coll.tag && !_pedestalsInRange.Contains(coll))
        {

            _pedestalsInRange.Add(coll);
            Debug.Log("pedestal enter");
        }
        if (MOUSE_TAG == coll.tag && !_miceInRange.Contains(coll))
        {

            _miceInRange.Add(coll);
            // TODO Methode "ElefantErschrecktSich"
            Debug.Log("mouse enter");
        }


    }

    void OnTriggerExit(Collider coll)
    {
        if (PEDESTAL_TAG == coll.tag)
        {
            _pedestalsInRange.Remove(coll);
            Debug.Log("pedestal exit");
        }
        if (MOUSE_TAG == coll.tag)
        {
            _miceInRange.Remove(coll);
            Debug.Log("mouse exit");
        }


    }


}
