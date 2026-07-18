using Defines.Enums;
using Defines.Expressions;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    namespace Client
    {
        [Serializable]
        public class GroupInstance
        {
            List<List<Logic.IUnit>> Groups = new List<List<Logic.IUnit>>();
            public GroupInstance()
            {
                for (int i = 0; i < (int)Group.END; ++i)
                {
                    Groups.Add(new List<Logic.IUnit>());
                }
            }
            public void RegistGroup(Logic.IUnit content, Group To)
            {
                Groups[(int)To].Insert(0, content);
            }
            public void DeregistGroup(Logic.IUnit content, Group From)
            {
                if (From == Group.NONE)
                {
                    return;
                }
                Groups[(int)From].Remove(content);
            }
            public ERESULT IsInGroup(Logic.IUnit content, Group Dst)
            {
                if (false == Groups[(int)Dst].Contains(content))
                {
                    return ERESULT.FALSE;
                }
                return ERESULT.TRUE;
            }
            public ERESULT MoveGroup(Logic.IUnit content, Group Src, Group Dst)
            {
                if (ERESULT.FALSE == IsInGroup(content, Src))
                {
                    return ERESULT.FALSE;
                }
                content.DeregistGroup();
                content.RegisterGroup(Dst);
                return ERESULT.TRUE;
            }
            public ERESULT StartGame(out IUnit outPlayer)
            {
                int iCount = Groups[(int)Group.PLAYER].Count;
                if (iCount <= 0 || iCount > 1)
                {
                    outPlayer = null;
                    return ERESULT.FALSE;
                }
                outPlayer = Groups[(int)Group.PLAYER].First();
                return ERESULT.TRUE;
            }
        }
    }
}
