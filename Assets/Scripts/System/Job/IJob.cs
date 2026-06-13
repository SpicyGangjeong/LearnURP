using Cysharp.Threading.Tasks;
using DEFINES.STRUCTURES;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public interface IJob
{
    UniTask Run();
}
public class JobDrawCard : IJob
{
    Card m_pCard = null;
    CardCanvas m_pCanvas = null;
    public JobDrawCard(Card pCard, CardCanvas pCardCanvas)
    {
        m_pCard = pCard;
        m_pCanvas = pCardCanvas;
    }
    public UniTask Run()
    {
        CGameInstance.Instance.TryHandboardInsertCard(m_pCard, m_pCanvas);
        return UniTask.CompletedTask;
    }
}
public class JobPlayCard : IJob
{
    Card m_pCard = null;
    CardCanvas m_pCanvas = null;
    public JobPlayCard(Card pCard, CardCanvas pCardCanvas)
    {
        m_pCard = pCard;
        m_pCanvas = pCardCanvas;
    }
    public UniTask Run()
    {
        CGameInstance.Instance.TryPlayCard(m_pCard, m_pCanvas);
        return UniTask.CompletedTask;
    }
}

public class JobDiscardCard : IJob
{
    Card m_pCard = null;
    CardCanvas m_pCanvas = null;
    public JobDiscardCard(Card pCard, CardCanvas pCardCanvas)
    {
        m_pCard = pCard;
        m_pCanvas = pCardCanvas;
    }
    public UniTask Run()
    {
        CGameInstance.Instance.TryDiscardCard(m_pCard, m_pCanvas);
        return UniTask.CompletedTask;
    }
}
