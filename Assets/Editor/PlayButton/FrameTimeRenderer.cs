using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorAPI
{
    internal static class ToolbarFpsDisplay
    {
        const string s_strScenePath = "Play/Scene FPS";
        const string s_strGamePath = "Play/Game FPS";

        static double s_fLastTime;
        static float s_fSceneFps;
        static float s_fGameFps;
        static float s_fThrottleTime = 0.25f;
        static double s_fLastRefreshTime;
        static bool s_bHooked;

        [InitializeOnLoadMethod]
        static void Init()
        {
            if (s_bHooked)
            {
                return;
            }

            s_bHooked = true;
            s_fLastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += OnEditorUpdate;
        }

        static void OnEditorUpdate()
        {
            double fNow = EditorApplication.timeSinceStartup;
            float fDelta = (float)(fNow - s_fLastTime);
            s_fLastTime = fNow;

            if (fDelta > 0.0001f)
            {
                // 단순 EMA로 흔들림 완화
                float fInstant = 1f / fDelta;
                s_fSceneFps = Mathf.Lerp(s_fSceneFps, fInstant, 0.1f);
            }

            if (EditorApplication.isPlaying && false == EditorApplication.isPaused)
            {
                float fGameInstant = 1f / Time.unscaledDeltaTime;
                s_fGameFps = Mathf.Lerp(s_fGameFps, fGameInstant, 0.1f);
            }
            else
            {
                s_fGameFps = 0f;
            }

            if (fNow - s_fLastRefreshTime > s_fThrottleTime)
            {
                s_fLastRefreshTime = fNow;
                MainToolbar.Refresh(s_strScenePath);
                MainToolbar.Refresh(s_strGamePath);
            }
        }

        [MainToolbarElement(s_strScenePath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 0)]
        static MainToolbarElement CreateSceneFpsLabel()
        {
            LockWidth(s_strScenePath, 90f);
            string strText = s_fSceneFps > 0.5f
                ? $"Scene {s_fSceneFps:0000}\t"
                : "Scene 0000\t";
            return new MainToolbarLabel(
                new MainToolbarContent(strText, "Scene / Editor FPS"));
        }

        [MainToolbarElement(s_strGamePath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 1)]
        static MainToolbarElement CreateGameFpsLabel()
        {
            LockWidth(s_strGamePath, 90f);
            string strText = s_fGameFps > 0.5f
                ? $"Game {s_fGameFps:0000}\t"
                : "Game 0000\t";

            return new MainToolbarLabel(
                new MainToolbarContent(strText, "Game FPS (Play Mode)"));
        }
        public static void LockWidth(string strTooltip, float fWidth)
        {
            EditorApplication.delayCall += () =>
            {
                foreach (EditorWindow pWindow in Resources.FindObjectsOfTypeAll<EditorWindow>())
                {
                    VisualElement pRoot = pWindow.rootVisualElement;
                    if (null == pRoot)
                    {
                        continue;
                    }

                    VisualElement pElement = pRoot.Query<VisualElement>()
                        .Where(p => p.tooltip == strTooltip)
                        .First();
                    if (null == pElement)
                    {
                        continue;
                    }

                    pElement.style.minWidth = fWidth;
                    pElement.style.maxWidth = fWidth;
                    pElement.style.flexShrink = 0f;
                    return;
                }
            };
        }
    }
}