using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    namespace Room
    {
        public class RoomLayout : MonoBehaviour
        {
            [SerializeField]
            CharacterSlot m_pCameraSlot = null;
            [SerializeField]
            CharacterSlot m_pPlayerSlot = null;
            [SerializeField]
            List<CharacterSlot> m_vMonsterSlots = new List<CharacterSlot>();

            public CharacterSlot CameraSlot => m_pCameraSlot;
            public CharacterSlot PlayerSlot => m_pPlayerSlot;
            public IReadOnlyList<CharacterSlot> MonsterSlots => m_vMonsterSlots;
        }
    }
}
