using AIBase.Enums;
using AIBase.Game;
using AIBase.Game.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.AIExecutor
{
    [Deck("AIExecutor", "AI_AIExecutor")]
    internal class AIExecutor : Executor
    {
        public AIGameState state;

        public AIExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            Executors.Clear();
            AddExecutor(ExecutorType.Summon, SummonCheck);
            AddExecutor(ExecutorType.GoToEndPhase, delegate () { return GotoPhaseCheck(DuelPhase.End); }) ;
            AddExecutor(ExecutorType.GoToBattlePhase, delegate () { return GotoPhaseCheck(DuelPhase.BattleStart); });
            AddExecutor(ExecutorType.GoToBattlePhase, delegate () { return GotoPhaseCheck(DuelPhase.BattleStart); });
            AddExecutor(ExecutorType.GoToMainPhase2, delegate () { return GotoPhaseCheck(DuelPhase.Main2); });
        }

        

        public bool SummonCheck()
        {
            if(state == null) { return false; }
            if (state.GetNextAction() is NormalSummon)
            {
                if (((NormalSummon)state.GetNextAction()).Monster.source == Card)
                {
                    state.PC++;
                    return true;
                }
            }
            return false;
        }

        public bool GotoPhaseCheck(DuelPhase p)
        {
            if (state == null) { return false; }
            if (state.GetNextAction() is GoToPhase)
            {
                if(((GoToPhase)state.GetNextAction()).Phase == p)
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnNewPhase()
        {
            //we just drew a card
            if (Duel.Phase == DuelPhase.Main1)
            {
                ForceStateRefresh();
            }
            base.OnNewPhase();
        }

        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available)
        {
            var next = state.GetNextAction();
            if(next is SelectZone)
            {
                state.PC++;
                Console.WriteLine("I'm placing a monster");
                return (int)Math.Pow(2, ((SelectZone)next).Zone);
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public virtual AIGameState SelectBestState(IList<AIGameState> options)
        {
            //if no options then no best state
            if (options.Count() == 0)
            {
                return null;
            }
            
            //just return most attack on board and the shortest chain of actions
            AIGameState bestState = options[0];
            Console.WriteLine("I have {0} options", options.Count());
            foreach (AIGameState state in options) 
            {
                if(CompareGameStateLT(bestState, state))
                {
                    bestState = state;
                }
            }

            Console.WriteLine("I chose a state with {0} actions", bestState.Actions.Count());
            foreach(var a in bestState.Actions)
            {
                Console.WriteLine(" - {0}", a.ToString());
            }

            return bestState;
        }

        public virtual bool CompareGameStateLT(AIGameState lhs, AIGameState rhs)
        {
            var lhsScore = 0;
            var lhsCount = 0;
            var rhsScore = 0;
            var rhsCount = 0;
            foreach (AICard card in lhs.Duel.Bot.MonsterZones)
            {
                if (card != null)
                {
                    lhsScore += card.Atk;
                    lhsCount++;
                }
            }
            foreach (AICard card in rhs.Duel.Bot.MonsterZones)
            {
                if (card != null)
                {
                    rhsScore += card.Atk;
                    rhsCount++;
                }
            }
           
            if (rhsCount > lhsCount) { return true; }
            if (rhsCount < lhsCount) { return false; }
            if (rhsScore < lhsScore) { return true; }
            if (rhsScore > lhsScore) { return false; }
            if (rhs.Actions.Count() < lhs.Actions.Count()) { return true; }

            return false;
        }

        private AIGameState ForceStateRefresh()
        {
            state = new AIGameState(Duel);
            state = SelectBestState(state.GenerateOptions());
            return state;
        }
    }
}
