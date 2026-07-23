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
            string strType = property.managedReferenceFullTypename;
            if (false == string.IsNullOrEmpty(strType))
            {
                int iDot = strType.LastIndexOf('.');
                string strShort = (-1 < iDot) ? strType.Substring(iDot + 1) : strType;
                label.text = $"{label.text} ({strShort})";
            }
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
