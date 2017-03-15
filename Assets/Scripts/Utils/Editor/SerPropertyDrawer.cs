using System;
using Assets.Soraphis.Lib;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerProperty))]
public class SerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        var labelRect = position.SplitRectH(10, 0, 4);

        var typeRect = position.SplitRectH(10, 4, 3);
        var valueRect = position.SplitRectH(10, 7, 3);

        var name_prop = property.FindPropertyRelative("Name");
        var type_prop = property.FindPropertyRelative("PropType");

        name_prop.stringValue = EditorGUI.TextField(labelRect, GUIContent.none, name_prop.stringValue);
        var type = (SerProperty.Type) EditorGUI.EnumPopup(typeRect, GUIContent.none, (SerProperty.Type) type_prop.enumValueIndex);
        type_prop.enumValueIndex = (int) type;

        switch (type) {
            case SerProperty.Type.INT:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("intValue"), GUIContent.none, false);
                break;
            case SerProperty.Type.FLOAT:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("floatValue"),GUIContent.none, false);
                break;
            case SerProperty.Type.BOOL:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("boolValue"), GUIContent.none,false);
                break;
            case SerProperty.Type.GAMEOBJECT:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("GameObjectValue"), GUIContent.none,false);
                break;
            case SerProperty.Type.COMPONENT:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("ComponentValue"), GUIContent.none,false);
                break;
            case SerProperty.Type.STRING:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("StringValue"),GUIContent.none, false);
                break;
            case SerProperty.Type.OBJECT:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("ObjectValue"),GUIContent.none, false);
                break;
            case SerProperty.Type.VECTOR3:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Vector3Value"),GUIContent.none, false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}