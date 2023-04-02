using AIBase.Enums;
using AIBase.Game.Actions;
using AIBase.Game.Buffs;
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
        public EffectTrigger Trigger;
        public Func<bool, AIGameState> Precondition;
        public Func<AIGameState[], AIGameState> PostConditionCost; //pre chain
        public Func<AIGameState[], AIGameState> PostConditionEffect; //activation in chain
        public IList<EffectTag> tags;
        public AICard parent;
    }

    public class Influence
    {
        public Func<bool, AIGameState, AICard> CleanUpCondition;
        public IList<Buff> Buffs;
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
        public IList<Influence> Buffs;
        
        public AICard(AICard copy)
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

        public static AICard FromClientCard(ClientCard card, ClientField field)
        {
            return new AICard(card, field);
        }

        public AICard(ClientCard card, ClientField field)
        {
            source = card;
            
            TrueName = (CardName)card.Id;
            EffectiveName = (CardName)card.Alias;
            Position = ((CardPosition)card.Position).ToBattlePos();
            FaceUp = ((CardPosition)card.Position == CardPosition.FaceUpAttack || (CardPosition)card.Position == CardPosition.FaceUpDefence);
            Location = field.GetMonstersInExtraZone().Contains(card) ? CardLoc.ExtraMonsterZone : card.Location.ToCardLoc();
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

        }

        public virtual bool NormalSummonCondition(AIGameState state)
        {
            return Type.isMonsterType() && Level <= 4;
        }

        public virtual bool TributeSummonCondition(AIGameState state)
        {
            return Type == CardBasicType.Monster && Level >= 5;
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

        public override string ToString()
        {
            return TrueName.ToString();
        }
    }
}
