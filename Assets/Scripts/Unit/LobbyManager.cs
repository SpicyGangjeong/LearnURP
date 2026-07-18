using Core;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    CharacterSlot m_pWaitingSlot = null;
    [SerializeField]
    CharacterSlot m_pCameraSlot = null;
    CInfoInstance InfoInstance = null;
    //CGameInstance GameInstance = null;
    private void Awake()
    {
        InfoInstance = CInfoInstance.Instance;
        //GameInstance = CGameInstance.Instance;
        m_pWaitingSlot.SetCurrentUnit(InfoInstance.PlayerInstance.PlayerUnit);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
