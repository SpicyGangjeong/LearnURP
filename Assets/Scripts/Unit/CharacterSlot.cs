using Defines.Expressions;
using Logic;
using UnityEngine;

public class CharacterSlot : MonoBehaviour, ISlot
{
    IUnit m_pCurrentUnit = null;
    public IUnit GetCurrentUnit()
    {
        return m_pCurrentUnit;
    }
    public void SetCurrentUnit(IUnit pUnit)
    {
        m_pCurrentUnit = pUnit;
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
