using AIBase.Enums;
using AIBase.Game.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    public class AIGameState
    {
        public AIDuel Duel;
        public List<AIDuel> PreviousStates;
        public List<Action> Actions;
        public int NormalAvailible;

        public AIGameState(AIGameState copy)
        {
            PreviousStates.Add(copy.Duel);
            Duel = new AIDuel(copy.Duel);
            Actions = copy.Actions.ToList();
            NormalAvailible = copy.NormalAvailible;
        }

        public IList<AIGameState> GenerateOptions()
        {
            IList<AIGameState> options = new List<AIGameState>();
    
            //consider cards in hand
            foreach(AICard card in Duel.Bot.Hand)
            {
                //normal summon
                if (NormalAvailible >= 1 && card.Type.isMonsterType() && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count() == 0)
                { 
                    options.Concat(GenerateNormalSummonsFromCard(card));  
                }  
            }

            //consider PhaseChange options
            if(Duel.ActiveBattle == null && Duel.CurrentChain.Count() == 0)
            {
                foreach(DuelPhase p in FollowPhases(Duel.Phase))
                {
                    AIGameState outcome = new AIGameState(this);
                    outcome.Actions.Add(new GoToPhase(p));
                    outcome.Duel.Phase = p;
                    options.Add(outcome);
                }
            }


            return options;
        }

        public IList<DuelPhase> FollowPhases(DuelPhase current)
        {
            switch (current)
            {
                case DuelPhase.Draw: return new List<DuelPhase> { DuelPhase.Standby };
                case DuelPhase.Standby: return new List<DuelPhase> { DuelPhase.Main1 };
                case DuelPhase.Main1: return new List<DuelPhase> { DuelPhase.BattleStart, DuelPhase.End };
                case DuelPhase.BattleStart: return new List<DuelPhase> { DuelPhase.Main2, DuelPhase.End };
                case DuelPhase.Main2: return new List<DuelPhase> { DuelPhase.End };
                default: return new List<DuelPhase>();
            }
        }

        public IList<AIGameState> GenerateNormalSummonsFromCard(AICard card)
        {
            IList<AIGameState> options = new List<AIGameState>();
            //non-tribute case
            if(card.Level <= 4 && card.NormalSummonCondition() && Duel.Bot.FreeMonsterZones() > 0)
            {
                //Face up atk summon
                for (int zone = 0; zone < 5; zone++)
                {
                    if (Duel.Bot.MonsterZones[zone] == null)
                    {
                        AIGameState outcome = new AIGameState(this);
                        outcome.Actions.Add(new NormalSummon(card));
                        outcome.Actions.Add(new SelectZone(zone));
                        outcome.Duel.Bot.Hand.Remove(card);
                        outcome.Duel.Bot.MonsterZones[zone] = card;
                        card.Location = CardLoc.MonsterZone;
                        card.FaceUp = true;
                        card.Position = BattlePos.Atk;

                        options.Add(outcome);
                    }
                }
            }

            return options;
        }
    }
    
}
