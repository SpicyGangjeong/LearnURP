using UnityEditor;
using UnityEngine;

namespace Propertydrawer
{
    [CustomPropertyDrawer(typeof(Core.Job.JobBase))]
    public class JobBase : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            label.text = property.FindPropertyRelative("m_strName").stringValue;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
