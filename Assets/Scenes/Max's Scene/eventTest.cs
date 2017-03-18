using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventTest : MonoBehaviour
{

    private UnityAction someListerner;

    void Awake()
    {
        someListerner=new UnityAction(someFunction);
    }

    void OnEnable()
    {
        EventManager.startListening("Test", someListerner);
    }

    void OnDisable()
    {
        EventManager.stopListening("Test",someListerner);

    }

    void someFunction()
    {
        Debug.Log("Some Function was Called!!!!einsELFS");
    }
}
