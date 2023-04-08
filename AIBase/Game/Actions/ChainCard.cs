using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class ChainCard : Action
    {
        public AICard Card;
        public int Effect;

        public ChainCard(AICard card, int effect)
        {
            Card = card;
            Effect = effect;
        }

        public override string ToString()
        {
            return "CHAIN_CARD " + Card.ToString() + " " + Effect.ToString();
        }
    }
}
