using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
