using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageController : MonoBehaviour
{

    private string PEDESTAL_TAG = "Pedestal";

    private List<Collider> _activePedestal;

    public bool CamouflageModeActive { get; private set; }

    void Awake()
    {
        _activePedestal = new List<Collider>();
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
        if (_activePedestal == null) return false;



        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if mode could be applied</returns>
    public bool EnterCamouflageMode()
    {
        if (_activePedestal.Count == 0)   return false;



        return true;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (PEDESTAL_TAG == coll.tag)
        {
            _activePedestal.Add(coll);
            Debug.Log("enter");
        }


    }

    void OnTriggerExit(Collider coll)
    {
        if (PEDESTAL_TAG == coll.tag)
        {
            _activePedestal.Remove(coll);
            Debug.Log("exit");
        }


    }


}
