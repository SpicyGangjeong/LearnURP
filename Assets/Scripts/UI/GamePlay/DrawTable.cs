using Core;
using Unity.Mathematics;
using UnityEngine;


namespace View
{
    namespace UI
    {
        public class DrawTable : MonoBehaviour
        {
            float2 vShowTime;
            GameInstance m_pGameInstance;
            [SerializeField]
            CardFrameRenderer m_pCardFrameRenderer;


            private void Awake()
            {
                vShowTime.x = 0;
                vShowTime.y = 2.0f;
                m_pGameInstance = GameInstance.Instance;
            }
            void Start()
            {

            }

            private void OnEnable()
            {
                vShowTime.x = vShowTime.y;
            }

            private void OnDisable()
            {
                vShowTime.x = vShowTime.y;
            }
            void Update()
            {
                vShowTime.x -= Time.deltaTime;
                if (vShowTime.x <= 0)
                {
                    gameObject.SetActive(false);
                }


            }

            public void ShowCards(Defines.Enums.CardPile eCardPile)
            {
                m_pCardFrameRenderer.RenderCardFrames(eCardPile);
            }

        }

    }

}

