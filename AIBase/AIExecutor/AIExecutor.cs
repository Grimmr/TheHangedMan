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
                    state.PC++;
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
                return (int)Math.Pow(2, ((SelectZone)next).Zone);
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            if(state.GetNextAction() is DeclareAttack)
            {
                var ret = ((DeclareAttack)state.GetNextAction()).Attacker.source;
                state.PC++;
                return ret;
            }
            return null;
        }

        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (state.GetNextAction() is SelectAttackTarget)
            {
                var def = ((SelectAttackTarget)state.GetNextAction()).target != null? ((SelectAttackTarget)state.GetNextAction()).target.source : null;
                var atk = ((SelectAttackTarget)state.GetNextAction()).attacker.source;
                state.PC++;
                return AI.Attack(atk, def);
            }
            return null;
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
            foreach (AICard card in lhs.Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone])
            {
                if (card != null)
                {
                    lhsScore += card.Atk;
                    lhsCount++;
                }
            }
            foreach (AICard card in rhs.Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone])
            {
                if (card != null)
                {
                    rhsScore += card.Atk;
                    rhsCount++;
                }
            }

            Console.WriteLine("lhs ({0} {1} {2} {3}) - rhs ({4} {5} {6} {7})", lhs.Duel.Fields[Player.Enemy].LP, lhsCount, lhsScore, lhs.Actions.Count(), rhs.Duel.Fields[Player.Enemy].LP, rhsCount, rhsScore, rhs.Actions.Count());
            bool ret = false;
            if (rhs.Duel.Fields[Player.Enemy].LP < lhs.Duel.Fields[Player.Enemy].LP) { ret = true; }
            else if (rhs.Duel.Fields[Player.Enemy].LP > lhs.Duel.Fields[Player.Enemy].LP) { ret = false; }
            else if (rhs.Duel.Fields[Player.Bot].LP <= 0) { ret = false; }
            else if (rhsCount > lhsCount) { ret = true; }
            else if (rhsCount < lhsCount) { ret = false; }
            else if (rhsScore < lhsScore) { ret = true; }
            else if (rhsScore > lhsScore) { ret = false; }
            else if (rhs.Actions.Count() < lhs.Actions.Count()) { ret = true; }

            return ret;
        }

        private AIGameState ForceStateRefresh()
        {
            state = new AIGameState(Duel);
            state = SelectBestState(state.GenerateOptions());
            return state;
        }
    }
}
