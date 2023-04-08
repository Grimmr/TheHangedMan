using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class SelectTarget : Action
    {
        public AICard target;
        public SelectTarget(AICard t)
        {
            target = t;
        }

        public override string ToString()
        {
            return "SELECT_TARGET " +  target.ToString();
        }
    }
}
