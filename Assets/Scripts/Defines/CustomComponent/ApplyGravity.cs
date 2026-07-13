using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    [SerializeField, Range(-20.0f, 20.0f)]
    float m_fGravityAmplitude = -9.14f;
    CharacterController pCharacterController = null;
    void Start()
    {
        pCharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 m_vGravity = new Vector3(0.0f, m_fGravityAmplitude, 0.0f) * Time.deltaTime;
        pCharacterController.Move(m_vGravity);
    }
}

