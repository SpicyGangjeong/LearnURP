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
    Card card = null;
    public JobPlayCard(Card card)
    {
        this.card = card;
    }
    public UniTask Run()
    {
        return UniTask.CompletedTask;
    }
}

public class JobDrawCard : IJob
{
    Card card = null;
    public JobDrawCard(Card card)
    {
        this.card = card;
    }

    public UniTask Run()
    {
        return UniTask.CompletedTask;
    }
}