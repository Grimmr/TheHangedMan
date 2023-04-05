using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Enums
{
    public enum BattlePos
    {
        Atk,
        Def
    }

    public static class BattlePosExtentions
    {
        public static BattlePos ToBattlePos(this CardPosition pos)
        {
            if (pos == CardPosition.Defence || pos == CardPosition.FaceUpDefence || pos == CardPosition.FaceDownDefence )
            {
                return BattlePos.Def;
            }
            else
            {
                return BattlePos.Atk;
            }
        }

        public static BattlePos Change(this BattlePos pos)
        {
            if(pos == BattlePos.Atk)
            {
                return BattlePos.Def;
            }
            else
            {
                return BattlePos.Atk;
            }
        }
    }
}
