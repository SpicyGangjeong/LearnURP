using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Propertydrawer
{
    /// <summary>
    /// CardEffect Inspector: Block(When) → Steps(Who) → Operations(What) → Params.
    /// List chrome (drag / + / -) = ReorderableList. Field layout = Draw*Element per level.
    /// </summary>
    [CustomPropertyDrawer(typeof(Logic.Card.CardEffect))]
    public class CardEffectPD : PropertyDrawer
    {
        const float s_fPadding = 2f;

        readonly Dictionary<string, ReorderableList> m_vListCache = new Dictionary<string, ReorderableList>();

        float LineHeight => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float fY = position.y;
            Rect rcFold = new Rect(position.x, fY, position.width, LineHeight);
            property.isExpanded = EditorGUI.Foldout(rcFold, property.isExpanded, label, true);
            fY += LineHeight;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                DescribeBlock(new Rect(position.x, fY, position.width, position.yMax - fY), property);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float fHeight = LineHeight;
            if (false == property.isExpanded)
            {
                return fHeight;
            }

            return fHeight + GetBlockHeight(property);
        }
        
        #region Block (When)

        void DescribeBlock(Rect position, SerializedProperty property)
        {
            SerializedProperty pBlock = property.FindPropertyRelative("m_Block");
            SerializedProperty pTrigger = pBlock.FindPropertyRelative("m_eTrigger");
            SerializedProperty pSteps = pBlock.FindPropertyRelative("m_vSteps");

            float fY = position.y;

            // When
            float fTriggerH = EditorGUI.GetPropertyHeight(pTrigger, true);
            EditorGUI.PropertyField(new Rect(position.x, fY, position.width, fTriggerH), pTrigger, new GUIContent("Trigger ( 언제 )"), true);
            fY += fTriggerH + s_fPadding;

            // Who — Steps list
            ReorderableList pStepsList = GetOrCreateStepsList(pSteps);
            float fStepsH = pStepsList.GetHeight();
            pStepsList.DoList(new Rect(position.x, fY, position.width, fStepsH));
        }

        float GetBlockHeight(SerializedProperty property)
        {
            SerializedProperty pBlock = property.FindPropertyRelative("m_Block");
            SerializedProperty pTrigger = pBlock.FindPropertyRelative("m_eTrigger");
            SerializedProperty pSteps = pBlock.FindPropertyRelative("m_vSteps");

            return EditorGUI.GetPropertyHeight(pTrigger, true)
                + s_fPadding
                + GetOrCreateStepsList(pSteps).GetHeight();
        }

        #endregion

        #region Steps (Who) — edit layout in DrawStepElement

        ReorderableList GetOrCreateStepsList(SerializedProperty pSteps)
        {
            return GetOrCreateList(
                pSteps,
                "Steps (누구에게)",
                DrawStepElement,
                GetStepElementHeight,
                OnAddStep);
        }

        void DrawStepElement(Rect rc, int iIndex, bool bActive, bool bFocus)
        {
            SerializedProperty pSteps = m_pCurrentDrawList.serializedProperty;
            if (null == pSteps || iIndex < 0 || iIndex >= pSteps.arraySize)
            {
                return;
            }

            SerializedProperty pStep = pSteps.GetArrayElementAtIndex(iIndex);
            float fY = rc.y + s_fPadding;

            // BuildExpression order: Select → Count → Scope → Entity → Operations
            SerializedProperty pSelect = pStep.FindPropertyRelative("m_eSelect");
            SerializedProperty pSelectCount = pStep.FindPropertyRelative("m_iSelectCount");
            SerializedProperty pScope = pStep.FindPropertyRelative("m_eScope");
            SerializedProperty pEntity = pStep.FindPropertyRelative("m_eEntity");

            float fColW = rc.width * 0.5f;
            Rect rcCol = new Rect(rc.x, fY, fColW, LineHeight);
            ShowNamePropertyField(rcCol, pSelect);
            rcCol.x += fColW;
            ShowNamePropertyField(rcCol, pSelectCount);
            fY += LineHeight + s_fPadding;

            rcCol = new Rect(rc.x, fY, fColW, LineHeight);
            ShowNamePropertyField(rcCol, pScope);
            rcCol.x += fColW;
            ShowNamePropertyField(rcCol, pEntity);
            fY += LineHeight + s_fPadding;

            // What — Operations nested list
            SerializedProperty pOps = pStep.FindPropertyRelative("m_vOperations");
            ReorderableList pOpsList = GetOrCreateOperationsList(pOps);
            pOpsList.DoList(new Rect(rc.x, fY, rc.width, pOpsList.GetHeight()));
        }

        float GetStepElementHeight(int iIndex)
        {
            SerializedProperty pSteps = m_pHeightArrayContext;
            if (null == pSteps || iIndex < 0 || iIndex >= pSteps.arraySize)
            {
                return LineHeight * 2f + s_fPadding * 3f;
            }

            SerializedProperty pStep = pSteps.GetArrayElementAtIndex(iIndex);
            SerializedProperty pOps = pStep.FindPropertyRelative("m_vOperations");
            return s_fPadding
                + LineHeight + s_fPadding   // Select | Count
                + LineHeight + s_fPadding   // Scope | Entity
                + GetOrCreateOperationsList(pOps).GetHeight()
                + s_fPadding;
        }

        void OnAddStep(ReorderableList pList)
        {
            SerializedProperty pSteps = pList.serializedProperty;
            int iIndex = pSteps.arraySize;
            pSteps.InsertArrayElementAtIndex(iIndex);
            SerializedProperty pNew = pSteps.GetArrayElementAtIndex(iIndex);
            pNew.FindPropertyRelative("m_eEntity").enumValueIndex = 0;
            pNew.FindPropertyRelative("m_eScope").enumValueIndex = 0;
            pNew.FindPropertyRelative("m_eSelect").enumValueIndex = 0;
            pNew.FindPropertyRelative("m_iSelectCount").intValue = 0;
            pNew.FindPropertyRelative("m_vOperations").arraySize = 0;
        }

        #endregion

        #region Operations (What) — edit layout in DrawOperationElement

        ReorderableList GetOrCreateOperationsList(SerializedProperty pOps)
        {
            return GetOrCreateList(
                pOps,
                "Operations (무엇을)",
                DrawOperationElement,
                GetOperationElementHeight,
                OnAddOperation);
        }

        void DrawOperationElement(Rect rc, int iIndex, bool bActive, bool bFocus)
        {
            SerializedProperty pOps = m_pCurrentDrawList.serializedProperty;
            if (null == pOps || iIndex < 0 || iIndex >= pOps.arraySize)
            {
                return;
            }

            SerializedProperty pOp = pOps.GetArrayElementAtIndex(iIndex);
            float fY = rc.y + s_fPadding;

            SerializedProperty pValue = pOp.FindPropertyRelative("m_eValue");
            SerializedProperty pAmount = pOp.FindPropertyRelative("m_iValue");

            float fColW = rc.width / 2f;
            Rect rcCol = new Rect(rc.x, fY, fColW, LineHeight);
            ShowNamePropertyField(rcCol, pValue);
            rcCol.x += fColW;
            ShowNamePropertyField(rcCol, pAmount);
            fY += LineHeight + s_fPadding;

            SerializedProperty pParams = pOp.FindPropertyRelative("m_vParams");
            ReorderableList pParamsList = GetOrCreateParamsList(pParams);
            pParamsList.DoList(new Rect(rc.x, fY, rc.width, pParamsList.GetHeight()));
        }

        float GetOperationElementHeight(int iIndex)
        {
            SerializedProperty pOps = m_pHeightArrayContext;
            if (null == pOps || iIndex < 0 || iIndex >= pOps.arraySize)
            {
                return LineHeight + s_fPadding * 2f;
            }

            SerializedProperty pOp = pOps.GetArrayElementAtIndex(iIndex);
            SerializedProperty pParams = pOp.FindPropertyRelative("m_vParams");
            return s_fPadding
                + LineHeight
                + s_fPadding
                + GetOrCreateParamsList(pParams).GetHeight()
                + s_fPadding;
        }

        void OnAddOperation(ReorderableList pList)
        {
            SerializedProperty pOps = pList.serializedProperty;
            int iIndex = pOps.arraySize;
            pOps.InsertArrayElementAtIndex(iIndex);
            SerializedProperty pNew = pOps.GetArrayElementAtIndex(iIndex);
            pNew.FindPropertyRelative("m_eValue").enumValueIndex = 0;
            pNew.FindPropertyRelative("m_iValue").intValue = 0;
            pNew.FindPropertyRelative("m_vParams").arraySize = 0;
        }

        #endregion

        #region Params — edit layout in DrawParamElement

        ReorderableList GetOrCreateParamsList(SerializedProperty pParams)
        {
            return GetOrCreateList(
                pParams,
                "Params",
                DrawParamElement,
                GetParamElementHeight,
                OnAddParam);
        }

        void DrawParamElement(Rect rc, int iIndex, bool bActive, bool bFocus)
        {
            SerializedProperty pParams = m_pCurrentDrawList.serializedProperty;
            if (null == pParams || iIndex < 0 || iIndex >= pParams.arraySize)
            {
                return;
            }

            SerializedProperty pParam = pParams.GetArrayElementAtIndex(iIndex);
            float fY = rc.y + s_fPadding;

            SerializedProperty pKey = pParam.FindPropertyRelative("m_eKey");
            SerializedProperty pValue = pParam.FindPropertyRelative("m_iValue");

            float fColW = rc.width / 2f;
            Rect rcCol = new Rect(rc.x, fY, fColW, LineHeight);
            ShowNamePropertyField(rcCol, pKey);
            rcCol.x += fColW;
            ShowNamePropertyField(rcCol, pValue);
        }

        float GetParamElementHeight(int iIndex)
        {
            return s_fPadding + LineHeight + s_fPadding;
        }

        void OnAddParam(ReorderableList pList)
        {
            SerializedProperty pParams = pList.serializedProperty;
            int iIndex = pParams.arraySize;
            pParams.InsertArrayElementAtIndex(iIndex);
            SerializedProperty pNew = pParams.GetArrayElementAtIndex(iIndex);
            pNew.FindPropertyRelative("m_eKey").enumValueIndex = 0;
            pNew.FindPropertyRelative("m_iValue").floatValue = 0f;
        }

        #endregion

        #region ReorderableList cache / helpers

        SerializedProperty m_pHeightArrayContext = null;
        ReorderableList m_pCurrentDrawList = null;

        ReorderableList GetOrCreateList(
            SerializedProperty pArray,
            string strHeader,
            ReorderableList.ElementCallbackDelegate pDraw,
            ReorderableList.ElementHeightCallbackDelegate pHeight,
            ReorderableList.AddCallbackDelegate pOnAdd)
        {
            string strKey = pArray.propertyPath;
            if (m_vListCache.TryGetValue(strKey, out ReorderableList pCached))
            {
                pCached.serializedProperty = pArray;
                return pCached;
            }

            ReorderableList pList = new ReorderableList(
                pArray.serializedObject,
                pArray,
                true,
                true,
                true,
                true);

            pList.drawHeaderCallback = (Rect rc) =>
            {
                EditorGUI.LabelField(rc, strHeader);
            };

            pList.drawElementCallback = (Rect rc, int i, bool bActive, bool bFocus) =>
            {
                m_pCurrentDrawList = pList;
                pDraw(rc, i, bActive, bFocus);
                m_pCurrentDrawList = null;
            };

            pList.elementHeightCallback = (int i) =>
            {
                m_pHeightArrayContext = pList.serializedProperty;
                float fH = pHeight(i);
                m_pHeightArrayContext = null;
                return fH;
            };

            pList.onAddCallback = pOnAdd;

            m_vListCache.Add(strKey, pList);
            return pList;
        }

        static void ShowNamePropertyField(Rect rcPosition, SerializedProperty spTarget)
        {
            Rect rcLabel = rcPosition;
            rcLabel.width *= 0.45f;
            Rect rcField = rcPosition;
            rcField.x = rcLabel.x + rcLabel.width;
            rcField.width = rcPosition.width - rcLabel.width;
            EditorGUI.LabelField(rcLabel, spTarget.displayName);
            EditorGUI.PropertyField(rcField, spTarget, GUIContent.none);
        }

        #endregion
    }
}
