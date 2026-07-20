using Core;
using Core.Client;
using Cysharp.Threading.Tasks;
using Logic;
using Logic.Room;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    namespace Room
    {
        public delegate void MapGenerated(IReadOnlyList<Logic.Room.Room> vRooms);
        public delegate void RoomEntered(Logic.Room.Room pRoom);
        public delegate void RoomExited(Logic.Room.Room pRoom);
        public delegate void RoomActivated(Logic.Room.Room pRoom);
        public delegate void RoomCompleted(Logic.Room.Room pRoom);

        [Serializable]
        public class RoomManager
        {
            public event MapGenerated m_pOnMapGenerated;
            public event RoomEntered m_pOnRoomEntered;
            public event RoomExited m_pOnRoomExited;
            public event RoomActivated m_pOnRoomActivated;
            public event RoomCompleted m_pOnRoomCompleted;

            Transform m_pRoomRoot = null;
            List<Logic.Room.Room> m_vRooms = new List<Logic.Room.Room>();
            Logic.Room.Room m_pCurrentRoom = null;
            RoomGenerator m_pGenerator = new RoomGenerator();
            int m_iStartRoomID = 0;
            readonly HashSet<string> m_vRaisedFlags = new HashSet<string>();
            readonly HashSet<string> m_vRaisedEvents = new HashSet<string>();

            public Logic.Room.Room CurrentRoom => m_pCurrentRoom;
            public IReadOnlyList<Logic.Room.Room> Rooms => m_vRooms;

            public void Initialize()
            {
                m_pOnMapGenerated += OnMapGeneratedEmpty;
                m_pOnRoomEntered += Defines.Helpers.EmptyEvent;
                m_pOnRoomExited += Defines.Helpers.EmptyEvent;
                m_pOnRoomActivated += Defines.Helpers.EmptyEvent;
                m_pOnRoomCompleted += Defines.Helpers.EmptyEvent;
            }

            static void OnMapGeneratedEmpty(IReadOnlyList<Logic.Room.Room> vRooms)
            {
            }

            public void SetRoomRoot(Transform pRoot)
            {
                if (null == pRoot)
                {
                    throw new ArgumentNullException(nameof(pRoot));
                }
                m_pRoomRoot = pRoot;
            }

            public void GenerateMap(IReadOnlyList<RoomDataSO> vSOs, int iStartRoomID)
            {
                if (null == vSOs)
                {
                    throw new ArgumentNullException(nameof(vSOs));
                }
                if (0 == vSOs.Count)
                {
                    throw new InvalidOperationException("RoomManager.GenerateMap requires at least one RoomDataSO.");
                }

                if (null != m_pCurrentRoom)
                {
                    ForceExitRoom();
                }

                m_vRaisedFlags.Clear();
                m_vRaisedEvents.Clear();
                m_iStartRoomID = iStartRoomID;

                List<RoomData> vData = new List<RoomData>(vSOs.Count);
                for (int i = 0; i < vSOs.Count; ++i)
                {
                    RoomDataSO pSO = vSOs[i];
                    if (null == pSO)
                    {
                        throw new InvalidOperationException($"RoomManager.GenerateMap RoomDataSO at index {i} is null.");
                    }
                    RoomData pData = pSO.Instantiate();
                    if (null == pData)
                    {
                        throw new InvalidOperationException($"RoomManager.GenerateMap failed to instantiate RoomDataSO at index {i}.");
                    }
                    vData.Add(pData);
                }

                m_vRooms = m_pGenerator.Build(vData, iStartRoomID);

                for (int i = 0; i < m_vRooms.Count; ++i)
                {
                    if (m_vRooms[i].Data.ID == m_iStartRoomID)
                    {
                        TryActivateRoom(m_vRooms[i]);
                        break;
                    }
                }

                m_pOnMapGenerated(m_vRooms);
            }

            public async UniTask EnterRoom(Logic.Room.Room pRoom)
            {
                if (null == pRoom)
                {
                    throw new ArgumentNullException(nameof(pRoom));
                }
                if (false == pRoom.Active)
                {
                    throw new InvalidOperationException($"RoomManager.EnterRoom room {pRoom.Data.ID} is not active.");
                }
                if (true == pRoom.Cleared)
                {
                    throw new InvalidOperationException($"RoomManager.EnterRoom room {pRoom.Data.ID} is already cleared.");
                }
                if (null == m_pRoomRoot)
                {
                    throw new InvalidOperationException("RoomManager.EnterRoom RoomRoot is not set.");
                }

                if (null != m_pCurrentRoom)
                {
                    if (false == m_pCurrentRoom.Cleared)
                    {
                        throw new InvalidOperationException(
                            $"RoomManager.EnterRoom cannot leave uncleared room {m_pCurrentRoom.Data.ID}.");
                    }
                    ExitRoom();
                }

                string strPrefabKey = pRoom.Data.PrefabKey;
                if (true == string.IsNullOrEmpty(strPrefabKey))
                {
                    throw new InvalidOperationException($"RoomManager.EnterRoom room {pRoom.Data.ID} has empty PrefabKey.");
                }

                UnityEngine.Object pPrototype = await CGameInstance.Instance.LoadAddressAssetAsync(strPrefabKey);
                if (null == pPrototype)
                {
                    throw new InvalidOperationException($"RoomManager.EnterRoom failed to load prefab '{strPrefabKey}'.");
                }

                GameObject pPrefab = pPrototype as GameObject;
                if (null == pPrefab)
                {
                    throw new InvalidOperationException($"RoomManager.EnterRoom asset '{strPrefabKey}' is not a GameObject.");
                }

                GameObject pInstance = UnityEngine.Object.Instantiate(pPrefab, m_pRoomRoot);
                RoomLayout pLayout = pInstance.GetComponent<RoomLayout>();
                if (null == pLayout)
                {
                    UnityEngine.Object.Destroy(pInstance);
                    throw new InvalidOperationException(
                        $"RoomManager.EnterRoom prefab '{strPrefabKey}' missing RoomLayout.");
                }

                pRoom.BindSlots(pLayout);
                ApplySlotPlacement(pRoom);
                pRoom.SetVisited(true);
                m_pCurrentRoom = pRoom;

                pRoom.OnRoomEnter();
                m_pOnRoomEntered(pRoom);

                if (null != pRoom.Completion && RoomCompletionKind.NONE == pRoom.Completion.m_eKind)
                {
                    TryCompleteRoom(pRoom);
                }
            }

            public bool ExitRoom()
            {
                if (null == m_pCurrentRoom)
                {
                    return false;
                }
                if (false == m_pCurrentRoom.Cleared)
                {
                    throw new InvalidOperationException(
                        $"RoomManager.ExitRoom room {m_pCurrentRoom.Data.ID} is not cleared.");
                }

                ForceExitRoom();
                return true;
            }

            void ForceExitRoom()
            {
                if (null == m_pCurrentRoom)
                {
                    return;
                }

                Logic.Room.Room pRoom = m_pCurrentRoom;
                pRoom.OnRoomExit();
                m_pOnRoomExited(pRoom);

                RoomLayout pLayout = pRoom.LayoutInstance;
                pRoom.UnbindSlots();
                if (null != pLayout)
                {
                    UnityEngine.Object.Destroy(pLayout.gameObject);
                }

                m_pCurrentRoom = null;
            }

            public void NotifyFlag(string strKey)
            {
                if (true == string.IsNullOrEmpty(strKey))
                {
                    throw new ArgumentException("RoomManager.NotifyFlag key is empty.", nameof(strKey));
                }
                m_vRaisedFlags.Add(strKey);
                // TODO: EVENT_TRIGGER 등 — 호출부에서 TryCompleteRoom(CurrentRoom) 연결
                ReevaluateInactiveRooms();
            }

            public void NotifySpecialEvent(string strKey)
            {
                if (true == string.IsNullOrEmpty(strKey))
                {
                    throw new ArgumentException("RoomManager.NotifySpecialEvent key is empty.", nameof(strKey));
                }
                m_vRaisedEvents.Add(strKey);
                // TODO: EVENT_TRIGGER 등 — 호출부에서 TryCompleteRoom(CurrentRoom) 연결
                ReevaluateInactiveRooms();
            }

            public void NotifyAllMonstersDefeated()
            {
                if (null == m_pCurrentRoom)
                {
                    return;
                }
                m_pCurrentRoom.SetAllMonstersDefeated(true);
                TryCompleteRoom(m_pCurrentRoom);
            }

            public bool TryCompleteRoom(Logic.Room.Room pRoom)
            {
                if (null == pRoom)
                {
                    throw new ArgumentNullException(nameof(pRoom));
                }
                if (true == pRoom.Cleared)
                {
                    return true;
                }
                if (false == IsCompletionSatisfied(pRoom))
                {
                    return false;
                }

                pRoom.SetCleared(true);
                m_pOnRoomCompleted(pRoom);

                IReadOnlyList<Logic.Room.Room> vConnected = pRoom.ConnectedRooms;
                for (int i = 0; i < vConnected.Count; ++i)
                {
                    TryActivateRoom(vConnected[i]);
                }
                return true;
            }

            public void TryActivateRoom(Logic.Room.Room pRoom)
            {
                if (null == pRoom)
                {
                    throw new ArgumentNullException(nameof(pRoom));
                }
                if (true == pRoom.Active)
                {
                    return;
                }
                if (false == IsActivationSatisfied(pRoom))
                {
                    return;
                }

                pRoom.SetActive(true);
                m_pOnRoomActivated(pRoom);
            }

            void ReevaluateInactiveRooms()
            {
                for (int i = 0; i < m_vRooms.Count; ++i)
                {
                    Logic.Room.Room pRoom = m_vRooms[i];
                    if (false == pRoom.Active)
                    {
                        TryActivateRoom(pRoom);
                    }
                }
            }

            bool IsActivationSatisfied(Logic.Room.Room pRoom)
            {
                RoomActivationCondition pActivation = pRoom.Activation;
                if (null == pActivation)
                {
                    return IsOpenCandidate(pRoom);
                }

                switch (pActivation.m_eKind)
                {
                    case RoomActivationKind.NONE:
                        return IsOpenCandidate(pRoom);
                    case RoomActivationKind.FLAG:
                        return false == string.IsNullOrEmpty(pActivation.m_strKey)
                            && true == m_vRaisedFlags.Contains(pActivation.m_strKey);
                    case RoomActivationKind.SPECIAL_EVENT:
                        return false == string.IsNullOrEmpty(pActivation.m_strKey)
                            && true == m_vRaisedEvents.Contains(pActivation.m_strKey);
                    default:
                        return false;
                }
            }

            bool IsOpenCandidate(Logic.Room.Room pRoom)
            {
                if (pRoom.Data.ID == m_iStartRoomID)
                {
                    return true;
                }
                return HasClearedNeighbor(pRoom);
            }

            bool HasClearedNeighbor(Logic.Room.Room pRoom)
            {
                IReadOnlyList<Logic.Room.Room> vConnected = pRoom.ConnectedRooms;
                for (int i = 0; i < vConnected.Count; ++i)
                {
                    if (true == vConnected[i].Cleared)
                    {
                        return true;
                    }
                }

                // Also accept being listed as a connection from a cleared room (one-way edges).
                for (int i = 0; i < m_vRooms.Count; ++i)
                {
                    Logic.Room.Room pOther = m_vRooms[i];
                    if (false == pOther.Cleared)
                    {
                        continue;
                    }
                    IReadOnlyList<Logic.Room.Room> vOtherConnected = pOther.ConnectedRooms;
                    for (int j = 0; j < vOtherConnected.Count; ++j)
                    {
                        if (vOtherConnected[j] == pRoom)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            bool IsCompletionSatisfied(Logic.Room.Room pRoom)
            {
                RoomCompletionCondition pCompletion = pRoom.Completion;
                if (null == pCompletion)
                {
                    return true;
                }

                switch (pCompletion.m_eKind)
                {
                    case RoomCompletionKind.NONE:
                        return true;
                    case RoomCompletionKind.ALL_MONSTERS_DEFEATED:
                        return true == pRoom.AllMonstersDefeated;
                    case RoomCompletionKind.EVENT_TRIGGER:
                        return false == string.IsNullOrEmpty(pCompletion.m_strKey)
                            && true == m_vRaisedEvents.Contains(pCompletion.m_strKey);
                    default:
                        return false;
                }
            }

            void ApplySlotPlacement(Logic.Room.Room pRoom)
            {
                CharacterSlot pPlayerSlot = pRoom.PlayerSlot;
                if (null == pPlayerSlot)
                {
                    throw new InvalidOperationException(
                        $"RoomManager.ApplySlotPlacement room {pRoom.Data.ID} has no PlayerSlot.");
                }

                CPlayable pPlayerUnit = CInfoInstance.Instance.PlayerInstance.PlayerUnit;
                if (null != pPlayerUnit)
                {
                    GameObject pPlayerObject = pPlayerUnit.GetTargetObject();
                    if (null == pPlayerObject)
                    {
                        throw new InvalidOperationException(
                            "RoomManager.ApplySlotPlacement player GameObject is null.");
                    }
                    pPlayerSlot.SetCurrentUnit(pPlayerUnit);
                }

                CharacterSlot pCameraSlot = pRoom.CameraSlot;
                if (null != pCameraSlot)
                {
                    Camera pCamera = CGameInstance.Instance.Main_Camera;
                    if (null != pCamera)
                    {
                        TransformHandle hTransform = pCameraSlot.transformHandle;
                        pCamera.transformHandle.SetParent(hTransform);
                        pCamera.transform.SetPositionAndRotation(hTransform.position, hTransform.rotation);
                    }
                }
            }
        }
    }
}
