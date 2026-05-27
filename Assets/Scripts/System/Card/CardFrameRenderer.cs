using UnityEngine;

public class CardFrameRenderer : MonoBehaviour
{
    CGameInstance GameInstance = null;
    void Awake()
    {
        GameInstance = CGameInstance.Instance;
    }

    void Update()
    {
        
    }
}
