using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class FlipSummon : Action
    {
        public AICard Monster;
        
        public FlipSummon(AICard m)
        {
            Monster = m;
        }

        public override string ToString()
        {
            return "FLIP_SUMMON " + Monster.ToString();
        }
    }
}
