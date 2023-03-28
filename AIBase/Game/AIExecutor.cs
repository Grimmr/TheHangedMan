using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using WindBot.Game.AI;

namespace AIBase.Game
{
    public class AIExecutor : Executor
    {
        public AIExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
        }
    }
}
