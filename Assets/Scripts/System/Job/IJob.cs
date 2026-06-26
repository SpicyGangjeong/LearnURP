using Cysharp.Threading.Tasks;
using DEFINES.STRUCTURES;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public interface IJob
{
    public static string Name { get; }
    UniTask Run();
}
public class JobDelayAction : IJob
{
    private const string s_strName = "JobDelayAction";
    public static string Name { get { return s_strName; } }

    Action m_Action;
    int m_iDelayMili;
    public JobDelayAction(Action action, int iDelayMili)
    {
        m_Action = action;
        m_iDelayMili = iDelayMili;
    }

    public async UniTask Run()
    {
        await UniTask.Delay(m_iDelayMili);
        m_Action();
    }
}

public class JobEndTurnAction : IJob
{
    private const string s_strName = "JobEndTurnAction";
    public static string Name { get { return s_strName; } }
    Func<UniTask> m_Action;
    public JobEndTurnAction(Func<UniTask> action)
    {
        m_Action = action;
        
    }
    public async UniTask Run()
    {
        await m_Action();
    }
}
public class JobDiscardAction : IJob
{
    private const string s_strName = "JobDiscardAction";
    public static string Name { get { return s_strName; } }
    Func<UniTask> m_Action;
    public JobDiscardAction(Func<UniTask> action)
    {
        m_Action = action;

    }
    public async UniTask Run()
    {
        await m_Action();
    }
}