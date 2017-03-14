using System;
using Assets.Scripts.K.I_;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoints))]
public class WaypointEditor : Editor {


    void OnSelectionChanged() {

    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView){

        var waypoints = target as Waypoints;
        if(waypoints == null) return;
        var spoints = serializedObject.FindProperty("points");

        for (var i = 0; i < spoints.arraySize; ++i) {
            var spoint = spoints.GetArrayElementAtIndex(i);
            var waypoint = spoint.vector3Value;

            EditorGUI.BeginChangeCheck();
            var pos = Handles.FreeMoveHandle(waypoint, Quaternion.identity, 0.25f, Vector3.one * 0.25f, Handles.RectangleCap);
            Handles.Label(new Vector3(pos.x, 0.5f, pos.z), "Point " + i);

            if (EditorGUI.EndChangeCheck()) {
                Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
                var plane = new Plane(Vector3.up, Vector3.zero);
                float f;
                plane.Raycast(ray, out f);
                pos = ray.GetPoint(f);

                spoint.vector3Value = pos;
                serializedObject.ApplyModifiedProperties();
            }

        }

        foreach (var pair in waypoints.pairs) {
            if(pair.First == pair.Second) continue;

            try {
                var first = waypoints.points[pair.First];
                var second = waypoints.points[pair.Second];
                Handles.DrawLine(first, second);

                var dir = (second - first).normalized;
                Handles.ArrowCap(0, second - dir*1.2f, Quaternion.LookRotation(second-first, Vector3.up), 1);
            }
            catch (IndexOutOfRangeException) {

            }
        }

        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();

    }

    public override void OnInspectorGUI() {
        var waypoints = target as Waypoints;

        var spoints = serializedObject.FindProperty("points");
        EditorGUILayout.PropertyField(spoints, true);
        if (GUILayout.Button("add")) {
            spoints.InsertArrayElementAtIndex(spoints.arraySize);
        }

        var spairs = serializedObject.FindProperty("pairs");

        for (int i = 0; i < spairs.arraySize; ++i) {

            var point = spairs.GetArrayElementAtIndex(i);

            point.FindPropertyRelative("First").intValue = Mathf.Clamp(point.FindPropertyRelative("First").intValue, 0, spoints.arraySize-1);
            point.FindPropertyRelative("Second").intValue = Mathf.Clamp(point.FindPropertyRelative("Second").intValue, 0, spoints.arraySize-1);
        }

        EditorGUILayout.PropertyField(spairs, true);



        serializedObject.ApplyModifiedProperties();
    }
}
