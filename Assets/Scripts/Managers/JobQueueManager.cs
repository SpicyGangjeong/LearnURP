using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JobQueueManager
{
    Queue<IJob> m_vJobLane = new Queue<IJob>();

    bool m_bProcessing = false;
    IJob.JobStates m_eStates = IJob.JobStates.NONE;

    public int JobCount => m_vJobLane.Count;
    public IJob.JobStates State { 
        get { return m_eStates; } 
        set {  m_eStates = value; }
    }

    private async UniTask ExecuteJobsAsync()
    {
        if (true == m_bProcessing)
        {
            return;
        }
        m_bProcessing = true;
        while (0 != m_vJobLane.Count) {
            await m_vJobLane.Dequeue().Run();
        }
        m_bProcessing = false;
    }
    
    public void EnqueueJob(IJob pJob)
    {
        m_vJobLane.Enqueue(pJob);
        ExecuteJobsAsync().Forget();
    }
}
