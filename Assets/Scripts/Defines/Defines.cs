using Core;
using Defines.Structures;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;

namespace Defines
{
    namespace Structures
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
                    Vector3 vTangent = Helpers.GetQuadraticBezierTangent(fRatio, m_StartInfo.vPosition, m_CenterInfo.vPosition, m_EndInfo.vPosition);
                    float fRotZ = (vTangent.sqrMagnitude > Defines.Constants.FLT_EPSILON5) ? Mathf.Atan2(vTangent.y, vTangent.x) * Mathf.Rad2Deg : 0f;
                    return new MoveInfo(
                        Helpers.GetQuadraticBezierPoint(fRatio, m_StartInfo.vPosition, m_CenterInfo.vPosition, m_EndInfo.vPosition),
                        Quaternion.Euler(0f, 0f, fRotZ)
                    );
                }
            }
            public float Ratio => m_vTimer.x / m_vTimer.y;
            public float Duration => m_vTimer.y;
            public bool IsLerping => m_vTimer.x != m_vTimer.y;

            public void Progress()
            {
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
    public static class Constants
    {
        public static readonly float FLT_EPSILON7 = 1.2E-7F;
        public static readonly float FLT_EPSILON5 = 1.2E-5F;

        public static readonly int TIME_MS_ASEC = 1000;
        public static readonly int TIME_MS_SORTING_TIMEOUT = 120;
        public static readonly int TIME_MS_DRAWING_DURATION = 300;
        public static readonly int TIME_MS_DISCARD_DURATION = 250;
        public static readonly int TIME_MS_DRAWING_INTERVAL = 200;

        public static readonly Vector2 TargetPC = new Vector2(1920, 1080);
        public static readonly Vector2 TargetMobile = new Vector2(1080, 2400);


        public const string s_strCardCanvas         = "CardCanvas";
        public const string s_strGamePlayCanvas     = "GamePlayCanvas";
        public const string s_strRoomRoot           = "RoomRoot";
        public const string s_strRoomCombat01       = "Room_Combat_01";
        public const string s_strRoomRest01         = "Room_Rest_01";
        public const string s_strRoomEvent01        = "Room_Event_01";
        public const string s_strRoomEventChest     = "evt_chest";
        public const string s_strRoomFlagBossGate   = "boss_gate";
    }
    public static class Helpers
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
        public static Sprite RequireAtlasSprite(SpriteAtlas pAtlas, string strName, string strLabel, [CallerMemberName] string strCalller = "")
        {
            if (null == pAtlas)
            {
                throw new System.InvalidOperationException(
                    $"{strCalller} {strLabel} atlas is null (sprite '{strName}').");
            }
            Sprite pSprite = pAtlas.GetSprite(strName);
            if (null == pSprite)
            {
                throw new System.InvalidOperationException(
                    $"{strCalller} {strLabel} sprite '{strName}' not found in atlas '{pAtlas.name}'.");
            }
            return pSprite;
        }
        public static void EmptyEvent() { }
        public static void EmptyEvent(Logic.Card.CardInstance pCard) { }
        public static void EmptyEvent(Logic.Room.Room pRoom) { }
        public static class BIT
        {
            static ulong ToUInt64<_Ty>(_Ty iValue) where _Ty : struct
            {
                if (iValue is Enum iEnum)
                {
                    return Convert.ToUInt64(iEnum);
                }

                return Convert.ToUInt64(iValue);
            }

            static _Ty FromUInt64<_Ty>(ulong iValue) where _Ty : struct
            {
                if (typeof(_Ty).IsEnum)
                {
                    return (_Ty)Enum.ToObject(typeof(_Ty), iValue);
                }

                return (_Ty)Convert.ChangeType(iValue, typeof(_Ty));
            }

            public static bool Has<_Ty>(_Ty iValue, _Ty iFlag) where _Ty : struct
            {
                return 0 != (ToUInt64(iValue) & ToUInt64(iFlag));
            }

            public static bool HasAll<_Ty>(_Ty iValue, _Ty iMask) where _Ty : struct
            {
                ulong iMaskValue = ToUInt64(iMask);
                return iMaskValue == (ToUInt64(iValue) & iMaskValue);
            }

            public static _Ty Set<_Ty>(_Ty iValue, _Ty iFlag) where _Ty : struct
            {
                return FromUInt64<_Ty>(ToUInt64(iValue) | ToUInt64(iFlag));
            }

            public static _Ty Clear<_Ty>(_Ty iValue, _Ty iFlag) where _Ty : struct
            {
                return FromUInt64<_Ty>(ToUInt64(iValue) & ~ToUInt64(iFlag));
            }

            public static _Ty Toggle<_Ty>(_Ty iValue, _Ty iFlag) where _Ty : struct
            {
                return FromUInt64<_Ty>(ToUInt64(iValue) ^ ToUInt64(iFlag));
            }

            public static bool IsNone<_Ty>(_Ty iValue) where _Ty : struct
            {
                return 0 == ToUInt64(iValue);
            }
        }
    }
}