
using DEFINES.ENUMS;
using System;
using System.Collections.Generic;

/// <summary>
/// 언제-( 이벤트 순서 )
/// </summary>
[Serializable]
public class CardEffectBlock
{
    public CardEffectTriggerType m_iTriggerType;
    public List<CardEffectStep> m_vSteps = new List<CardEffectStep>();
}

/// <summary>
/// 누구에게 - ( 연산 순서 )
/// </summary>
[Serializable]
public class CardEffectStep
{
    public CardEffectTargetType m_iTargetType;
    public List<CardEffectOperation> m_vOperations = new List<CardEffectOperation>();
}

/// <summary>
/// 무엇을 - 특정 값만큼 - ( ??? )
/// </summary>
[Serializable]
public class CardEffectOperation
{
    public CardEffectValueType m_iValueType;
    public int m_iValue;
    public List<EffectParam> m_vParams = new List<EffectParam>();  // 확장용
}

/// <summary>
/// ???
/// </summary>
[Serializable]
public class EffectParam
{
    public CardEffectParamKey m_iKey;  // DURATION, BUFF_ID, COUNT …
    public int m_iValue;
}