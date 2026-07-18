using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorAPI
{
    internal static class PlayFromBootstrap
    {
        const string s_strToolbarPath = "Play/From Bootstrap";
        const string s_strBootstrapScene = "Assets/Scenes/InitializeScene.unity";

        static string s_strSceneToRestore = null;

        [MainToolbarElement(s_strToolbarPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        static MainToolbarElement CreatePlayButton()
        {
            // IconContent name-only lookup requires: Assets/Editor Default Resources/Icons/{name}.png
            Texture2D pIcon = EditorGUIUtility.IconContent("PlayFromBootstrap", "StartBootStrap").image as Texture2D;
            if (null == pIcon)
            {
                pIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/Editor Default Resources/Icons/PlayFromBootstrap.png");
            }

            if (null == pIcon)
            {
                pIcon = EditorGUIUtility.IconContent("PlayButton").image as Texture2D;
            }

            MainToolbarContent pContent = new MainToolbarContent(
                pIcon,
                "Play From Bootstrap (InitializeScene)");

            return new MainToolbarButton(pContent, OnPlayButtonClicked);
        }

        static void OnPlayButtonClicked()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            if (false == EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            s_strSceneToRestore = SceneManager.GetActiveScene().path;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (s_strSceneToRestore != s_strBootstrapScene)
            {
                EditorSceneManager.OpenScene(s_strBootstrapScene);
            }

            EditorApplication.isPlaying = true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange eState)
        {
            if (PlayModeStateChange.EnteredEditMode != eState)
            {
                return;
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            if (string.IsNullOrEmpty(s_strSceneToRestore))
            {
                return;
            }

            if (s_strSceneToRestore == SceneManager.GetActiveScene().path)
            {
                return;
            }

            EditorSceneManager.OpenScene(s_strSceneToRestore);
            s_strSceneToRestore = null;
        }
    }
}
