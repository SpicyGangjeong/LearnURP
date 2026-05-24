using Unity.VisualScripting;
using UnityEngine;

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
