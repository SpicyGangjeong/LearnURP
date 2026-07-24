using Core;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] CharacterSlot m_pWaitingSlot = null;
    [SerializeField] CharacterSlot m_pCameraSlot = null;
    InfoInstance InfoInstance = null;
    GameInstance GameInstance = null;
    private void Awake()
    {
        InfoInstance = InfoInstance.Instance;
        GameInstance = GameInstance.Instance;
        GameInstance.Rooms.m_pOnRoomExited += EnterLobby;
        EnterLobby();
    }
    private void OnDestroy()
    {
        GameInstance.Rooms.m_pOnRoomExited -= EnterLobby;
    }

    public void EnterLobby(Logic.Room.Room fromRoom)
    {
        EnterLobby();
    }
    private void EnterLobby()
    {
        m_pWaitingSlot.SetCurrentUnit(InfoInstance.PlayerInstance.PlayerUnit);
        //m_pCameraSlot.SetCurrentUnit();
        Transform mainCameraTransform = GameInstance.Main_Camera.transform;
        mainCameraTransform.SetParent(m_pCameraSlot.transform);
        mainCameraTransform.SetPositionAndRotation(m_pCameraSlot.transform.position, m_pCameraSlot.transform.rotation);
    }
}
