using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

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
