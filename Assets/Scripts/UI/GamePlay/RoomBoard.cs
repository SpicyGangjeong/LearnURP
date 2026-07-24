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

            GameInstance m_pGameInstance = null;
            RoomManager m_pRoomManager = null;
            readonly List<RoomButton> m_vButtons = new List<RoomButton>();

            void Awake()
            {
                m_pGameInstance = GameInstance.Instance;
                m_pRoomManager = m_pGameInstance.Rooms;
                if (null == m_pRoomRoot)
                {
                    GameObject pRootObject = GameObject.Find(Defines.Constants.s_strRoomRoot);
                    if (null == pRootObject)
                    {
                        pRootObject = new GameObject(Defines.Constants.s_strRoomRoot);
                    }
                    m_pRoomRoot = pRootObject.transform;
                }
                m_pRoomManager.SetRoomRoot(m_pRoomRoot);
                m_pRoomManager.m_pOnMapGenerated += OnMapGenerated;
                m_pRoomManager.m_pOnRoomEntered += OnRoomEntered;
                m_pRoomManager.m_pOnRoomExited += OnRoomExited;
                m_pRoomManager.m_pOnRoomActivated += OnRoomActivated;
                m_pRoomManager.m_pOnRoomCompleted += OnRoomCompleted;

                RefreshExitButton();

                m_pRoomManager.GenerateMap(m_vRoomDataSOs, m_iStartRoomID);
            }

            void OnDestroy()
            {
                m_pRoomManager.m_pOnMapGenerated -= OnMapGenerated;
                m_pRoomManager.m_pOnRoomEntered -= OnRoomEntered;
                m_pRoomManager.m_pOnRoomExited -= OnRoomExited;
                m_pRoomManager.m_pOnRoomActivated -= OnRoomActivated;
                m_pRoomManager.m_pOnRoomCompleted -= OnRoomCompleted;
                ClearButtons();
            }
            public void RequestExitRoom()
            {
                m_pRoomManager.ExitRoom();
            }

            void OnMapGenerated(IReadOnlyList<Logic.Room.Room> vRooms)
            {
                ClearButtons();
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
                m_pButtonParent.gameObject.SetActive(bVisible);
            }

            void SetExitVisible(bool bVisible)
            {
                m_pExitButtonRoot.SetActive(bVisible);
            }

            //public void DebugNotifyAllMonstersDefeated()
            //{
            //    if (null == m_pRoomManager)
            //    {
            //        return;
            //    }
            //    m_pRoomManager.NotifyAllMonstersDefeated();
            //}

            //public void DebugNotifyChestEvent()
            //{
            //    if (null == m_pRoomManager)
            //    {
            //        return;
            //    }
            //    m_pRoomManager.NotifySpecialEvent(Defines.Constants.s_strRoomEventChest);
            //    // TODO: EVENT_TRIGGER 등 — 호출부에서 TryCompleteRoom(CurrentRoom) 연결
            //    if (null != m_pRoomManager.CurrentRoom)
            //    {
            //        m_pRoomManager.TryCompleteRoom(m_pRoomManager.CurrentRoom);
            //    }
            //}

            //public void DebugNotifyBossGateFlag()
            //{
            //    if (null == m_pRoomManager)
            //    {
            //        return;
            //    }
            //    m_pRoomManager.NotifyFlag(Defines.Constants.s_strRoomFlagBossGate);
            //}

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
