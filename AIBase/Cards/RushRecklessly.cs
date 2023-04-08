using AIBase.Enums;
using AIBase.Game;
using AIBase.Game.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Cards
{
    internal class RushRecklessly : AICard
    {
        public RushRecklessly(AICard copy) : base(copy) { }
        public RushRecklessly(ClientCard card) : base(card) { }

        protected override void createEffects()
        {
            CardEffect activation = new CardEffect();
            activation.Precondition = precondition;
            activation.PostConditionCost = postConditionCost;
            activation.PostConditionEffect = postConditionEffect;
            activation.Triggers = new List<EffectTrigger> { EffectTrigger.Activation };
            activation.Tags = new List<EffectTag> { EffectTag.AddFromDeckToHand };
            activation.Parent = this;

            Effects.Add(activation);
        }

        public bool precondition(AIGameState state)
        {
            //There must be one face up monster on the field
            var validTarget = false;
            foreach(var field in state.Duel.Fields)
            {
                foreach(var card in field.Value.Locations[CardLoc.MonsterZone])
                {
                    if(card != null && card.FaceUp)
                    {
                        validTarget = true;
                    }
                }
            }
            if(!validTarget) { return false; }

            //we must have an open spell trap slot
            for (int i = 0; i < 5; i++)
            {
                if (state.Duel.Fields[Owner].Locations[CardLoc.SpellZone][i] == null)
                {
                    return true;
                }
            }

            return false;
        }

        public IList<AIGameState> postConditionCost(AIGameState state)
        {
            var options = new List<AIGameState>();

            foreach (var initial in HelperSpellZoneSellect(state))
            {
                foreach (var target in state.Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone].Concat(state.Duel.Fields[Player.Enemy].Locations[CardLoc.MonsterZone]).ToList())
                {
                    if (target != null)
                    {
                        var option = new AIGameState(initial);
                        option.Actions.Add(new SelectTarget(target));
                        options.Add(option); 
                    }
                }
            }

            return options;
        }

        public IList<AIGameState> postConditionEffect(AIGameState state)
        {
            var options = new List<AIGameState>();

            //assume we are the top of the chain
            //cards drawn = option index + 1 (0: 3 discard, 1: 6 discard)
            var choice = (SelectTarget)state.Actions[state.Duel.CurrentChain.Last().Item3 + 1];
            var option = new AIGameState(state);
            return option.ComputeAttackUp(option.getCard(choice.target), 700);
        }
    }
}
