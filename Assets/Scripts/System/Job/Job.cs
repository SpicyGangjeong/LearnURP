using Cysharp.Threading.Tasks;
using System;

namespace Core
{
    namespace Job
    {
        public interface IJob
        {
            [Flags]
            public enum JobStates : byte
            {
                NONE = 0,
                JOB_DELAY = 1 << 0, // 1
                JOB_END_TURN = 1 << 1, // 2
            }
            public static string Name { get; }
            UniTask Run();
        }
        public class JobDelayCallback : IJob
        {
            private const string s_strName = "JobDelayCallback";
            public static string Name { get { return s_strName; } }

            Action m_Callback;
            int m_iDelayMili;
            public JobDelayCallback(Action callback, int iDelayMili)
            {
                m_Callback = callback;
                m_iDelayMili = iDelayMili;
            }

            public async UniTask Run()
            {
                await UniTask.Delay(m_iDelayMili);
                m_Callback();
            }
        }
        public class JobDeferredCallback : IJob
        {
            private const string s_strName = "JobDeferredCallback";
            public static string Name { get { return s_strName; } }
            Func<UniTask> m_Callback;
            public JobDeferredCallback(Func<UniTask> callback)
            {
                m_Callback = callback;
            }

            public async UniTask Run()
            {
                await m_Callback();

            }
        }

        public class JobEndTurnCallback : IJob
        {
            private const string s_strName = "JobEndTurnCallback";
            public static string Name { get { return s_strName; } }
            Func<UniTask> m_Callback;
            public JobEndTurnCallback(Func<UniTask> callback)
            {
                m_Callback = callback;

            }
            public async UniTask Run()
            {
                await m_Callback();
            }
        }
        public class JobDiscardCallback : IJob
        {
            private const string s_strName = "JobDiscardCallback";
            public static string Name { get { return s_strName; } }
            Func<UniTask> m_Callback;
            public JobDiscardCallback(Func<UniTask> callback)
            {
                m_Callback = callback;

            }
            public async UniTask Run()
            {
                await m_Callback();
            }
        }
        public class JobDrawCallback : IJob
        {
            private const string s_strName = "JobDrawCallback";
            public static string Name { get { return s_strName; } }
            Func<UniTask> m_Callback;
            public JobDrawCallback(Func<UniTask> callback)
            {
                m_Callback = callback;

            }
            public async UniTask Run()
            {
                await m_Callback();
            }
        }

        public class JobAwaitingEndCallback : IJob
        {
            private const string s_strName = "JobAwaitingEndCallback";
            public static string Name { get { return s_strName; } }
            Func<UniTask> m_Callback;
            public JobAwaitingEndCallback(Func<UniTask> callback)
            {
                m_Callback = callback;
            }
            public async UniTask Run()
            {
                await m_Callback();
            }
        }
    }
}