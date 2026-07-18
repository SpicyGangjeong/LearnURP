using Core;
using Core.Room;
using Logic.Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    namespace UI
    {
        public class RoomBoard : MonoBehaviour
        {
            [SerializeField]
            RoomButton m_pRoomButtonPrefab = null;
            [SerializeField]
            Transform m_pButtonParent = null;
            [SerializeField]
            Transform m_pRoomRoot = null;
            [SerializeField]
            List<RoomDataSO> m_vRoomDataSOs = new List<RoomDataSO>();
            [SerializeField]
            int m_iStartRoomID = 0;
            [SerializeField]
            GameObject m_pExitButtonRoot = null;

            CGameInstance m_pGameInstance = null;
            RoomManager m_pRoomManager = null;
            readonly List<RoomButton> m_vButtons = new List<RoomButton>();

            void Start()
            {
                m_pGameInstance = CGameInstance.Instance;
                m_pRoomManager = m_pGameInstance.Rooms;
                if (null == m_pRoomManager)
                {
                    throw new System.InvalidOperationException("RoomBoard: RoomManager is null.");
                }

                if (null == m_pRoomRoot)
                {
                    GameObject pRootObject = GameObject.Find(Defines.Constants.s_strRoomRoot);
                    if (null == pRootObject)
                    {
                        pRootObject = new GameObject(Defines.Constants.s_strRoomRoot);
                        pRootObject.transform.SetParent(CGameInstance.Instance.transform);
                    }
                    m_pRoomRoot = pRootObject.transform;
                }

                if (null == m_pButtonParent)
                {
                    m_pButtonParent = transform;
                }

                m_pRoomManager.SetRoomRoot(m_pRoomRoot);
                m_pRoomManager.m_pOnMapGenerated += OnMapGenerated;
                m_pRoomManager.m_pOnRoomEntered += OnRoomEntered;
                m_pRoomManager.m_pOnRoomExited += OnRoomExited;
                m_pRoomManager.m_pOnRoomActivated += OnRoomActivated;
                m_pRoomManager.m_pOnRoomCompleted += OnRoomCompleted;

                RefreshExitButton();
                WireExitButton();

                if (0 == m_vRoomDataSOs.Count)
                {
                    Debug.LogWarning("RoomBoard: m_vRoomDataSOs is empty. Assign RoomDataSO assets in the inspector.");
                    return;
                }

                m_pRoomManager.GenerateMap(m_vRoomDataSOs, m_iStartRoomID);
            }

            void WireExitButton()
            {
                if (null == m_pExitButtonRoot)
                {
                    return;
                }
                Button pExitButton = m_pExitButtonRoot.GetComponent<Button>();
                if (null == pExitButton)
                {
                    pExitButton = m_pExitButtonRoot.GetComponentInChildren<Button>(true);
                }
                if (null == pExitButton)
                {
                    throw new System.InvalidOperationException("RoomBoard: Exit button root has no Button.");
                }
                pExitButton.onClick.RemoveListener(RequestExitRoom);
                pExitButton.onClick.AddListener(RequestExitRoom);
            }

            void OnDestroy()
            {
                if (null != m_pRoomManager)
                {
                    m_pRoomManager.m_pOnMapGenerated -= OnMapGenerated;
                    m_pRoomManager.m_pOnRoomEntered -= OnRoomEntered;
                    m_pRoomManager.m_pOnRoomExited -= OnRoomExited;
                    m_pRoomManager.m_pOnRoomActivated -= OnRoomActivated;
                    m_pRoomManager.m_pOnRoomCompleted -= OnRoomCompleted;
                }
                ClearButtons();
            }

            public void RequestExitRoom()
            {
                if (null == m_pRoomManager)
                {
                    return;
                }
                if (null == m_pRoomManager.CurrentRoom || false == m_pRoomManager.CurrentRoom.Cleared)
                {
                    return;
                }
                m_pRoomManager.ExitRoom();
            }

            void OnMapGenerated(IReadOnlyList<Logic.Room.Room> vRooms)
            {
                ClearButtons();
                if (null == m_pRoomButtonPrefab)
                {
                    throw new System.InvalidOperationException("RoomBoard: m_pRoomButtonPrefab is null.");
                }

                for (int i = 0; i < vRooms.Count; ++i)
                {
                    RoomButton pButton = Instantiate(m_pRoomButtonPrefab, m_pButtonParent);
                    pButton.Bind(vRooms[i]);
                    m_vButtons.Add(pButton);
                }
                RefreshExitButton();
            }

            void OnRoomEntered(Logic.Room.Room pRoom)
            {
                RefreshExitButton();
                RefreshButtons();
            }

            void OnRoomExited(Logic.Room.Room pRoom)
            {
                RefreshExitButton();
                RefreshButtons();
            }

            void OnRoomActivated(Logic.Room.Room pRoom)
            {
                RefreshButtons();
            }

            void OnRoomCompleted(Logic.Room.Room pRoom)
            {
                RefreshExitButton();
                RefreshButtons();
            }

            void RefreshButtons()
            {
                for (int i = 0; i < m_vButtons.Count; ++i)
                {
                    m_vButtons[i].Refresh();
                }
            }

            void ClearButtons()
            {
                for (int i = 0; i < m_vButtons.Count; ++i)
                {
                    if (null != m_vButtons[i])
                    {
                        Destroy(m_vButtons[i].gameObject);
                    }
                }
                m_vButtons.Clear();
            }

            void RefreshExitButton()
            {
                bool bShow = null != m_pRoomManager
                    && null != m_pRoomManager.CurrentRoom
                    && true == m_pRoomManager.CurrentRoom.Cleared;
                SetExitVisible(bShow);
            }

            public void SetMapVisible(bool bVisible)
            {
                if (null != m_pButtonParent)
                {
                    m_pButtonParent.gameObject.SetActive(bVisible);
                }
            }

            void SetExitVisible(bool bVisible)
            {
                if (null != m_pExitButtonRoot)
                {
                    m_pExitButtonRoot.SetActive(bVisible);
                }
            }

            public void DebugNotifyAllMonstersDefeated()
            {
                if (null == m_pRoomManager)
                {
                    return;
                }
                m_pRoomManager.NotifyAllMonstersDefeated();
            }

            public void DebugNotifyChestEvent()
            {
                if (null == m_pRoomManager)
                {
                    return;
                }
                m_pRoomManager.NotifySpecialEvent(Defines.Constants.s_strRoomEventChest);
                // TODO: EVENT_TRIGGER 등 — 호출부에서 TryCompleteRoom(CurrentRoom) 연결
                if (null != m_pRoomManager.CurrentRoom)
                {
                    m_pRoomManager.TryCompleteRoom(m_pRoomManager.CurrentRoom);
                }
            }

            public void DebugNotifyBossGateFlag()
            {
                if (null == m_pRoomManager)
                {
                    return;
                }
                m_pRoomManager.NotifyFlag(Defines.Constants.s_strRoomFlagBossGate);
            }

//#if UNITY_EDITOR
//            void OnGUI()
//            {
//                if (false == Application.isPlaying || null == m_pRoomManager)
//                {
//                    return;
//                }
//                const float fWidth = 160f;
//                const float fHeight = 28f;
//                float fX = 12f;
//                float fY = Screen.height - 120f;
//                if (true == GUI.Button(new Rect(fX, fY, fWidth, fHeight), "Dbg Kill All"))
//                {
//                    DebugNotifyAllMonstersDefeated();
//                }
//                if (true == GUI.Button(new Rect(fX, fY + 32f, fWidth, fHeight), "Dbg Chest Event"))
//                {
//                    DebugNotifyChestEvent();
//                }
//                if (true == GUI.Button(new Rect(fX, fY + 64f, fWidth, fHeight), "Dbg Boss Gate"))
//                {
//                    DebugNotifyBossGateFlag();
//                }
//            }
//#endif
        }
    }
}
