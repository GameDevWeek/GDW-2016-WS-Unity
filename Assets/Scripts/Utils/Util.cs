using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Polar {
    public float radius;
    public float elevation;
    public float azimuth;

    public Polar(float radius, float elevation, float azimuth) {
        this.radius = radius;
        this.elevation = elevation;
        this.azimuth = azimuth;
    }
}

public class Util {
    public static GameObject Find(string name) {
        GameObject go = GameObject.Find(name);
        Debug.Assert(go != null, "Could not find object " + name);

        return go;
    }

    public static T Find<T>(string name) {
        GameObject go = Find(name);
        Debug.Assert(go.GetComponent<T>() != null, "Found object " + name + " but component " + typeof(T).Name + " is missing");

        return go.GetComponent<T>();
    }

    public static T FindChildInHierarchy<T>(Transform root, string name) where T : Object {
        var go = FindChildInHierarchy(root, name);
        return go ? go.GetComponent<T>() : null;
    }

    public static GameObject FindChildInHierarchy(Transform root, string name, bool includeInactive = true) {
        Transform[] children = root.GetComponentsInChildren<Transform>(includeInactive);
        foreach (Transform child in children) {
            if (child.name == name)
                return child.gameObject;
        }

        return null;
    }

    public static void RemoveAllChildren(GameObject go) {
        for (int i = go.transform.childCount - 1; i >= 0; --i) {
            var child = go.transform.GetChild(i).gameObject;
            Object.Destroy(child);
        }
    }

    /// <summary>
    /// Shuffles an IList with Fisher-Yates shuffle.
    /// </summary>
    public static void Shuffle<T>(IList<T> list) {
        for (int i = 0; i < list.Count - 1; ++i) {
            int randomIdx = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[randomIdx];
            list[randomIdx] = tmp;
        }
    }

    public static T RandomElement<T>(T[] list) {
        return list[Random.Range(0, list.Length)];
    }

    public static Vector3 PolarToVector(float radius, float elevation, float azimuth) {
        return new Vector3(-radius * Mathf.Cos(elevation) * Mathf.Sin(azimuth),
                            radius * Mathf.Sin(elevation),
                            radius * Mathf.Cos(elevation) * Mathf.Cos(azimuth));
    }

    public static Polar VectorToPolar(Vector3 v) {
        float r = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        return new Polar(r, Mathf.Asin(v.y / r), -Mathf.Atan2(v.x, v.z));
    }

    /**
     * <summary>Checks if a given point is in the field of view of an object with a position and viewing direction.</summary>
     */
    public static bool IsPointInFOV(Vector3 point, Vector3 position, Vector3 direction, float fieldOfViewInDeg, Vector3 projPlaneNormal) {
        var dirP = Vector3.ProjectOnPlane(direction, projPlaneNormal);
        var dP = Vector3.ProjectOnPlane(point - position, projPlaneNormal);
        return Vector3.Angle(dirP, dP) <= fieldOfViewInDeg * 0.5f;
    }
}
