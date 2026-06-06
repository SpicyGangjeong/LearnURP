using UnityEngine;

public class CardFrameRenderer : MonoBehaviour
{
    CGameInstance m_pGameInstance = null;
    void Awake()
    {
        m_pGameInstance = CGameInstance.Instance;
    }

    void Update()
    {
        
    }
}
