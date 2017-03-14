using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    [SerializeField]
    public string Name;

    [Range(0, 1000)]
    [SerializeField]
    public int ScoreValue;

}
