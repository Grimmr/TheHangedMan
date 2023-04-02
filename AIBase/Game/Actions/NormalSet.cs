using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class NormalSet : Action
    {
        public AICard Monster;
        public NormalSet(AICard t) 
        {
            Monster = t;
        }

        public override string ToString()
        {
            return "SET " + Monster.ToString();
        }

    }
}
