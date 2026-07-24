using Core;
using UnityEngine;

namespace View
{
    namespace UI
    {
        public class MainMenuCanvas : MonoBehaviour
        {
            GameInstance m_pGameInstance = null;

            void Start()
            {
                m_pGameInstance = GameInstance.Instance;
            }

            public void StartFieldLevel(int i)
            {
                m_pGameInstance.StartFieldLevel(i);
            }
        }

    }
}
