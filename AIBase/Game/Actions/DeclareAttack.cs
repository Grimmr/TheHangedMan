using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class DeclareAttack : Action
    {
        public AICard Attacker; 
        public DeclareAttack(AICard a) 
        {
            Attacker = a;
        }

        public override string ToString()
        {
            return "DECLARE_ATTACK " + Attacker.ToString();
        }
    }
}
