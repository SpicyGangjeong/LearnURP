using Defines.Expressions;
using Logic;

namespace Core
{
    namespace Client
    {
        public class PlayerInstance
        {
            IController m_pPlayerController = null;
            CPlayable m_pPlayerUnit = null;

            public CPlayable PlayerUnit => m_pPlayerUnit;

            public ERESULT PlayerRegist(CPlayable playable)
            {
                if (null != m_pPlayerUnit)
                {
                    m_pPlayerUnit.DeregistGroup();
                }
                m_pPlayerUnit = playable;
                m_pPlayerUnit.RegisterGroup(Defines.Enums.Group.PLAYER);
                return ERESULT.TRUE;
            }
            public ERESULT StartFieldLevel()
            {
                if (null != m_pPlayerUnit || null != m_pPlayerController)
                {
                    m_pPlayerController.BindUnit(m_pPlayerUnit);
                    return ERESULT.TRUE;
                }
                else
                {
                    return ERESULT.FALSE;
                }
            }
            public void BindController(Logic.IController controller)
            {
                m_pPlayerController = controller;
            }
        }
    }
}
