using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace Core
{
    namespace Job
    {
        [Serializable]
        public abstract class JobBase
        {
            [Flags]
            public enum JobStates : byte
            {
                NONE = 0,
                JOB_DELAY = 1 << 0, // 1
                JOB_END_TURN = 1 << 1, // 2
            }
            public string m_strName = "";
            public abstract UniTask Run();
        }
        /// <summary>
        /// 최소한 일정 시간 대기 한 뒤 실행
        /// </summary>
        [Serializable]
        public class JobDelayCallback : JobBase
        {
            Func<UniTask> m_Callback;
            int m_iDelayMilisecnd;
            public JobDelayCallback(Func<UniTask> callback, string strEventName, int iDelayMilisecnd)
            {
                m_Callback = callback;
                m_strName = strEventName;
                m_iDelayMilisecnd = iDelayMilisecnd;
            }

            public override async UniTask Run()
            {
                await UniTask.Delay(m_iDelayMilisecnd);
                await m_Callback();
            }
        }
        /// <summary>
        /// 기본 대기열 콜백
        /// </summary>
        [Serializable]
        public class JobDeferredCallback : JobBase
        {
            Func<UniTask> m_Callback;
            public JobDeferredCallback(Func<UniTask> callback, string strEventName)
            {
                m_Callback = callback;
                m_strName = strEventName;
            }

            public override async UniTask Run()
            {
                await m_Callback();

            }
        }
    }
}