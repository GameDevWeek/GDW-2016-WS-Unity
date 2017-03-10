using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerInspector : Editor {

    private Spawner m_spawner;
    private SerializedObject m_serializedTarget;
    private SerializedProperty m_list;
    private SerializedProperty m_defaultPreloadSize;
    private Dictionary<string, GameObjectPool> m_goPools;

    private List<bool> m_foldoutToggle = new List<bool>();

    private void OnEnable() {
        m_spawner = (Spawner)target;
        m_serializedTarget = new SerializedObject(m_spawner);
        m_list = m_serializedTarget.FindProperty("m_samples");

        for (int i = 0; i < m_list.arraySize; i++) {
            m_foldoutToggle.Add(false);
        }
        
        m_defaultPreloadSize = m_serializedTarget.FindProperty("m_defaultPreloadSize");
        m_goPools = (Dictionary<string, GameObjectPool>) typeof(Spawner).GetField("m_goPools", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(m_spawner);
    }

    public override void OnInspectorGUI() {
        EditorUtility.SetDirty(target);
        m_serializedTarget.Update();

        EditorGUILayout.PropertyField(m_defaultPreloadSize);

        EditorGUILayout.Space();
        var list = m_spawner.samples;

        bool hasUnitializedElements = false;

        for (int i = 0; i < m_list.arraySize; i++) {
            SerializedProperty sample = m_list.GetArrayElementAtIndex(i);

            if (!hasUnitializedElements) {
                hasUnitializedElements = list[i].prefab == null;
            }
            
            string name = list[i].prefab != null ? list[i].prefab.name : ("Uninitialized Element");

            EditorGUILayout.BeginHorizontal();
            m_foldoutToggle[i] = EditorGUILayout.Foldout(m_foldoutToggle[i], name, true);
            // Number of active and total number of available spawned objects
            GameObjectPool pool;
            if( m_goPools.TryGetValue(name, out pool)) {
                EditorGUILayout.HelpBox(pool.numActive + "/" + pool.elements.Count, MessageType.None);
            }

            //EditorGUILayout.HelpBox(pool.numActive + "/" + pool.elements.Count, MessageType.Info);
            //GUILayout.(pool.numActive + "/" + pool.elements.Count);

            bool removed = false;
            if (GUILayout.Button("X", GUILayout.Width(30.0f)) && 
                EditorUtility.DisplayDialog("Remove pool sample", "Are you sure you want to remove the pool sample?", "Yes", "No")) {
                m_list.DeleteArrayElementAtIndex(i);
                m_foldoutToggle.RemoveAt(i);
                removed = true;
            }
            EditorGUILayout.EndHorizontal();

            if (!removed && m_foldoutToggle[i]) {
                SerializedProperty prefab = sample.FindPropertyRelative("prefab");
                SerializedProperty preloadSize = sample.FindPropertyRelative("preloadSize");
                SerializedProperty allowPoolGrowth = sample.FindPropertyRelative("allowPoolGrowth");

                EditorGUILayout.PropertyField(prefab);
                EditorGUILayout.PropertyField(preloadSize);
                EditorGUILayout.PropertyField(allowPoolGrowth);
            }
        }

        if (GUILayout.Button("Add")) {
            list.Add(new Spawner.SpawnSample());
            m_foldoutToggle.Add(false);
        }

        if (hasUnitializedElements) {
            EditorGUILayout.HelpBox("There are uninitialized elements. Make sure all prefabs are set.", MessageType.Error);
        }

        m_serializedTarget.ApplyModifiedProperties();
    }
}
