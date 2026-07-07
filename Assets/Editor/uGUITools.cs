/// Credit Senshi  

/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/ (uGUITools link)



using UnityEditor;



namespace UnityEngine.UI.Extensions

{

    public static class uGUITools

    {

        static bool CanUseParentRect(RectTransform t) => t != null && t.parent is RectTransform;



        static void ApplyAnchorsToCorners(RectTransform t)

        {

            var pt = (RectTransform)t.parent;

            float pw = pt.rect.width;

            float ph = pt.rect.height;

            if (pw == 0f || ph == 0f) return;



            t.anchorMin = new Vector2(

                t.anchorMin.x + t.offsetMin.x / pw,

                t.anchorMin.y + t.offsetMin.y / ph);

            t.anchorMax = new Vector2(

                t.anchorMax.x + t.offsetMax.x / pw,

                t.anchorMax.y + t.offsetMax.y / ph);

            t.offsetMin = t.offsetMax = Vector2.zero;

        }



        static void ApplyCornersToAnchors(RectTransform t)

        {

            t.offsetMin = t.offsetMax = Vector2.zero;

        }



        [MenuItem("uGUI/Anchors to Corners %[")]

        static void AnchorsToCornersMenu()

        {

            foreach (Transform transform in Selection.transforms)

            {

                var t = transform as RectTransform;

                if (!CanUseParentRect(t)) continue;



                Undo.RecordObject(t, "Anchors to Corners");

                ApplyAnchorsToCorners(t);

            }

        }



        [MenuItem("CONTEXT/RectTransform/Anchors to Corners", false, 2010)]

        static void AnchorsToCornersContext(MenuCommand command)

        {

            var t = command.context as RectTransform;

            if (!CanUseParentRect(t)) return;



            Undo.RecordObject(t, "Anchors to Corners");

            ApplyAnchorsToCorners(t);

        }



        [MenuItem("CONTEXT/RectTransform/Anchors to Corners", true)]

        static bool AnchorsToCornersContextValidate(MenuCommand command) =>

            CanUseParentRect(command.context as RectTransform);



        [MenuItem("uGUI/Corners to Anchors %]")]

        static void CornersToAnchorsMenu()

        {

            foreach (Transform transform in Selection.transforms)

            {

                var t = transform as RectTransform;

                if (t == null) continue;



                Undo.RecordObject(t, "Corners to Anchors");

                ApplyCornersToAnchors(t);

            }

        }



        [MenuItem("CONTEXT/RectTransform/Corners to Anchors", false, 2011)]

        static void CornersToAnchorsContext(MenuCommand command)

        {

            var t = command.context as RectTransform;

            if (t == null) return;



            Undo.RecordObject(t, "Corners to Anchors");

            ApplyCornersToAnchors(t);

        }



        [MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]

        static void MirrorHorizontallyAnchors()

        {

            MirrorHorizontally(false);

        }



        [MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]

        static void MirrorHorizontallyParent()

        {

            MirrorHorizontally(true);

        }



        static void MirrorHorizontally(bool mirrorAnchors)

        {

            foreach (Transform transform in Selection.transforms)

            {

                RectTransform t = transform as RectTransform;

                RectTransform pt = t != null ? t.parent as RectTransform : null;



                if (t == null || pt == null) continue;



                Undo.RecordObject(t, "Mirror Horizontally");



                if (mirrorAnchors)

                {

                    Vector2 oldAnchorMin = t.anchorMin;

                    t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);

                    t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);

                }



                Vector2 oldOffsetMin = t.offsetMin;

                t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);

                t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);



                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);

            }

        }



        [MenuItem("uGUI/Mirror Vertically Around Anchors %'")]

        static void MirrorVerticallyAnchors()

        {

            MirrorVertically(false);

        }



        [MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]

        static void MirrorVerticallyParent()

        {

            MirrorVertically(true);

        }



        static void MirrorVertically(bool mirrorAnchors)

        {

            foreach (Transform transform in Selection.transforms)

            {

                RectTransform t = transform as RectTransform;

                RectTransform pt = t != null ? t.parent as RectTransform : null;



                if (t == null || pt == null) continue;



                Undo.RecordObject(t, "Mirror Vertically");



                if (mirrorAnchors)

                {

                    Vector2 oldAnchorMin = t.anchorMin;

                    t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);

                    t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);

                }



                Vector2 oldOffsetMin = t.offsetMin;

                t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);

                t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);



                t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);

            }

        }

    }

}


