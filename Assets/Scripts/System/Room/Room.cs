using Defines;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    namespace Room
    {
        public delegate void RoomEnter(Room pRoom);
        public delegate void RoomExit(Room pRoom);

        [Serializable]
        public class Room
        {
            [SerializeField]
            RoomData m_pData = null;
            [NonSerialized]
            List<Room> m_vConnectedRooms = new List<Room>();
            bool m_bActive = false;
            bool m_bVisited = false;
            bool m_bCleared = false;
            bool m_bAllMonstersDefeated = false;
            RoomLayout m_pLayoutInstance = null;
            CharacterSlot m_pCameraSlot = null;
            CharacterSlot m_pPlayerSlot = null;
            List<CharacterSlot> m_vMonsterSlots = new List<CharacterSlot>();

            public event RoomEnter m_pOnRoomEnter;
            public event RoomExit m_pOnRoomExit;

            public RoomData Data => m_pData;
            public IReadOnlyList<Room> ConnectedRooms => m_vConnectedRooms;
            public bool Active => m_bActive;
            public bool Visited => m_bVisited;
            public bool Cleared => m_bCleared;
            public bool AllMonstersDefeated => m_bAllMonstersDefeated;
            public RoomActivationCondition Activation => m_pData.Activation;
            public RoomCompletionCondition Completion => m_pData.Completion;
            public RoomLayout LayoutInstance => m_pLayoutInstance;
            public CharacterSlot CameraSlot => m_pCameraSlot;
            public CharacterSlot PlayerSlot => m_pPlayerSlot;
            public IReadOnlyList<CharacterSlot> MonsterSlots => m_vMonsterSlots;

            public Room(RoomData pData)
            {
                if (null == pData)
                {
                    throw new ArgumentNullException(nameof(pData));
                }
                m_pData = pData;
                m_pOnRoomEnter += Helpers.EmptyEvent;
                m_pOnRoomExit += Helpers.EmptyEvent;
            }

            public void SetActive(bool bActive)
            {
                m_bActive = bActive;
            }

            public void SetVisited(bool bVisited)
            {
                m_bVisited = bVisited;
            }

            public void SetCleared(bool bCleared)
            {
                m_bCleared = bCleared;
            }

            public void SetAllMonstersDefeated(bool bDefeated)
            {
                m_bAllMonstersDefeated = bDefeated;
            }

            public void SetConnectedRooms(List<Room> vConnectedRooms)
            {
                if (null == vConnectedRooms)
                {
                    throw new ArgumentNullException(nameof(vConnectedRooms));
                }
                m_vConnectedRooms = vConnectedRooms;
            }

            public void BindSlots(RoomLayout pLayout)
            {
                if (null == pLayout)
                {
                    throw new ArgumentNullException(nameof(pLayout));
                }
                m_pLayoutInstance = pLayout;
                m_pCameraSlot = pLayout.CameraSlot;
                m_pPlayerSlot = pLayout.PlayerSlot;
                m_vMonsterSlots = new List<CharacterSlot>(pLayout.MonsterSlots);
            }

            public void UnbindSlots()
            {
                m_pLayoutInstance = null;
                m_pCameraSlot = null;
                m_pPlayerSlot = null;
                m_vMonsterSlots.Clear();
            }

            public void OnRoomEnter()
            {
                m_pOnRoomEnter.Invoke(this);
            }

            public void OnRoomExit()
            {
                m_pOnRoomExit.Invoke(this);
            }
        }
    }
}
