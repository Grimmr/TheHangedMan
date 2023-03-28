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
        public List<Tuple<int,AIDuel>> PreviousStates;
        public List<Action> Actions;
        public int PC;

        public AIGameState(AIGameState copy)
        {
            Actions = copy.Actions.ToList();
            PreviousStates = copy.PreviousStates.ToList();
            PreviousStates.Add(new Tuple<int, AIDuel>(Actions.Count(), copy.Duel));
            Duel = new AIDuel(copy.Duel);
            PC = copy.PC;
        }

        public AIGameState(Duel duel, bool normal=true)
        {
            PreviousStates = new List<Tuple<int, AIDuel>>();
            Duel = new AIDuel(duel);
            Actions = new List<Action>();
            PC = 0;
        }

        public IList<AIGameState> GenerateOptions()
        {
            IList<AIGameState> protoOptions = new List<AIGameState>();
    
            //consider cards in hand
            foreach(AICard card in Duel.Bot.Hand)
            {
                //normal summon
                if (Duel.TurnPlayer == Player.Bot && CanNormal() && card.Type.isMonsterType() && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count() == 0)
                {
                    protoOptions = protoOptions.Concat(GenerateNormalSummonsFromCard(card)).ToList();  
                }  
            }

            //consider PhaseChange options
            if(Duel.TurnPlayer == Player.Bot && Duel.CurrentChain.Count() == 0)
            {
                foreach(DuelPhase p in FollowPhases(Duel.Phase))
                {
                    AIGameState outcome = new AIGameState(this);
                    //only add an action if it makes sense
                    if (p == DuelPhase.BattleStart || p == DuelPhase.End || p == DuelPhase.Main2)
                    {
                        outcome.Actions.Add(new GoToPhase(p));
                    }
                    outcome.Duel.Phase = p;
                    protoOptions.Add(outcome);
                }
            }

            //if there are any follow states return them not these
            IList<AIGameState> options = new List<AIGameState>();
            foreach(AIGameState state in protoOptions)
            {
                IList<AIGameState> follow = state.GenerateOptions();
                if (follow.Count != 0)
                {
                    options = options.Concat(follow).ToList();
                }
                else
                {
                    options.Add(state);
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
                case DuelPhase.BattleStart: return new List<DuelPhase> { DuelPhase.BattleStep };
                case DuelPhase.BattleStep: return new List<DuelPhase> { DuelPhase.Main2, DuelPhase.End };
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

        public Action GetNextAction()
        {
            return Actions[PC];
        }

        private bool CanNormal()
        {
            for(int i = Actions.Count()-1; i >= 0; i--)
            {
                if (Actions[i] is NormalSummon)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
