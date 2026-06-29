using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JobQueueManager
{
    Queue<IJob> m_vJobLaneFrontBuffer = new Queue<IJob>();
    Queue<IJob> m_vJovLaneBackBuffer = new Queue<IJob>();

    bool m_bProcessing = false;
    DEFINES.ENUMS.JobStates m_eStates = DEFINES.ENUMS.JobStates.NONE;

    public int JobCount => m_vJobLaneFrontBuffer.Count;
    public DEFINES.ENUMS.JobStates State { 
        get { return m_eStates; } 
        set {  m_eStates = value; }
    }

    public bool IsAwaitingEnd()
    {
        return 0 == m_vJobLaneFrontBuffer.Count && 
        DEFINES.HELPERS.BIT.Has(m_eStates, DEFINES.ENUMS.JobStates.JOB_END_TURN);
    }
    public void SwapBuffers()
    {
        Queue<IJob> temp = m_vJobLaneFrontBuffer;
        m_vJobLaneFrontBuffer = m_vJovLaneBackBuffer;
        m_vJovLaneBackBuffer = temp;
    }


    private async UniTask ExecuteJobsAsync()
    {
        if (true == m_bProcessing)
        {
            return;
        }
        m_bProcessing = true;
        while (0 != m_vJobLaneFrontBuffer.Count) {
            await m_vJobLaneFrontBuffer.Dequeue().Run();
        }
        m_bProcessing = false;
    }
    
    public void EnqueueJob(IJob pJob, bool bFront = true)
    {
        if (true == bFront)
        {
            m_vJobLaneFrontBuffer.Enqueue(pJob);
            ExecuteJobsAsync().Forget();
        }
        else
        {
            m_vJovLaneBackBuffer.Enqueue(pJob);
        }
    }
}
