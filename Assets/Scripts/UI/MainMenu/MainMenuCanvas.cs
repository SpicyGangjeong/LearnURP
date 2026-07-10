using Core;
using UnityEngine;

namespace View
{
    namespace UI
    {
        public class MainMenuCanvas : MonoBehaviour
        {
            CGameInstance m_pGameInstance = null;

            void Start()
            {
                m_pGameInstance = CGameInstance.Instance;
            }

            void Update()
            {
            }
            public void StartDeck(int i)
            {
                m_pGameInstance.StartDeck(i);
                Destroy(gameObject);
            }
        }

    }
}
