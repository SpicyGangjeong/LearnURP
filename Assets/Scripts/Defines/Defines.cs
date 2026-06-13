using DEFINES.STRUCTURES;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
namespace DEFINES
{
    namespace STRUCTURES
    {
        public struct MoveInfo
        {
            public Quaternion vRotQ;
            public Vector3 vPosition;

            public MoveInfo(Vector3 vPosition, Quaternion vRotQ) : this()
            {
                this.vPosition = vPosition;
                this.vRotQ = vRotQ;
            }
            public static readonly MoveInfo identity = new MoveInfo(Vector3.zero, Quaternion.identity);
        }
        public struct LerpInfo
        {
            private Vector2 m_vTimer;
            private ILerp m_pLerpModel;
            private event LerpModelCallback m_Callback;
            public static LerpInfo Linear(float fDuration, in MoveInfo pStart, in MoveInfo pEnd, LerpModelCallback callback)
            {
                return new LerpInfo(fDuration, pStart, pEnd, callback);
            }
            public static LerpInfo Bezier(float fDuration, in MoveInfo pStart, in MoveInfo pCenter, in MoveInfo pEnd, LerpModelCallback callback)
            {
                return new LerpInfo(fDuration, pStart, pCenter, pEnd, callback);
            }
            private LerpInfo(float fDuration, in MoveInfo pStart, in MoveInfo pEnd, LerpModelCallback callback)
            {
                m_vTimer = Vector2.up * fDuration;
                m_pLerpModel = new LinearLerpInfo(in pStart, in pEnd);
                m_Callback = callback;
            }
            private LerpInfo(float fDuration, in MoveInfo pStart, in MoveInfo pCenter, in MoveInfo pEnd, LerpModelCallback callback)
            {
                m_vTimer = Vector2.up * fDuration;
                m_pLerpModel = new QuadraticLerpInfo(in pStart, in pCenter, in pEnd);
                m_Callback = callback;
            }
            public LerpInfo(in LerpInfo other)
            {
                m_vTimer = other.m_vTimer;
                m_pLerpModel = other.m_pLerpModel;
                m_Callback = other.m_Callback;
            }
            private interface ILerp
            {
                public MoveInfo GetMoveInfo(float fRatio);
            }
            private struct LinearLerpInfo : ILerp
            {
                MoveInfo m_StartInfo;
                MoveInfo m_EndInfo;
                public LinearLerpInfo(in MoveInfo startInfo, in MoveInfo endInfo)
                {
                    m_StartInfo = startInfo;
                    m_EndInfo = endInfo;
                }
                public MoveInfo GetMoveInfo(float fRatio)
                {
                    return new MoveInfo(
                        Vector3.Lerp(m_StartInfo.vPosition, m_EndInfo.vPosition, fRatio),
                        Quaternion.Slerp(m_StartInfo.vRotQ, m_EndInfo.vRotQ, fRatio)
                    );
                }
            }
            private struct QuadraticLerpInfo : ILerp
            {
                MoveInfo m_StartInfo;
                MoveInfo m_CenterInfo;
                MoveInfo m_EndInfo;
                public QuadraticLerpInfo(in MoveInfo startInfo, in MoveInfo centerInfo, in MoveInfo endInfo)
                {
                    m_StartInfo = startInfo;
                    m_CenterInfo = centerInfo;
                    m_EndInfo = endInfo;
                }
                public MoveInfo GetMoveInfo(float fRatio)
                {
                    Vector3 vTangent = HELPERS.GetQuadraticBezierTangent(fRatio, m_StartInfo.vPosition, m_CenterInfo.vPosition, m_EndInfo.vPosition);
                    float fRotZ = (vTangent.sqrMagnitude > DEFINES.CONSTANTS.FLT_EPSILON5) ? Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg : 0f;
                    return new MoveInfo(
                        HELPERS.GetQuadraticBezierPoint(fRatio, m_StartInfo.vPosition, m_CenterInfo.vPosition, m_EndInfo.vPosition),
                        Quaternion.Euler(0f, 0f, fRotZ)
                    );
                }
            }
            public float Ratio => m_vTimer.x / m_vTimer.y;
            public float Duration => m_vTimer.y;
            public bool IsLerping => m_vTimer.x != m_vTimer.y;

            public void Progress(){
                if (true == IsLerping)
                {
                    m_vTimer.x += Time.deltaTime;
                    if (m_vTimer.x >= m_vTimer.y)
                    {
                        SetFinish();
                    }
                }
            }
            public MoveInfo GetMoveInfo()
            {
                return m_pLerpModel.GetMoveInfo(Ratio);
            }
            public void SetFinish()
            {
                m_vTimer.x = m_vTimer.y;
                m_Callback();
            }
        }
    }
    public static class CONSTANTS
    {
        public static readonly float FLT_EPSILON7 = 1.2E-7F;
        public static readonly float FLT_EPSILON5 = 1.2E-5F;

        public static readonly float TIME_DISCARD = 0.5f;
    }
    public static class HELPERS
    {
        public static Vector3 GetQuadraticBezierPoint(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return fOneMinusT * fOneMinusT * p0 + 2f * fOneMinusT * fT * p1 + fT * fT * p2;
        }
        public static Vector3 GetQuadraticBezierTangent(float fT, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float fOneMinusT = 1f - fT;
            return 2f * fOneMinusT * (p1 - p0) + 2f * fT * (p2 - p1);
        }

        public static void ApplyMoveInfo(in MoveInfo moveinfo, Transform pTransform)
        {
            pTransform.SetPositionAndRotation(moveinfo.vPosition, moveinfo.vRotQ);
        }
        public static void ExtractMoveInfo(out MoveInfo moveinfo, Transform pTransform)
        {
            moveinfo.vPosition = pTransform.position;
            moveinfo.vRotQ = pTransform.rotation;
        }
        public static void EmptyEvent() { }
        public static void EmptyEvent(Card pCard) { }
        public static void EmptyEvent(Card pCard, CardCanvas pCardCanvas) { }
    }
    namespace ENUMS
    {
        enum GamePlayCanvasPvtPos : int
        {
            DECK=0, 
            DISCARD=1,
            DISAPPEAR=2,
            LEFT=3,
            RIGHT=4,
            HANDBOARD=5,
            END=6,
        }
        enum SystemState : int
        {
            NONE = -1,
            INITIALIZE = 0,
            IDLE = 1,
            PLAYING = 2,
            END = 3,
        }
        public enum SceneID
        {
            NONE = -1,
            MAIN_MENU = 0,
            GAME_PLAY = 1,
            END,
        }
        public enum CardPile
        {
            NONE = -1,
            HAND = 0,
            DISCARD = 1,
            DECK = 2,
            DISAPPEARED = 3,
            FIELD = 4,
            ALL = 5,
            END = 6,
        }
        public enum CardType : int
        {
            NONE = -1,
            ATTACK = 0,
            DEFENSE = 1,
            MAGIC = 2,
            ITEM = 3,
            END = 4,
        }
        public enum CardEffectTriggerType
        {
            NONE = -1,
            DRAW = 0,
            PLAY = 1,
            DISCARD = 2,
            RETURN = 3,
            DISAPPEAR = 4,
            SHUFFLE = 5,
            END = 6,
        }
        public enum CardEffectTargetType
        {
            NONE = -1,
            SELF = 0,
            SELECTED = 1,
            ALL = 2,
            CARD = 3,
            END,
        }
        public enum CardEffectValueType
        {
            NONE = -1,
            DAMAGE = 0,
            HEAL = 1,
            SHIELD = 2,
            BUFF = 3,
            DEBUFF = 4,
            END = 5,
        }
    }
}