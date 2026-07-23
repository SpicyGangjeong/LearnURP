using Defines.Expressions;
using Logic;
using UnityEngine;

public class CharacterSlot : MonoBehaviour, ISlot
{
    IUnit m_pCurrentUnit = null;
    MeshRenderer m_pRenderer = null;
    private void Awake()
    {
        m_pRenderer = GetComponent<MeshRenderer>();
        SetSlotRenderState();
    }
    public IUnit GetCurrentUnit()
    {
        return m_pCurrentUnit;
    }
    public void SetCurrentUnit(IUnit pUnit)
    {
        m_pCurrentUnit = pUnit;
        SetSlotRenderState();
        TransformHandle hTransform = pUnit.GetTransformHandle();
        CharacterController cct = pUnit.GetTargetObject().GetComponent<CharacterController>();
        if (null != cct && true == cct.enabled)
        {
            cct.enabled = false;
        }
        hTransform.SetParent(transformHandle);
        hTransform.SetPositionAndRotation(transformHandle.position, transformHandle.rotation);
        if (null != cct)
        {
            cct.enabled = true;
        }
    }
    void SetSlotRenderState()
    {
        m_pRenderer.enabled = false;//(null == m_pCurrentUnit);
    }

    public GameObject GetTargetObject()
    {
        return gameObject;
    }

    public TransformHandle GetTransformHandle()
    {
        return transformHandle;
    }

    public void Triggered()
    {
        throw new System.NotImplementedException();
    }
}
