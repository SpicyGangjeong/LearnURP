using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    namespace Job
    {
        [Serializable]
        public class JobQueueManager
        {
            [SerializeReference]
            List<JobBase> m_vJobLane = new List<JobBase>();

            bool m_bProcessing = false;
            JobBase.JobStates m_eStates = JobBase.JobStates.NONE;

            public int JobCount => m_vJobLane.Count;
            public JobBase.JobStates State
            {
                get { return m_eStates; }
                set { m_eStates = value; }
            }

            private async UniTask ExecuteJobsAsync()
            {
                if (true == m_bProcessing)
                {
                    return;
                }
                m_bProcessing = true;
                try
                {
                    while (0 != m_vJobLane.Count)
                    {
                        JobBase pJob = m_vJobLane[0];
                        m_vJobLane.RemoveAt(0);
                        await pJob.Run();
                    }
                }
                finally
                {
                    m_bProcessing = false;
                }
            }

            public void EnqueueJob(JobBase pJob)
            {
                m_vJobLane.Add(pJob);
                ExecuteJobsAsync().Forget();
            }
        }
    }
}