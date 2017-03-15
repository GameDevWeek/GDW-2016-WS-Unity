using System;
using UnityEngine;

[Serializable]
public class SerProperty {
    public enum Type {
        INT, FLOAT, BOOL, GAMEOBJECT, COMPONENT, STRING, OBJECT, VECTOR3
    }

    public string Name;
    public Type PropType;

    public bool boolValue;
    public int intValue;
    public float floatValue;
    public Component ComponentValue;
    public GameObject GameObjectValue;
    public string StringValue;
    public UnityEngine.Object ObjectValue;
    public Vector3 Vector3Value;

    public SerProperty() { Name = ""; }
    public SerProperty(string name, Type propType) {
        Name = name;
        PropType = propType;
    }

    public void ValueFrom(SerProperty other) {
        if(other == null)throw new ArgumentException("Property should not be null");
        if(PropType != other.PropType) throw new ArgumentException("Properties need same type");
        switch (PropType) {
            case Type.INT: intValue = other.intValue; break;
            case Type.FLOAT: floatValue = other.floatValue; break;
            case Type.BOOL: boolValue = other.boolValue; break;
            case Type.GAMEOBJECT: GameObjectValue = other.GameObjectValue; break;
            case Type.COMPONENT: ComponentValue = other.ComponentValue; break;
            case Type.STRING: StringValue = other.StringValue; break;
            case Type.OBJECT: ObjectValue = other.ObjectValue; break;
            case Type.VECTOR3: Vector3Value = other.Vector3Value; break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}