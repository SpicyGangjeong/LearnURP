using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    [SerializeField]
    CharacterController pCharacterController = null;
    void Start()
    {
        pCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 m_vGravity = Vector3.down * Time.deltaTime;
        pCharacterController.Move(m_vGravity);
    }
}
