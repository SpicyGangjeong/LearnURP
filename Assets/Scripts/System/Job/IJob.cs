using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface IJob
{
    UniTask Run();
}
public class JobPlayCard : IJob
{
    Card m_pCard = null;
    public JobPlayCard(Card pCard)
    {
        m_pCard = pCard;
    }
    public UniTask Run()
    {
        return UniTask.CompletedTask;
    }
}

public class JobDrawCard : IJob
{
    Card m_pCard = null;
    public JobDrawCard(Card pCard)
    {
        m_pCard = pCard;
    }

    public UniTask Run()
    {
        return UniTask.CompletedTask;
    }
}