using System;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("ABCGradius", "AI_ABCGradius")]
    public class SampleExecutor : DefaultExecutor
    {
        public enum CardID : int
        {
            PotOfExtravagance = 49238328,
        }
        public SampleExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //Shotguns
            AddExecutor(ExecutorType.Activate, (int)CardID.PotOfExtravagance, POEeffect);
        }

        public bool POEeffect()
        {
            //always try and draw 2
            AI.SelectOption(1);
            return true;
        }
    }
}

