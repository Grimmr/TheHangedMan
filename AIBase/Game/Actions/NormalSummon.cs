using AIBase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class NormalSummon : Action
    {
        public AICard Monster;

        public NormalSummon(AICard m)
        {
            Monster = m;
        }

        public override string ToString()
        {
            return "NORMAL " + Monster.ToString();
        }
    }
}
