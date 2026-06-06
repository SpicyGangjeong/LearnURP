using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

class JobQueueManager
{
    Queue<IJob> m_vJobs = new Queue<IJob>();
    bool m_bProcessing = false;
    public async UniTask ExecuteJobsAsync()
    {
        if (true == m_bProcessing)
        {
            return;
        }
        m_bProcessing = true;
        while (0 != m_vJobs.Count) {
            await m_vJobs.Dequeue().Run();
        }
        m_bProcessing = false;
    }
    public void EnqueueJob(IJob pJob)
    {
        m_vJobs.Enqueue(pJob);
        ExecuteJobsAsync().Forget();
    }
}
