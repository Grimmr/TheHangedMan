using AIBase.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    using ChainLink = Tuple<AICard, CardEffect, AICard>;
    using Battle = Tuple<Player, AICard, AICard>;
    public class AIDuel
    {
        public Dictionary<Player, AIPlayerField> Fields;

        public int TurnCount;
        public Player TurnPlayer;
        
        public DuelPhase Phase;
        public bool PhaseEnd;
        public Player AttackingPlayer;

        public Player LastChainPlayer;

        // the chain tupple is activated Card, targeting bool, target card (if bool is false then the activated card did not specificly target the targeting card)
        public IList<ChainLink> CurrentChain;

        public AIDuel(AIDuel copy)
        {
            Fields = new Dictionary<Player, AIPlayerField>();
            Fields[Player.Bot] = new AIPlayerField(copy.Fields[Player.Bot]);
            Fields[Player.Enemy] = new AIPlayerField(copy.Fields[Player.Enemy]);
            TurnCount = copy.TurnCount;
            TurnPlayer = copy.TurnPlayer;
            Phase = copy.Phase;
            PhaseEnd = copy.PhaseEnd;
            AttackingPlayer = copy.AttackingPlayer;
            LastChainPlayer = copy.LastChainPlayer;
            CurrentChain = new List<ChainLink>();
            foreach(ChainLink link in copy.CurrentChain)
            {
                CurrentChain.Add(new ChainLink(new AICard(link.Item1), link.Item2, new AICard(link.Item1)));
            }
        }

        public AIDuel(Duel duel)
        {
            Fields = new Dictionary<Player, AIPlayerField>();
            Fields[Player.Bot] = new AIPlayerField(duel.Fields[0]); //might need swapping vvvvv
            Fields[Player.Enemy] = new AIPlayerField(duel.Fields[1]); //might need swapping ^^^
            TurnCount = duel.Turn;
            TurnPlayer = duel.Player == 0 ? Player.Bot : Player.Enemy; //might need swaping
            Phase = duel.Phase;
            PhaseEnd = duel.MainPhaseEnd;
            AttackingPlayer = duel.Fields[0].UnderAttack ? Player.Enemy : Player.Bot;
            LastChainPlayer = duel.LastChainPlayer == 0 ? Player.Bot : Player.Enemy;

            //don't copy over chain as I don't understand the structure in windbot duel
            CurrentChain = new List<ChainLink>();
        }
    }
}
