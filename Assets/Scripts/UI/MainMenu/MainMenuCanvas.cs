using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    CGameInstance gameInstance = null;

    [SerializeField]
    private Button btnSet0 = null;
    [SerializeField]
    private Button btnSet1 = null;
    [SerializeField]
    private Button btnSet2 = null;

    void Start()
    {
        gameInstance = CGameInstance.Instance;
    }

    void Update()
    {
    }
    public void StartDeck(int i)
    {
        gameInstance.StartDeck(i);
        Destroy(gameObject);
    }
}
