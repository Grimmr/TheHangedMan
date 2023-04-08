using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    internal class NoAction : Action
    {
        public NoAction() { }

        public override string ToString()
        {
            return "NO_ACTION";
        }
    }
}
