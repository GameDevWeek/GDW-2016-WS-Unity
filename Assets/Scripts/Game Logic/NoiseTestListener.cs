using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTestListener : MonoBehaviour, INoiseListener {

    public void Inform(NoiseSourceData data) {
        Debug.DrawLine(transform.position, data.initialPosition);
    }
}
