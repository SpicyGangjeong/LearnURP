using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DrawTable : MonoBehaviour
{
    float2 vShowTime;
    CGameInstance m_pGameInstance;
    [SerializeField]
    CardFrameRenderer m_pCardFrameRenderer;


    private void Awake()
    {
        vShowTime.x = 0;
        vShowTime.y = 1.0f;
        m_pGameInstance = CGameInstance.Instance;
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

    public void ShowCards(DEFINES.ENUMS.CardPile cardPile)
    {
        m_pCardFrameRenderer.RenderCardFrames(cardPile);
    }

}
