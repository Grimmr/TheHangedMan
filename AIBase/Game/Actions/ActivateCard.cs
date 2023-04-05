using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class ActivateCard : Action
    {
        public AICard Card;
        public int Effect;

        public ActivateCard(AICard card, int effect)
        {
            Card = card;
            Effect = effect;
        }

        public override string ToString()
        {
            return "ACTIVATE_CARD " + Card.ToString() + " " + Effect.ToString();
        }
    }
}
