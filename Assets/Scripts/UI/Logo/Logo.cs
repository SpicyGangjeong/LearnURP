using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    LoadSceneAddressable loadAddressableScene = null;
    void Start()
    {
        loadAddressableScene = gameObject.GetOrAddComponent<LoadSceneAddressable>();
    }

    void OnClick()
    {
        loadAddressableScene.LoadScene();
        
    }
}
