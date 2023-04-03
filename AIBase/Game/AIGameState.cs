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
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    public class AIGameState
    {
        public AIDuel Duel;
        public List<Action> Actions;
        public int PC;
        public int HiddenInfo; //when PC == HiddenInfo we must regenerate our options

        public AIGameState(AIGameState copy)
        {
            Actions = copy.Actions.ToList();
            Duel = new AIDuel(copy.Duel);
            PC = copy.PC;
            HiddenInfo = copy.HiddenInfo;
        }

        public AIGameState(Duel duel)
        {
            Duel = new AIDuel(duel);
            Actions = new List<Action>();
            PC = 0;
            HiddenInfo = -1;
        }

        public IList<AIGameState> GenerateOptionsFromHidden(Duel duel)
        {
            Duel = new AIDuel(duel);
            PC = HiddenInfo;
            HiddenInfo = -1;
            for(int i = Actions.Count()-1; i >= PC; i--)
            {
                Actions.RemoveAt(i);
            }

            return GenerateOptions();
        }

        public IList<AIGameState> GenerateOptions()
        {
            IList<AIGameState> protoOptions = new List<AIGameState>();

            //our turn
            if (Duel.TurnPlayer == Player.Bot)
            {
                //consider cards in hand
                foreach (AICard card in Duel.Fields[Player.Bot].Locations[CardLoc.Hand])
                {
                    //normal summon
                    if (CanNormal() && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count() == 0)
                    {
                        protoOptions = protoOptions.Concat(GenerateNormalSummonsFromCard(card)).ToList();
                    }
                    //normal set
                    if (CanNormal() && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count() == 0)
                    {
                        protoOptions = protoOptions.Concat(GenerateNormalSetFromCard(card)).ToList();
                    }
                }

                //consider attacks
                if (Duel.Phase == DuelPhase.BattleStep && Duel.CurrentChain.Count() == 0)
                {
                    protoOptions = protoOptions.Concat(GenerateAttacks()).ToList();
                }

                //consider monsters on field
                foreach(AICard card in Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone])
                {
                    if (card != null)
                    {
                        //flip summons
                        if ((Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && Duel.CurrentChain.Count() == 0)
                        {
                            protoOptions = protoOptions.Concat(GenerateFlipSummonFromCard(card)).ToList();
                        } 
                    }
                }

                //consider PhaseChange options
                if (Duel.CurrentChain.Count() == 0)
                {
                    foreach (DuelPhase p in FollowPhases(Duel.Phase))
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
                case DuelPhase.Main1: return Duel.TurnCount > 1? new List<DuelPhase> { DuelPhase.BattleStart, DuelPhase.End } : new List<DuelPhase> { DuelPhase.End };
                case DuelPhase.BattleStart: return new List<DuelPhase> { DuelPhase.BattleStep };
                case DuelPhase.BattleStep: return new List<DuelPhase> { DuelPhase.Main2, DuelPhase.End };
                case DuelPhase.Main2: return new List<DuelPhase> { DuelPhase.End };
                default: return new List<DuelPhase>();
            }
        }

        public IList<AIGameState> GenerateNormalSummonsFromCard(AICard card)
        {
            IList<AIGameState> options = new List<AIGameState>();
             
            if(card.NormalSummonCondition(this) && Duel.Fields[Player.Bot].FreeMonsterZones() > 0)
            {
                for (int zone = 0; zone < 5; zone++)
                {
                    if (Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone][zone] == null)
                    {
                        options = options.Concat(ComputeNormalSummon(card, zone)).ToList();
                    }
                }
            }

            return options;
        }

        public IList<AIGameState> GenerateNormalSetFromCard(AICard card)
        {
            IList<AIGameState> options = new List<AIGameState>();

            if (card.NormalSetCondition(this) && Duel.Fields[Player.Bot].FreeMonsterZones() > 0)
            {
                for (int zone = 0; zone < 5; zone++)
                {
                    if (Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone][zone] == null)
                    {
                        options = options.Concat(ComputeNormalSet(card, zone)).ToList();
                    }
                }
            }

            return options;
        }

        public IList<AIGameState> GenerateFlipSummonFromCard(AICard card)
        {
            IList<AIGameState> options = new List<AIGameState>();

            if (card.FlipSummonCondition(this))
            {
                options = options.Concat(ComputeFlipSummon(card)).ToList();
            }

            return options;
        }

        public IList<AIGameState> GenerateAttacks()
        {
            IList<AIGameState> options = new List<AIGameState>();

            foreach(AICard attacker in Duel.Fields[Player.Bot].Locations[CardLoc.MonsterZone])
            {
                if (attacker != null && attacker.AttackCondition(this))
                {
                    //direct attack
                    if (Duel.Fields[Player.Enemy].FreeMonsterZones() == 7)
                    {
                        options = options.Concat(ComputeAttack(attacker, null)).ToList();
                    }
                    else
                    {
                        foreach(AICard defender in Duel.Fields[Player.Enemy].Locations[CardLoc.MonsterZone])
                        {
                            if(defender != null && defender.AttackTargetCondition(this))
                            {
                                options = options.Concat(ComputeAttack(attacker, defender)).ToList();
                            }
                        }
                    }
                }
            }

            return options;
        }

        public IList<AIGameState> ComputeNormalSummon(AICard target, int zone)
        {
            var initial = new AIGameState(this);
            initial.Actions.Add(new NormalSummon(target));
            initial.Actions.Add(new SelectZone(zone));
            initial.Duel.Fields[Player.Bot].MoveCard(initial.getCard(target), target.Location, CardLoc.MonsterZone, zone);
            initial.getCard(target).Location = CardLoc.MonsterZone;
            initial.getCard(target).FaceUp = true;
            initial.getCard(target).Position = BattlePos.Atk;

            return new List<AIGameState> { initial };
        }

        public IList<AIGameState> ComputeNormalSet(AICard target, int zone)
        {
            var initial = new AIGameState(this);
            initial.Actions.Add(new NormalSet(target));
            initial.Actions.Add(new SelectZone(zone));
            initial.Duel.Fields[Player.Bot].MoveCard(initial.getCard(target), target.Location, CardLoc.MonsterZone, zone);
            initial.getCard(target).Location = CardLoc.MonsterZone;
            initial.getCard(target).FaceUp = false;
            initial.getCard(target).Position = BattlePos.Def;

            return new List<AIGameState> { initial };
        }

        public IList<AIGameState> ComputeFlipSummon(AICard target)
        {
            var initial = new AIGameState(this);
            initial.Actions.Add(new FlipSummon(target));
            initial.getCard(target).FaceUp = true;
            initial.getCard(target).Position = BattlePos.Atk;

            return new List<AIGameState> { initial };
        }

        public IList<AIGameState> ComputeAttack(AICard atk, AICard def)
        {
            var initial = new AIGameState(this);
            initial.Actions.Add(new DeclareAttack(atk));
            initial.Actions.Add(new SelectAttackTarget(atk, def));

            var attacker = initial.getCard(atk);
            var defender = def != null? initial.getCard(def) : null;

            if(defender == null)
            {
                return initial.ComputeDamage(attacker.Controller.Not(), attacker.Atk);
            }
            else
            {
                //if target monster was face down flip it face up and assume we won and it was in def
                if(!defender.FaceUp)
                {
                    defender.FaceUp = true;
                    initial.setHiddeInfo(initial.Actions.Count());
                    return initial.ComputeDestruction(defender);
                }

                
                if(defender.Position == BattlePos.Atk)
                {
                    if(defender.Atk <= attacker.Atk)
                    {
                        var damageOutcomes = initial.ComputeDamage(defender.Controller, attacker.Atk - defender.Atk);
                        var destructionOutcomes = new List<AIGameState>();
                        foreach(AIGameState state in damageOutcomes)
                        {
                            destructionOutcomes = destructionOutcomes.Concat(state.ComputeDestruction(defender)).ToList();
                        }
                        return destructionOutcomes;
                    } 
                    else
                    {
                        var damageOutcomes = initial.ComputeDamage(attacker.Controller, defender.Atk - attacker.Atk);
                        var destructionOutcomes = new List<AIGameState>();
                        foreach (AIGameState state in damageOutcomes)
                        {
                            destructionOutcomes = destructionOutcomes.Concat(state.ComputeDestruction(attacker)).ToList();
                        }
                        return destructionOutcomes;
                    }
                }
                else
                {
                    if(defender.Def < attacker.Atk)
                    {
                        return initial.ComputeDestruction(defender);
                    }
                    else
                    {
                        return initial.ComputeDamage(attacker.Controller, defender.Def - attacker.Atk);
                    }
                }
            }
        }

        public IList<AIGameState> ComputeDestruction(AICard target)
        {
            return ComputeSendToGrave(target);
        }

        public IList<AIGameState> ComputeSendToGrave(AICard target)
        {
            var initial = new AIGameState(this);
            initial.Duel.Fields[target.Controller].MoveCard(initial.getCard(target), target.Location, CardLoc.Graveyard);
            initial.getCard(target).Location = CardLoc.Graveyard;
            return new List<AIGameState> { initial };
        }

        public IList<AIGameState> ComputeDamage(Player target, int v)
        {
            var initial = new AIGameState(this);
            initial.Duel.Fields[target].LP -= v;
            return new List<AIGameState> { initial };
        }

        public Action GetNextAction()
        {   
            return Actions[PC];
        }

        public AICard getCard(AICard target)
        {
            foreach(var field in Duel.Fields)
            {
                foreach(var location in field.Value.Locations)
                {
                    foreach(var card in location.Value)
                    {
                        if(card != null && target.source == card.source) { return card; }
                    }
                }
            }
            return null;
        }

        public void setHiddeInfo(int v)
        {
            if (v < HiddenInfo || HiddenInfo == -1) { HiddenInfo = v; }
        }

        private bool CanNormal()
        {
            for(int i = Actions.Count()-1; i >= 0; i--)
            {
                if (Actions[i] is NormalSummon || Actions[i] is NormalSet)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
