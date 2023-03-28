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
        public DuelPhase Phase;

        public GoToPhase(DuelPhase p)
        {
            Phase = p;
        }

        public override string ToString()
        {
            return "GOTO_PHASE " + Phase.ToString();
        }
    }
}
