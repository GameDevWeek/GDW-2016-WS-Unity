using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public const string JADE_ELEPHANT_TAG = "JadeElephant";

    private Collider _grabbableInRange;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Grab"))
        {
            Grab();
        }

    }

    public void Grab()
    {
        if (_grabbableInRange == null) return;

        if (_grabbableInRange.tag == JADE_ELEPHANT_TAG)
        {
            DestroyImmediate(_grabbableInRange.gameObject);
            _grabbableInRange = null;

            // TODO Methode "ScoreErhoehen"
            Debug.Log("Score++");
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (JADE_ELEPHANT_TAG == coll.tag)
        {
            _grabbableInRange = coll;
            Debug.Log("grabbable enter");


        }


    }

    void OnTriggerExit(Collider coll)
    {
        if (JADE_ELEPHANT_TAG == coll.tag && _grabbableInRange == coll)
        {
            _grabbableInRange = null;
            Debug.Log("grabbable exit");
        }


    }
}
