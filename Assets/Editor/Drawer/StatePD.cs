using UnityEditor;
using UnityEngine;

namespace Propertydrawer
{
    [CustomPropertyDrawer(typeof(Logic.State.CState))]
    public class StatePD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.LabelField(position, property.managedReferenceFullTypename.Substring(property.managedReferenceFullTypename.LastIndexOf('.')));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
