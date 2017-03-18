using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadControl : MonoBehaviour {
    [SerializeField]
    private Transform m_head;

    public bool updateHeadRotation = false;
    public float desiredAngleInDeg = 0.0f;
    public float headRotationSpeed = 360.0f;

    private float m_curAngleInDeg = 0.0f;

    public void SetHeadRotation(float angleInDeg) {
        updateHeadRotation = true;
        this.desiredAngleInDeg = angleInDeg;
    }

    void LateUpdate() {
        var angles = m_head.transform.eulerAngles;
        
        if (!updateHeadRotation) {
            desiredAngleInDeg = angles.y;
        }

        m_curAngleInDeg = UpdateValue(m_curAngleInDeg, desiredAngleInDeg, headRotationSpeed);

        m_head.transform.eulerAngles = new Vector3(angles.x, m_curAngleInDeg, angles.z);
    }

    float GetDirection(float value, float desiredValue, float epsilon) {
        var azimuthDiff = To360Degree(desiredValue - value);

        if (azimuthDiff > 180) {
            azimuthDiff = -(360 - azimuthDiff);

        } else if (azimuthDiff < -180) {
            azimuthDiff = 360 + azimuthDiff;
        }

        if (Mathf.Abs(azimuthDiff) <= epsilon) {
            return 0.0f;
        } else if (azimuthDiff > 0.0f) {
            return 1.0f;
        }
        
        return -1.0f;
    }

    float UpdateValue(float value, float desiredValue, float speed) {
        float dir = GetDirection(value, desiredValue, Mathf.Epsilon);
        
        value += dir * speed * Time.deltaTime;

        if (dir != GetDirection(value, desiredValue, Mathf.Epsilon)) {
            value = desiredValue;
        }

        return value;
    }

    float To360Degree(float v) {
        v = v % 360;

        if (v < 0) {
            v += 360;
        }

        return v;
    }
}
