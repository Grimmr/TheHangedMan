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
    internal class PotOfExtravagance : AICard
    {
        public PotOfExtravagance(AICard copy) : base(copy) { }
        public PotOfExtravagance(ClientCard card) : base(card) { }

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
            //we must be at the start of our main phase
            if( state.Actions.Count() > 0 && (!(state.Actions.Last() is  GoToPhase) || ((GoToPhase)state.Actions.Last()).Phase != DuelPhase.Main1))
            {
                return false;
            }
            
            //we must have at least 3 cards in the extra
            if (state.Duel.Fields[Owner].Locations[CardLoc.ExtraDeck].Count() < 3)
            { 
                return false; 
            }

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
            var ExtraCount = state.Duel.Fields[Owner].Locations[CardLoc.ExtraDeck].Count();

            for (int i = 0; i < 5; i++)
            {
                {
                    var option = new AIGameState(state);
                    option.Actions.Add(new SelectZone(i));
                    option.Actions.Add(new SelectOption(0));
                    option.Duel.Fields[Player.Bot].MoveCard(option.getCard(this), CardLoc.Hand, CardLoc.SpellZone, i);
                    option.setHiddeInfo(option.Actions.Count());
                    options.Add(option);
                }

                if (ExtraCount >= 6)
                {
                    var option = new AIGameState(state);
                    option.Actions.Add(new SelectZone(i));
                    option.Actions.Add(new SelectOption(1));
                    option.Duel.Fields[Player.Bot].MoveCard(option.getCard(this), CardLoc.Hand, CardLoc.SpellZone, i);
                    option.setHiddeInfo(option.Actions.Count());
                    options.Add(option);
                }
            }

            return options;
        }

        public IList<AIGameState> postConditionEffect(AIGameState state)
        {
            var options = new List<AIGameState>();

            //assume we are the top of the chain
            //cards drawn = option index + 1 (0: 3 discard, 1: 6 discard)
            var choice = (SelectOption)state.Actions[state.Duel.CurrentChain.Last().Item3+1];
            var option = new AIGameState(state);
            return option.ComputeDraw(Owner, choice.Choice + 1);
        }
    }
}
