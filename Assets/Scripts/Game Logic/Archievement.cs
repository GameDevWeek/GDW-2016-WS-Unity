using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class Archievement : ScriptableObject{
#if UNITY_EDITOR
    public string name;
    [TextArea] public string text;

    public float defaultValue;
    public float currentValue { get; set; }
    public float archievmentReachedValue;

    public bool saveOnLocalMachine;

    [Header("listener")]
    [Tooltip("The (static) event's name, on this component")] public SerProperty property;
    public bool increment_instead_set;
    public float scaling;

    [HideInInspector] public string assembly;
    [HideInInspector] public string classPath;

    public void OnValidate() {
        property.PropType = SerProperty.Type.OBJECT;
        if (property.ObjectValue != null) {
            if (!(property.ObjectValue is MonoScript)) {
                property.ObjectValue = null;
            }else {
                assembly = (property.ObjectValue as MonoScript).GetClass().AssemblyQualifiedName;
                classPath = AssetDatabase.GetAssetPath(property.ObjectValue);
            }
        }
    }
#endif
}

