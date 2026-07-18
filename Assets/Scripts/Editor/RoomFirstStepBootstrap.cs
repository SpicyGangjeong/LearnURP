#if UNITY_EDITOR
using Logic.Room;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UI;
using View.UI;

namespace EditorTools
{
    public static class RoomFirstStepBootstrap
    {
        const string s_strPrefabFolder = "Assets/Prefab/Room";
        const string s_strSOFolder = "Assets/ScriptableObjects/Room";
        const string s_strButtonPrefabPath = "Assets/Prefab/UI/RoomButton.prefab";
        const string s_strCombatPrefabPath = "Assets/Prefab/Room/Room_Combat_01.prefab";
        const string s_strRestPrefabPath = "Assets/Prefab/Room/Room_Rest_01.prefab";
        const string s_strEventPrefabPath = "Assets/Prefab/Room/Room_Event_01.prefab";
        const string s_strCombatSOPath = "Assets/ScriptableObjects/Room/RoomData_Combat_01.asset";
        const string s_strEventSOPath = "Assets/ScriptableObjects/Room/RoomData_Event_01.asset";
        const string s_strRestSOPath = "Assets/ScriptableObjects/Room/RoomData_Rest_01.asset";

        [MenuItem("Tools/Room/Bootstrap First Step Assets")]
        public static void Bootstrap()
        {
            BootstrapFinal();
        }

        [MenuItem("Tools/Room/Bootstrap Final Step Assets")]
        public static void BootstrapFinal()
        {
            EnsureFolder("Assets/Prefab");
            EnsureFolder(s_strPrefabFolder);
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder(s_strSOFolder);

            CreateRoomPrefab(s_strCombatPrefabPath, "Room_Combat_01", new Color(0.7f, 0.2f, 0.2f, 1f));
            CreateRoomPrefab(s_strEventPrefabPath, "Room_Event_01", new Color(0.55f, 0.35f, 0.75f, 1f));
            CreateRoomPrefab(s_strRestPrefabPath, "Room_Rest_01", new Color(0.2f, 0.5f, 0.7f, 1f));
            GameObject pButton = CreateRoomButtonPrefab(s_strButtonPrefabPath);

            RegisterAddressable(s_strCombatPrefabPath, Defines.Constants.s_strRoomCombat01);
            RegisterAddressable(s_strEventPrefabPath, Defines.Constants.s_strRoomEvent01);
            RegisterAddressable(s_strRestPrefabPath, Defines.Constants.s_strRoomRest01);

            RoomDataSO pCombatSO = CreateOrLoadRoomSO(s_strCombatSOPath);
            pCombatSO.m_ScriptedObject = new RoomInformation
            {
                strName = "Combat Arena",
                iID = 0,
                eType = Defines.Enums.RoomType.COMBAT,
                vConnectedRoomIDs = new List<int> { 1 },
                strPrefabKey = Defines.Constants.s_strRoomCombat01,
                m_pActivation = new RoomActivationCondition
                {
                    m_eKind = RoomActivationKind.NONE,
                    m_strKey = string.Empty,
                },
                m_pCompletion = new RoomCompletionCondition
                {
                    m_eKind = RoomCompletionKind.ALL_MONSTERS_DEFEATED,
                    m_strKey = string.Empty,
                },
            };
            EditorUtility.SetDirty(pCombatSO);

            RoomDataSO pEventSO = CreateOrLoadRoomSO(s_strEventSOPath);
            pEventSO.m_ScriptedObject = new RoomInformation
            {
                strName = "Chest Event",
                iID = 1,
                eType = Defines.Enums.RoomType.EVENT,
                vConnectedRoomIDs = new List<int> { 2 },
                strPrefabKey = Defines.Constants.s_strRoomEvent01,
                m_pActivation = new RoomActivationCondition
                {
                    m_eKind = RoomActivationKind.NONE,
                    m_strKey = string.Empty,
                },
                m_pCompletion = new RoomCompletionCondition
                {
                    m_eKind = RoomCompletionKind.EVENT_TRIGGER,
                    m_strKey = Defines.Constants.s_strRoomEventChest,
                },
            };
            EditorUtility.SetDirty(pEventSO);

            RoomDataSO pRestSO = CreateOrLoadRoomSO(s_strRestSOPath);
            pRestSO.m_ScriptedObject = new RoomInformation
            {
                strName = "Rest Room",
                iID = 2,
                eType = Defines.Enums.RoomType.REST,
                vConnectedRoomIDs = new List<int>(),
                strPrefabKey = Defines.Constants.s_strRoomRest01,
                m_pActivation = new RoomActivationCondition
                {
                    m_eKind = RoomActivationKind.FLAG,
                    m_strKey = Defines.Constants.s_strRoomFlagBossGate,
                },
                m_pCompletion = new RoomCompletionCondition
                {
                    m_eKind = RoomCompletionKind.NONE,
                    m_strKey = string.Empty,
                },
            };
            EditorUtility.SetDirty(pRestSO);

            WireRoomBoardPrefab(pButton, pCombatSO, pEventSO, pRestSO);
            EnsureRoomBoardOnGamePlayCanvas();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Room Final Step assets bootstrapped. Enter Play from Main Menu Start.");
        }

