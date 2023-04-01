using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public  class SelectAttackTarget : Action
    {
        public AICard attacker;
        public AICard target;
        public SelectAttackTarget(AICard a, AICard t) 
        {
            attacker = a;
            target = t;
        }

        public override string ToString()
        {
            return "SELECT_ATTACK_TARGET " + attacker.ToString() + " " + (target != null? target.ToString() : "direct");
        }
    }
}
