using Defines.Expressions;

namespace Core
{
    namespace Client
    {
        public class PlayerInstance
        {
            Logic.IController m_pPlayerController = null;
            Logic.CPlayable m_pPlayerUnit = null;

            public ERESULT PlayerRegist(Logic.CPlayable playable)
            {
                if (null != m_pPlayerUnit)
                {
                    m_pPlayerUnit.DeregistGroup();
                }
                m_pPlayerUnit = playable;
                m_pPlayerUnit.RegisterGroup(Defines.Enums.Group.PLAYER);
                return ERESULT.TRUE;
            }
            public ERESULT StartGame()
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
