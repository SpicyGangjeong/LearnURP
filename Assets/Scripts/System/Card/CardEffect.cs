
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

#region Declaration
[Serializable]
public partial class CardEffect
{
    enum Trigger : int
    {
        NONE = -1,
        DRAW = 0,
        PLAY = 1,
        DISCARD = 2,
        RETURN = 3,
        DISAPPEAR = 4,
        SHUFFLE = 5,
        END = 6,
    }
    enum Value : int
    {
        NONE = -1,
        DAMAGE = 0,
        HEAL = 1,
        SHIELD = 2,
        BUFF = 3,
        DEBUFF = 4,
        END = 5,
    }
    enum SubParam : int
    {
        NONE = -1,
        DURATION,
        BUFF_ID,
        COUNT,
        END,
    }

    [Serializable]
    class Block
    {
        [SerializeField] Trigger m_eTrigger = Trigger.NONE;
        [SerializeField] List<Step> m_vSteps = new List<Step>();

        public Block() { }
        public Block(Block other)
        {
            m_eTrigger = other.m_eTrigger;
            m_vSteps = new List<Step>(other.m_vSteps.Count);
            foreach (Step pStep in other.m_vSteps)
            {
                m_vSteps.Add(new Step(pStep));
            }
        }
    }
    [Serializable]
    class Step
    {
        [SerializeField] ITargetable.Entity  m_eEntity = ITargetable.Entity.NONE;
        [SerializeField] ITargetable.Scope  m_eScope = ITargetable.Scope.NONE;
        [SerializeField] ITargetable.Select  m_eSelect = ITargetable.Select.NONE;
        [SerializeField] int m_iSelectCount;

        [SerializeField] List<Operation> m_vOperations = new List<Operation>();
        public Step() { }
        public Step(Step other)
        {
            m_eEntity = other.m_eEntity;
            m_eScope = other.m_eScope;
            m_eSelect = other.m_eSelect;
            m_iSelectCount = other.m_iSelectCount;
            m_vOperations = new List<Operation>(other.m_vOperations.Count);
            foreach (Operation pStep in other.m_vOperations)
            {
                m_vOperations.Add(new Operation(pStep));
            }
        }
    }
    [Serializable]
    class Operation
    {
        [SerializeField] Value m_eValue = Value.NONE;
        [SerializeField] int m_iValue;
        [SerializeField] List<Param> m_vParams = new List<Param>();
        public Operation() { }
        public Operation(Operation other)
        {
            m_eValue = other.m_eValue;
            m_iValue = other.m_iValue;
            m_vParams = new List<Param>(other.m_vParams.Count);
            foreach (Param pStep in other.m_vParams)
            {
                m_vParams.Add(new Param(pStep));
            }
        }
    }
    [Serializable]
    class Param
    {
        [SerializeField] SubParam m_eKey = SubParam.NONE;
        [SerializeField] float m_iValue;
        public Param() { }
        public Param(Param other)
        {
            m_eKey = other.m_eKey;
            m_iValue = other.m_iValue;
        }
    }
}

#endregion // Declaration

#region Definition
public partial class CardEffect : ITriggable
{
    [SerializeField] Block m_block = new Block();

    public CardEffect() { }
    public CardEffect(CardEffect other)
    {
        m_block = new Block(other.m_block);
    }
    public void Triggered()
    {
        throw new NotImplementedException();
    }
}
#endregion // Definition
