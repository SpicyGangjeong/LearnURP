using UnityEngine;

public class CardFrameRenderer : MonoBehaviour
{
    CGameInstance GameInstance = null;
    void Awake()
    {
        GameInstance = CGameInstance.Instance;
        if (null == GameInstance)
        {
            Debug.LogError("GameInstance is null");
            return;
        }
    }

    void Update()
    {
        
    }
}
