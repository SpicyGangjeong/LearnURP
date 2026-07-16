using UnityEditor;


[CustomEditor(typeof(ApplyGravity))]
[CanEditMultipleObjects]
public class ApplyGravityComponent : Editor
{
    SerializedProperty m_fGravityAmplitude;

    void OnEnable()
    {
        m_fGravityAmplitude = serializedObject.FindProperty("m_fGravityAmplitude");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_fGravityAmplitude);
        serializedObject.ApplyModifiedProperties();
    }
}