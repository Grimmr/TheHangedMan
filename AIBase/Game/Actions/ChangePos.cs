using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    internal class ChangePos : Action
    {
        public AICard Monster;

        public ChangePos(AICard m)
        {
            Monster = m;
        }

        public override string ToString()
        {
            return "CHANGE_POS " + Monster.ToString();
        }
    }
}
