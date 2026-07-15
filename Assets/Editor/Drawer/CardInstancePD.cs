using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Propertydrawer
{
    [CustomPropertyDrawer(typeof(Logic.Card.CardInstance))]
    public class CardInstancePD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty cardData = property.FindPropertyRelative("m_pData");
            SerializedProperty cardScriptedData = cardData.FindPropertyRelative("m_Data");
            SerializedProperty cardCurrentPile = cardData.FindPropertyRelative("m_eCurrentPile");

            string strCardName = cardScriptedData.FindPropertyRelative("strName").stringValue;
            label.text = strCardName + "\t" + cardCurrentPile.enumDisplayNames[cardCurrentPile.enumValueIndex];

            EditorGUI.PropertyField(position, cardData, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty pCardData = property.FindPropertyRelative("m_pData");
            return EditorGUI.GetPropertyHeight(pCardData, label, true);
        }
    }
}
