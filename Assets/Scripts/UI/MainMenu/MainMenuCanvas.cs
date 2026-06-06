using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class MainMenuCanvas : MonoBehaviour
{
    CGameInstance m_pGameInstance = null;

    [FormerlySerializedAs("btnSet0")]
    [SerializeField]
    Button m_pBtnSet0 = null;
    [FormerlySerializedAs("btnSet1")]
    [SerializeField]
    Button m_pBtnSet1 = null;
    [FormerlySerializedAs("btnSet2")]
    [SerializeField]
    Button m_pBtnSet2 = null;

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
