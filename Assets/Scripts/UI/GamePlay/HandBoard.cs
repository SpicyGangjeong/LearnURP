using System.Collections.Generic;
using UnityEngine;

public class HandBoard : MonoBehaviour
{
    private const int MAX_HAND_SLOTS = 10;
    [SerializeField] private int _maxSlots = MAX_HAND_SLOTS;
    [SerializeField] private float _curveHeight = 120f;
    CGameInstance gameInstance = null;
    IReadOnlyList<Card> handCards = null;
    [SerializeField] private int iHand = 0;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _controlPos;
    private Vector3[] _handPos;

    private int iCurrentHand = 10;

    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        _startPos = Vector3.Lerp(corners[0], corners[1], 0.5f); // 좌측 중앙
        _endPos = Vector3.Lerp(corners[3], corners[2], 0.5f);   // 우측 중앙
        Vector3 centerPos = Vector3.Lerp(_startPos, _endPos, 0.5f);
        _controlPos = centerPos + Vector3.up * _curveHeight;

        CalcHandPos(centerPos);
    }

    private void CalcHandPos(Vector3 centerPos)
    {
        _handPos = new Vector3[Mathf.Max(1, _maxSlots)];
        if (iCurrentHand == 1)
        {
            _handPos[0] = centerPos;
            return;
        }

        for (int i = 0; i < iCurrentHand; i++)
        {
            float t = (float)i / (iCurrentHand - 1);
            _handPos[i] = GetQuadraticBezierPoint(t, _startPos, _controlPos, _endPos);
        }

        return;
    }

    public Vector3 GetSlotPosition(int index)
    {
        if (_handPos == null || _handPos.Length == 0)
            return transform.position;

        index = Mathf.Clamp(index, 0, _handPos.Length - 1);
        return _handPos[index];
    }
    private Vector3 GetQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
    }

    void Start()
    {
        gameInstance = CGameInstance.Instance;
        if (null == handCards)
        {
            handCards = gameInstance.GetCards(DEFINES.CardPile.HAND);
        }
    }
    private void Update()
    {
        iHand = handCards.Count;
    }
}
