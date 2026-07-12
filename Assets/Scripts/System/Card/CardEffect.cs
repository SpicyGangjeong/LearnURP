using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static Logic.ITargetable;

namespace Logic
{
    namespace Card
    {
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
                public void BuildExpression(in StringBuilder sb)
                {
                    switch (m_eTrigger)
                    {
                        case Trigger.NONE:
                        case Trigger.END:
                            return;
                        default:
                            break;
                    }
                    sb.Append("When ");
                    sb.Append(m_eTrigger.ToString());
                    sb.Append(", ");
                    foreach (Step iter in m_vSteps)
                    {
                        iter.BuildExpression(sb);
                    }
                }
            }
            [Serializable]
            class Step
            {
                [SerializeField] ITargetable.Entity m_eEntity = ITargetable.Entity.NONE;
                [SerializeField] ITargetable.Scope m_eScope = ITargetable.Scope.NONE;
                [SerializeField] ITargetable.Select m_eSelect = ITargetable.Select.NONE;
                [SerializeField] int m_iSelectCount;

                [SerializeField] List<Operation> m_vOperations = new List<Operation>();
                public Step() { }
                public Step(Step other)
                {
                    m_eEntity = other.m_eEntity;
                    m_eSelect = other.m_eSelect;
                    m_eScope = other.m_eScope;
                    m_iSelectCount = other.m_iSelectCount;
                    m_vOperations = new List<Operation>(other.m_vOperations.Count);
                    foreach (Operation pOper in other.m_vOperations)
                    {
                        m_vOperations.Add(new Operation(pOper));
                    }
                }
                public void BuildExpression(in StringBuilder sb)
                {
                    switch (m_eSelect)
                    {
                        case Select.NONE:
                        case Select.END:
                            break;
                        default:
                            sb.Append(m_eSelect.ToString());
                            sb.Append(" ");
                            if (0 == m_iSelectCount)
                            {
                                sb.Append(m_iSelectCount);
                                sb.Append(" ");
                            }
                            break;
                    }
                    switch (m_eScope)
                    {
                        case Scope.NONE:
                        case Scope.END:
                            break;
                        default:
                            sb.Append(m_eScope.ToString());
                            sb.Append(" ");
                            break;
                    }
                    switch (m_eEntity)
                    {
                        case Entity.NONE:
                        case Entity.END:
                            break;
                        default:
                            sb.Append(m_eEntity.ToString());
                            sb.Append(" ");
                            break;
                    }
                    foreach (Operation pOper in m_vOperations)
                    {
                        pOper.BuildExpression(sb);
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
                    foreach (Param pParam in other.m_vParams)
                    {
                        m_vParams.Add(new Param(pParam));
                    }
                }
                public void BuildExpression(in StringBuilder sb)
                {
                    switch (m_eValue)
                    {
                        case Value.NONE:
                        case Value.END:
                            return;
                        default:
                            sb.Append(m_eValue.ToString()); 
                            sb.Append(" ");
                            break;
                    }
                    sb.Append(m_iValue);
                    sb.Append(" ");
                    foreach (Param pParam in m_vParams)
                    {
                        pParam.BuildExpression(sb);
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
                public void BuildExpression(in StringBuilder sb)
                {
                    switch (m_eKey)
                    {
                        case SubParam.NONE:
                        case SubParam.END:
                            return;
                        default:
                            sb.Append(m_eKey.ToString());
                            sb.Append(" ");
                            break;
                    }
                    sb.Append(m_iValue);
                    sb.Append(" ");
                }
            }
        }

        #endregion // Declaration

        #region Definition
        public partial class CardEffect : ITriggable
        {
            [SerializeField] Block m_Block = new Block();

            public CardEffect() { }
            public CardEffect(CardEffect other)
            {
                m_Block = new Block(other.m_Block);
            }
            public void Triggered()
            {
                throw new NotImplementedException();
            }
            public void BuildExpression(in StringBuilder sb)
            {
                m_Block.BuildExpression(sb);
                sb.Append("\n");
            }
        }
        #endregion // Definition

    }
}