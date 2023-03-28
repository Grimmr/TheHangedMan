using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game.Actions
{
    public class GoToPhase : Action
    {
        public DuelPhase phase;

        public GoToPhase(DuelPhase p)
        {
            phase = p;
        }
    }
}
