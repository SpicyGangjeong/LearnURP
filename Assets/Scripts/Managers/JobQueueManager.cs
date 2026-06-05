using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

class JobQueueManager
{
    Queue<IJob> qJobs = new Queue<IJob>();
    bool bProcessing = false;
    public async UniTask ExecuteJobsAsync()
    {
        if (true == bProcessing)
        {
            return;
        }
        bProcessing = true;
        while (0 != qJobs.Count) {
            await qJobs.Dequeue().Run();
        }
        bProcessing = false;
    }
    public void EnqueueJob(IJob queue)
    {
        qJobs.Enqueue(queue);
        ExecuteJobsAsync().Forget();
    }
}