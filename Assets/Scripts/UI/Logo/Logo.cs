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
        // LoadSceneMode.Single이면 기존에 열린 씬은 Unity가 내려주므로
        // 현재 씬을 따로 UnloadSceneAsync 할 필요가 없습니다(순서 꼬임 방지).
        loadAddressableScene.LoadScene();
    }
}
