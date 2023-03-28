using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class SelectZone : Action
    {
        public int Zone;

        public SelectZone(int z)
        {
            Zone = z;
        }

        public override string ToString()
        {
            return "SELECT_ZONE " + Zone.ToString();
        }
    }
}
