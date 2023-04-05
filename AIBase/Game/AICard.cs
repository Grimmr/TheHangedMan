using AIBase.Cards;
using AIBase.Enums;
using AIBase.Game.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    public class CardEffect
    {
        public IList<EffectTrigger> Triggers;
        public Func<AIGameState, bool> Precondition;
        public Func<AIGameState, IList<AIGameState>> PostConditionCost; //pre chain
        public Func<AIGameState, IList<AIGameState>> PostConditionEffect; //activation in chain
        public Func<AIGameState, IList<AIGameState>> EffectCancel; //removal of effect (eg when an equip is destroyed so atk must be reduced again)
        public IList<EffectTag> Tags;
        public AICard Parent;
    }

    public class AICard
    {
        public ClientCard source;
        
        public CardName TrueName;
        public CardName EffectiveName;

        public BattlePos Position;
        public bool FaceUp;
        public CardLoc Location;
        public int Level;
        public int Rank;
        public int LinkCount;
        public int LScale;
        public int RScale;
        public HashSet<Direction> LinkMarkers;
        public CardBasicType Type;
        public bool Tuner;
        public bool Pend;
        public CardAttribute Attr;
        public CardRace Race;
        public int Atk;
        public int Def;
        public int BaseAtk;
        public int BaseDef;
        public IList<CardName> Overlays;
        public Player Owner;
        public Player Controller;
        public bool Negated;
        public SummonMethod SummonSource;

        public IList<CardEffect> Effects;

        public static AICard FromAICard(AICard card)
        {
            switch (card.TrueName)
            {
                case CardName.PotOfExtravagance:
                    return new PotOfExtravagance(card);
                default:
                    return new AICard(card);
            }
        }

        protected AICard(AICard copy)
        {
            source = copy.source;
            
            TrueName = copy.TrueName;
            EffectiveName = copy.EffectiveName;
            Position = copy.Position;
            FaceUp = copy.FaceUp;
            Location = copy.Location;
            Level = copy.Level;
            Rank = copy.Rank;
            LScale = copy.LScale;
            RScale = copy.RScale;
            LinkCount = copy.LinkCount;
            LinkMarkers = new HashSet<Direction>();
            foreach (Direction dir in copy.LinkMarkers) { LinkMarkers.Add(dir); }
            Type = copy.Type;
            Tuner = copy.Tuner;
            Attr = copy.Attr;
            Race = copy.Race;
            Atk = copy.Atk;
            Def = copy.Def;
            BaseAtk = copy.BaseAtk;
            BaseDef = copy.BaseDef;
            Overlays = copy.Overlays.ToList();
            Owner = copy.Owner;
            Controller = copy.Controller;
            Negated = copy.Negated;
            SummonSource = copy.SummonSource;
            Effects = copy.Effects;
        }

        public static AICard FromClientCard(ClientCard card)
        {
            switch ((CardName)card.Id)
            {
                case CardName.PotOfExtravagance:
                    return new PotOfExtravagance(card);
                default:
                    return new AICard(card);
            }
        }

        protected AICard(ClientCard card)
        {
            source = card;
            
            TrueName = (CardName)card.Id;
            EffectiveName = (CardName)card.Alias;
            Position = ((CardPosition)card.Position).ToBattlePos();
            FaceUp = ((CardPosition)card.Position == CardPosition.FaceUpAttack || (CardPosition)card.Position == CardPosition.FaceUpDefence);
            Location = card.Location.ToCardLoc();
            Level = card.Level;
            Rank = card.Rank;
            LScale = card.LScale;
            RScale = card.RScale;
            
            LinkCount = card.LinkCount;
            LinkMarkers = new HashSet<Direction>();
            foreach (CardLinkMarker d in new CardLinkMarker[]{ CardLinkMarker.BottomLeft, CardLinkMarker.Bottom, CardLinkMarker.BottomRight, CardLinkMarker.Left, CardLinkMarker.Right, CardLinkMarker.TopLeft, CardLinkMarker.Top, CardLinkMarker.TopRight })
            {
                if (card.HasLinkMarker((int)d)) { d.ToDirection(); }
            }

            Type = ((CardType)card.Type).ToCardBasicType();
            Tuner = ((CardType)card.Type | CardType.Tuner) == (CardType)card.Type;
            Race = (CardRace)card.Race;
            Attr = (CardAttribute)card.Attribute;
            Atk = card.Attack;
            Def = card.Defense;
            BaseAtk = card.BaseAttack;
            BaseDef = card.BaseDefense;
            
            Overlays = new List<CardName>();
            foreach (int id in card.Overlays)
            {
                Overlays.Add((CardName)id);
            }

            Owner = card.Owner == 0? Player.Bot : Player.Enemy; //this might need to flip
            Controller = card.Controller == 0? Player.Bot : Player.Enemy; //^^^
            Negated = card.Disabled != 0; //^^^
            SummonSource = card.IsSpecialSummoned ? SummonMethod.Special : SummonMethod.Normal;

            Effects = new List<CardEffect>();
            createEffects();

        }

        public virtual bool NormalSummonCondition(AIGameState state)
        {
            return Type.isMonsterType() && Level <= 4;
        }

        public virtual bool NormalSetCondition(AIGameState state)
        {
            return Type.isMonsterType() && Level <= 4;
        }

        public virtual bool FlipSummonCondition(AIGameState state)
        {
            if (!Type.isMonsterType() || FaceUp)
            {
                return false;
            }

            for (int i = state.Actions.Count() - 1; i >= 0; i--)
            {
                if (state.Actions[i] is NormalSet)
                {
                    if (((NormalSet)state.Actions[i]).Monster.source == this.source)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public virtual bool TributeSummonCondition(AIGameState state)
        {
            return Type == CardBasicType.Monster && Level >= 5;
        }

        public virtual bool ChangePosCondition(AIGameState state)
        {
            if (!Type.isMonsterType() || !FaceUp)
            {
                return false;
            }

            for (int i = state.Actions.Count() - 1; i >= 0; i--)
            {
                if (state.Actions[i] is ChangePos)
                {
                    if (((ChangePos)state.Actions[i]).Monster.source == this.source)
                    {
                        return false;
                    }
                }

                if (state.Actions[i] is NormalSummon)
                {
                    if (((NormalSummon)state.Actions[i]).Monster.source == this.source)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual IList<List<AICard>> GetTributeOptions(AIGameState state)
        {
            var ret = new List<List<AICard>>();
            if (Level <= 4)
            {
                return ret;
            }
            if (Level >= 5 && Level <= 6)
            {
                foreach(AICard card in state.Duel.Fields[Controller].Locations[CardLoc.MonsterZone])
                {
                    ret.Add(new List<AICard> { card });
                }
            }
            else //Level >= 7
            {
                foreach (AICard cardA in state.Duel.Fields[Controller].Locations[CardLoc.MonsterZone])
                {
                    foreach (AICard cardB in state.Duel.Fields[Controller].Locations[CardLoc.MonsterZone])
                    {
                        if(cardA != cardB)
                        {
                            ret.Add(new List<AICard> { cardA, cardB });
                        }
                    }
                }
            }

            return ret;
        }

        public virtual bool AttackCondition(AIGameState state)
        {
            if (!FaceUp || Position == BattlePos.Def) { return false;  }
            
            for (int i = state.Actions.Count() - 1; i >= 0; i--)
            {
                if (state.Actions[i] is DeclareAttack)
                {
                    if (((DeclareAttack)state.Actions[i]).Attacker.source == this.source)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public virtual bool AttackTargetCondition(AIGameState state)
        {
            return true;
        }

        public virtual bool IsEffectMonster(AIGameState state)
        {
            return Type.isMonsterType() && Effects.Count > 0;
        }

        protected virtual void createEffects()
        {

        }

        public IList<AIGameState> nil(AIGameState state)
        {
            return new List<AIGameState> { state };
        }

        public override string ToString()
        {
            return TrueName.ToString();
        }
    }
}
