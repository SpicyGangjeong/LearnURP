using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JobQueueManager
{
    Queue<IJob> m_vJobLane = new Queue<IJob>();
    bool m_bProcessing = false;
    DEFINES.ENUMS.JobStates m_eStates = DEFINES.ENUMS.JobStates.NONE;

    public int JobCount => m_vJobLane.Count;
    public DEFINES.ENUMS.JobStates State { 
        get { return m_eStates; } 
        set {  m_eStates = value; }
    }

    public bool IsAwaitingEnd()
    {
        return 0 == m_vJobLane.Count && 
        DEFINES.HELPERS.BIT.Has(m_eStates, DEFINES.ENUMS.JobStates.JOB_END_TURN);
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
