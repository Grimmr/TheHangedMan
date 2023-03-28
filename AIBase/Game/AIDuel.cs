using AIBase.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    using ChainLink = Tuple<AICard, bool, AICard>;
    using Battle = Tuple<Player, bool, AICard, AICard>;
    public class AIDuel
    {
        public AIPlayerField Bot;
        public AIPlayerField Enemy; 

        public int TurnCount;
        public Player TurnPlayer;
        
        public DuelPhase Phase;
        public bool PhaseEnd;
        public Battle ActiveBattle;

        public Player LastChainPlayer;

        // the chain tupple is activated Card, targeting bool, target card (if bool is false then the activated card did not specificly target the targeting card)
        public IList<ChainLink> CurrentChain;

        public AIDuel(AIDuel copy)
        {
            Bot = new AIPlayerField(copy.Bot);
            Enemy = new AIPlayerField(copy.Enemy);
            TurnCount = copy.TurnCount;
            TurnPlayer = copy.TurnPlayer;
            Phase = copy.Phase;
            PhaseEnd = copy.PhaseEnd;
            ActiveBattle = new Battle(copy.ActiveBattle.Item1, copy.ActiveBattle.Item2, new AICard(copy.ActiveBattle.Item3), AICard(copy.ActiveBattle.Item4));
            LastChainPlayer = copy.LastChainPlayer;
            CurrentChain = new List<Tuple<AICard, bool, AICard>>();
            foreach(ChainLink link in copy.CurrentChain)
            {
                CurrentChain.Add(new ChainLink(new AICard(link.Item1), link.Item2, new AICard(link.Item1)));
            }
        }
    }
}