        static void EnsureRoomBoardOnGamePlayCanvas()
        {
            string strCanvasPath = "Assets/Prefab/UI/GamePlayCanvas.prefab";
            string strBoardPath = "Assets/Prefab/UI/RoomBoard.prefab";
            GameObject pCanvasRoot = PrefabUtility.LoadPrefabContents(strCanvasPath);
            RoomBoard pExisting = pCanvasRoot.GetComponentInChildren<RoomBoard>(true);
            if (null != pExisting)
            {
                SerializedObject pCanvasSO = new SerializedObject(pCanvasRoot.GetComponent<GamePlayCanvas>());
                if (null != pCanvasSO.FindProperty("m_pRoomBoard"))
                {
                    pCanvasSO.FindProperty("m_pRoomBoard").objectReferenceValue = pExisting;
                    pCanvasSO.ApplyModifiedPropertiesWithoutUndo();
                    PrefabUtility.SaveAsPrefabAsset(pCanvasRoot, strCanvasPath);
                }
                PrefabUtility.UnloadPrefabContents(pCanvasRoot);
                return;
            }

            GameObject pBoardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(strBoardPath);
            if (null == pBoardPrefab)
            {
                PrefabUtility.UnloadPrefabContents(pCanvasRoot);
                Debug.LogError("RoomBoard prefab missing.");
                return;
            }

            GameObject pBoardInstance = (GameObject)PrefabUtility.InstantiatePrefab(pBoardPrefab, pCanvasRoot.transform);
            pBoardInstance.name = "RoomBoard";
            RectTransform pRect = pBoardInstance.GetComponent<RectTransform>();
            if (null != pRect)
            {
                pRect.localScale = Vector3.one;
                pRect.anchorMin = Vector2.zero;
                pRect.anchorMax = Vector2.one;
                pRect.offsetMin = Vector2.zero;
                pRect.offsetMax = Vector2.zero;
            }

            Canvas pNestedCanvas = pBoardInstance.GetComponent<Canvas>();
            if (null != pNestedCanvas)
            {
                Object.DestroyImmediate(pNestedCanvas);
            }
            CanvasScaler pScaler = pBoardInstance.GetComponent<CanvasScaler>();
            if (null != pScaler)
            {
                Object.DestroyImmediate(pScaler);
            }
            GraphicRaycaster pRaycaster = pBoardInstance.GetComponent<GraphicRaycaster>();
            if (null != pRaycaster)
            {
                Object.DestroyImmediate(pRaycaster);
            }

            GamePlayCanvas pCanvas = pCanvasRoot.GetComponent<GamePlayCanvas>();
            if (null != pCanvas)
            {
                SerializedObject pCanvasSO = new SerializedObject(pCanvas);
                pCanvasSO.FindProperty("m_pRoomBoard").objectReferenceValue = pBoardInstance.GetComponent<RoomBoard>();
                pCanvasSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(pCanvasRoot, strCanvasPath);
            PrefabUtility.UnloadPrefabContents(pCanvasRoot);
        }

        static void EnsureFolder(string strPath)
        {
            if (true == AssetDatabase.IsValidFolder(strPath))
            {
                return;
            }
            string strParent = Path.GetDirectoryName(strPath).Replace("\\", "/");
            string strName = Path.GetFileName(strPath);
            if (false == AssetDatabase.IsValidFolder(strParent))
            {
                EnsureFolder(strParent);
            }
            AssetDatabase.CreateFolder(strParent, strName);
        }

        static GameObject CreateRoomPrefab(string strPath, string strName, Color pColor)
        {
            GameObject pRoot = new GameObject(strName);
            RoomLayout pLayout = pRoot.AddComponent<RoomLayout>();

            GameObject pFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pFloor.name = "Floor";
            pFloor.transform.SetParent(pRoot.transform, false);
            Renderer pRenderer = pFloor.GetComponent<Renderer>();
            if (null != pRenderer && null != pRenderer.sharedMaterial)
            {
                Material pMat = new Material(pRenderer.sharedMaterial);
                pMat.color = pColor;
                pRenderer.sharedMaterial = pMat;
            }

            Transform pCameraSlot = new GameObject("CameraSlot").transform;
            pCameraSlot.SetParent(pRoot.transform, false);
            pCameraSlot.localPosition = new Vector3(0f, 5f, -8f);

            Transform pPlayerSlot = new GameObject("PlayerSlot").transform;
            pPlayerSlot.SetParent(pRoot.transform, false);
            pPlayerSlot.localPosition = new Vector3(0f, 0f, -2f);

            Transform pMonsterSlot = new GameObject("MonsterSlot_0").transform;
            pMonsterSlot.SetParent(pRoot.transform, false);
            pMonsterSlot.localPosition = new Vector3(0f, 0f, 2f);

            SerializedObject pSO = new SerializedObject(pLayout);
            pSO.FindProperty("m_pCameraSlot").objectReferenceValue = pCameraSlot;
            pSO.FindProperty("m_pPlayerSlot").objectReferenceValue = pPlayerSlot;
            SerializedProperty pMonsterList = pSO.FindProperty("m_vMonsterSlots");
            pMonsterList.arraySize = 1;
            pMonsterList.GetArrayElementAtIndex(0).objectReferenceValue = pMonsterSlot;
            pSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject pPrefab = PrefabUtility.SaveAsPrefabAsset(pRoot, strPath);
            Object.DestroyImmediate(pRoot);
            return pPrefab;
        }

        static GameObject CreateRoomButtonPrefab(string strPath)
        {
            GameObject pRoot = new GameObject("RoomButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(RoomButton));
            RectTransform pRect = pRoot.GetComponent<RectTransform>();
            pRect.sizeDelta = new Vector2(240f, 64f);

            GameObject pLabelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            pLabelGO.transform.SetParent(pRoot.transform, false);
            RectTransform pLabelRect = pLabelGO.GetComponent<RectTransform>();
            pLabelRect.anchorMin = Vector2.zero;
            pLabelRect.anchorMax = Vector2.one;
            pLabelRect.offsetMin = Vector2.zero;
            pLabelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI pLabel = pLabelGO.GetComponent<TextMeshProUGUI>();
            pLabel.text = "Room";
            pLabel.alignment = TextAlignmentOptions.Center;
            pLabel.fontSize = 24f;

            RoomButton pRoomButton = pRoot.GetComponent<RoomButton>();
            SerializedObject pSO = new SerializedObject(pRoomButton);
            pSO.FindProperty("m_pButton").objectReferenceValue = pRoot.GetComponent<Button>();
            pSO.FindProperty("m_pLabel").objectReferenceValue = pLabel;
            pSO.ApplyModifiedPropertiesWithoutUndo();

            GameObject pPrefab = PrefabUtility.SaveAsPrefabAsset(pRoot, strPath);
            Object.DestroyImmediate(pRoot);
            return pPrefab;
        }

        static RoomDataSO CreateOrLoadRoomSO(string strPath)
        {
            RoomDataSO pExisting = AssetDatabase.LoadAssetAtPath<RoomDataSO>(strPath);
            if (null != pExisting)
            {
                return pExisting;
            }
            RoomDataSO pCreated = ScriptableObject.CreateInstance<RoomDataSO>();
            AssetDatabase.CreateAsset(pCreated, strPath);
            return pCreated;
        }

        static void RegisterAddressable(string strAssetPath, string strAddress)
        {
            AddressableAssetSettings pSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (null == pSettings)
            {
                Debug.LogError("Addressable settings missing. Skip address registration for " + strAddress);
                return;
            }

            string strGuid = AssetDatabase.AssetPathToGUID(strAssetPath);
            AddressableAssetEntry pEntry = pSettings.FindAssetEntry(strGuid);
            if (null == pEntry)
            {
                AddressableAssetGroup pGroup = pSettings.DefaultGroup;
                pEntry = pSettings.CreateOrMoveEntry(strGuid, pGroup);
            }
            pEntry.address = strAddress;
            EditorUtility.SetDirty(pSettings);
        }

        static void WireRoomBoardPrefab(
            GameObject pButtonPrefab,
            RoomDataSO pCombatSO,
            RoomDataSO pEventSO,
            RoomDataSO pRestSO)
        {
            string strBoardPath = "Assets/Prefab/UI/RoomBoard.prefab";
            GameObject pBoardRoot = PrefabUtility.LoadPrefabContents(strBoardPath);
            RoomBoard pBoard = pBoardRoot.GetComponent<RoomBoard>();
            if (null == pBoard)
            {
                PrefabUtility.UnloadPrefabContents(pBoardRoot);
                Debug.LogError("RoomBoard component missing on prefab.");
                return;
            }

            Transform pButtonParent = pBoardRoot.transform.Find("BackBoard");
            if (null == pButtonParent)
            {
                pButtonParent = pBoardRoot.transform;
            }

            Transform pExitTransform = pBoardRoot.transform.Find("ExitRoomButton");
            if (null == pExitTransform)
            {
                GameObject pExitGO = new GameObject("ExitRoomButton", typeof(RectTransform), typeof(Image), typeof(Button));
                pExitGO.transform.SetParent(pBoardRoot.transform, false);
                RectTransform pExitRect = pExitGO.GetComponent<RectTransform>();
                pExitRect.anchorMin = new Vector2(1f, 0f);
                pExitRect.anchorMax = new Vector2(1f, 0f);
                pExitRect.pivot = new Vector2(1f, 0f);
                pExitRect.anchoredPosition = new Vector2(-40f, 40f);
                pExitRect.sizeDelta = new Vector2(160f, 48f);

                GameObject pExitLabelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
                pExitLabelGO.transform.SetParent(pExitGO.transform, false);
                RectTransform pExitLabelRect = pExitLabelGO.GetComponent<RectTransform>();
                pExitLabelRect.anchorMin = Vector2.zero;
                pExitLabelRect.anchorMax = Vector2.one;
                pExitLabelRect.offsetMin = Vector2.zero;
                pExitLabelRect.offsetMax = Vector2.zero;
                TextMeshProUGUI pExitLabel = pExitLabelGO.GetComponent<TextMeshProUGUI>();
                pExitLabel.text = "Exit Room";
                pExitLabel.alignment = TextAlignmentOptions.Center;
                pExitLabel.fontSize = 22f;

                Button pExitButton = pExitGO.GetComponent<Button>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(pExitButton.onClick, pBoard.RequestExitRoom);
                pExitTransform = pExitGO.transform;
            }

            EnsureDebugButtons(pBoardRoot, pBoard);

            SerializedObject pBoardSO = new SerializedObject(pBoard);
            pBoardSO.FindProperty("m_pRoomButtonPrefab").objectReferenceValue = pButtonPrefab.GetComponent<View.UI.RoomButton>();
            pBoardSO.FindProperty("m_pButtonParent").objectReferenceValue = pButtonParent;
            pBoardSO.FindProperty("m_pExitButtonRoot").objectReferenceValue = pExitTransform.gameObject;
            pBoardSO.FindProperty("m_iStartRoomID").intValue = 0;
            SerializedProperty pList = pBoardSO.FindProperty("m_vRoomDataSOs");
            pList.arraySize = 3;
            pList.GetArrayElementAtIndex(0).objectReferenceValue = pCombatSO;
            pList.GetArrayElementAtIndex(1).objectReferenceValue = pEventSO;
            pList.GetArrayElementAtIndex(2).objectReferenceValue = pRestSO;
            pBoardSO.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(pBoardRoot, strBoardPath);
            PrefabUtility.UnloadPrefabContents(pBoardRoot);
        }

        static void EnsureDebugButtons(GameObject pBoardRoot, RoomBoard pBoard)
        {
            Transform pDebugRoot = pBoardRoot.transform.Find("DebugButtons");
            if (null == pDebugRoot)
            {
                GameObject pDebugGO = new GameObject("DebugButtons", typeof(RectTransform));
                pDebugGO.transform.SetParent(pBoardRoot.transform, false);
                RectTransform pDebugRect = pDebugGO.GetComponent<RectTransform>();
                pDebugRect.anchorMin = new Vector2(0f, 0f);
                pDebugRect.anchorMax = new Vector2(0f, 0f);
                pDebugRect.pivot = new Vector2(0f, 0f);
                pDebugRect.anchoredPosition = new Vector2(40f, 40f);
                pDebugRect.sizeDelta = new Vector2(200f, 160f);
                pDebugRoot = pDebugGO.transform;
            }

            CreateDebugButton(pDebugRoot, pBoard, "Dbg_KillAll", "Kill All", 0f, nameof(RoomBoard.DebugNotifyAllMonstersDefeated));
            CreateDebugButton(pDebugRoot, pBoard, "Dbg_Chest", "Chest Event", -56f, nameof(RoomBoard.DebugNotifyChestEvent));
            CreateDebugButton(pDebugRoot, pBoard, "Dbg_BossGate", "Boss Gate", -112f, nameof(RoomBoard.DebugNotifyBossGateFlag));
        }

        static void CreateDebugButton(
            Transform pParent,
            RoomBoard pBoard,
            string strName,
            string strLabel,
            float fY,
            string strMethodName)
        {
            Transform pExisting = pParent.Find(strName);
            if (null != pExisting)
            {
                return;
            }

            GameObject pGO = new GameObject(strName, typeof(RectTransform), typeof(Image), typeof(Button));
            pGO.transform.SetParent(pParent, false);
            RectTransform pRect = pGO.GetComponent<RectTransform>();
            pRect.anchorMin = new Vector2(0f, 1f);
            pRect.anchorMax = new Vector2(0f, 1f);
            pRect.pivot = new Vector2(0f, 1f);
            pRect.anchoredPosition = new Vector2(0f, fY);
            pRect.sizeDelta = new Vector2(180f, 48f);

            GameObject pLabelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            pLabelGO.transform.SetParent(pGO.transform, false);
            RectTransform pLabelRect = pLabelGO.GetComponent<RectTransform>();
            pLabelRect.anchorMin = Vector2.zero;
            pLabelRect.anchorMax = Vector2.one;
            pLabelRect.offsetMin = Vector2.zero;
            pLabelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI pTmp = pLabelGO.GetComponent<TextMeshProUGUI>();
            pTmp.text = strLabel;
            pTmp.alignment = TextAlignmentOptions.Center;
            pTmp.fontSize = 18f;

            Button pButton = pGO.GetComponent<Button>();
            switch (strMethodName)
            {
                case nameof(RoomBoard.DebugNotifyAllMonstersDefeated):
                    UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(pButton.onClick, pBoard.DebugNotifyAllMonstersDefeated);
                    break;
                case nameof(RoomBoard.DebugNotifyChestEvent):
                    UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(pButton.onClick, pBoard.DebugNotifyChestEvent);
                    break;
                case nameof(RoomBoard.DebugNotifyBossGateFlag):
                    UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(pButton.onClick, pBoard.DebugNotifyBossGateFlag);
                    break;
            }
        }
    }
}
#endif
