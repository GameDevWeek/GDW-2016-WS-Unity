using UnityEditor;
using UnityEngine;

public class ReplaceObjectTool : EditorWindow {

    [MenuItem("Tools/Utils/Object Replacer Tool")]
    static void OpenWindow() {
        EditorWindow.GetWindow<ReplaceObjectTool>();
    }

    private Object obj = null;

    private void OnGUI() {

        obj = EditorGUILayout.ObjectField("Prefab", obj, typeof(GameObject), false);

        if (GUILayout.Button("Replace")) {
            var go = obj as GameObject;
            if(go == null) return;

            var transforms = Selection.GetTransforms(SelectionMode.TopLevel);


            foreach (var transform in transforms) {
                Undo.RecordObject(transform.gameObject, "Replacing with prefabs");

                var ng = PrefabUtility.InstantiatePrefab(go) as GameObject;
                ng.transform.position = transform.position;
                ng.transform.rotation = transform.rotation;
                ng.transform.localScale = transform.localScale;
                GameObject.DestroyImmediate(transform.gameObject);
            }


        }
    }

}