using Core;
using Core.Room;
using Cysharp.Threading.Tasks;
using Logic.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    namespace UI
    {
        public class RoomButton : MonoBehaviour
        {
            [SerializeField]
            Button m_pButton = null;
            [SerializeField]
            TextMeshProUGUI m_pLabel = null;

            Logic.Room.Room m_pRoom = null;
            GameInstance m_pGameInstance = null;

            void Awake()
            {
                if (null == m_pButton)
                {
                    m_pButton = GetComponent<Button>();
                }
                if (null == m_pButton)
                {
                    throw new System.InvalidOperationException("RoomButton requires a Button component.");
                }
                m_pButton.onClick.AddListener(OnClicked);
            }

            void OnDestroy()
            {
                if (null != m_pButton)
                {
                    m_pButton.onClick.RemoveListener(OnClicked);
                }
            }

            public void Bind(Logic.Room.Room pRoom)
            {
                if (null == pRoom)
                {
                    throw new System.ArgumentNullException(nameof(pRoom));
                }
                m_pRoom = pRoom;
                m_pGameInstance = GameInstance.Instance;
                Refresh();
            }

            public void Refresh()
            {
                if (null == m_pRoom)
                {
                    return;
                }
                if (null != m_pLabel)
                {
                    m_pLabel.text = $"{m_pRoom.Data.Name} ({m_pRoom.Data.Type})";
                }
                m_pButton.interactable = true == m_pRoom.Active && false == m_pRoom.Cleared;
            }

            void OnClicked()
            {
                if (null == m_pRoom || null == m_pGameInstance)
                {
                    return;
                }
                EnterAsync().Forget();
            }

            async UniTaskVoid EnterAsync()
            {
                await m_pGameInstance.Rooms.EnterRoom(m_pRoom);
            }
        }
    }
}
