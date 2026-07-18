using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using View.UI;

[CustomPreview(typeof(Logic.Card.CardDataSO))]
public class CardPreview : ObjectPreview
{
    PreviewRenderUtility m_pPreview;
    GameObject m_pRoot;
    CardCanvas m_pCardCanvas;
    Texture m_pCachedTex;
    Logic.Card.CardDataSO m_pBoundData;

    const string s_strPrefabPath = "Assets/Prefab/UI/CardCanvas.prefab";

    public override bool HasPreviewGUI() => true;

    public override void Initialize(Object[] targets)
    {
        base.Initialize(targets);
        EnsureSetup();
        Bind((Logic.Card.CardDataSO)target);
    }

    void EnsureSetup()
    {
        if (null != m_pPreview)
        {
            return;
        }

        m_pPreview = new PreviewRenderUtility(true);
        Camera pCam = m_pPreview.camera;
        pCam.clearFlags = CameraClearFlags.SolidColor;
        pCam.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        pCam.cameraType = CameraType.Game;
        pCam.nearClipPlane = 0.01f;
        pCam.farClipPlane = 100f;
        pCam.orthographic = true;
        pCam.orthographicSize = 1f;
        pCam.transform.position = new Vector3(0f, 0f, -10f);
        pCam.transform.rotation = Quaternion.identity;

        m_pRoot = new GameObject("CardPreviewRoot", typeof(Canvas), typeof(CanvasScaler));
        Canvas pCanvas = m_pRoot.GetComponent<Canvas>();
        pCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        pCanvas.worldCamera = pCam;
        pCanvas.planeDistance = 1f;

        m_pPreview.AddSingleGO(m_pRoot);

        GameObject pPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(s_strPrefabPath);
        GameObject pInst = (GameObject)PrefabUtility.InstantiatePrefab(pPrefab);
        pInst.transform.SetParent(m_pRoot.transform, false);
        m_pCardCanvas = pInst.GetComponent<CardCanvas>();
    }

    void Bind(Logic.Card.CardDataSO pData)
    {
        if (null == m_pCardCanvas || null == pData)
        {
            return;
        }

        m_pBoundData = pData;
        m_pCardCanvas.BindCardData(m_pBoundData.Instantiate());
        m_pCachedTex = null;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        if (null == m_pPreview)
        {
            EnsureSetup();
            Bind(target as Logic.Card.CardDataSO);
        }

        Logic.Card.CardDataSO pData = target as Logic.Card.CardDataSO;
        if (true == IsDirty(pData))
        {
            Bind(pData);
            m_pPreview.BeginPreview(r, background);
            m_pPreview.camera.Render();
            m_pCachedTex = m_pPreview.EndPreview();
        }

        if (null != m_pCachedTex)
        {
            GUI.DrawTexture(r, m_pCachedTex, ScaleMode.ScaleToFit, false);
        }
    }

    public bool IsDirty(Logic.Card.CardDataSO pData)
    {
        return EditorUtility.IsDirty(pData)
            || pData != m_pBoundData
            || null == m_pCachedTex;
    }

    public override void Cleanup()
    {
        if (null != m_pPreview)
        {
            m_pPreview.Cleanup();
            m_pPreview = null;
        }
        m_pRoot = null;
        m_pCardCanvas = null;
        m_pCachedTex = null;
        m_pBoundData = null;
        base.Cleanup();
    }
}
