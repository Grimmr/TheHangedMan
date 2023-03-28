using AIBase.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Game
{
    public class CardEffect
    {
        public EffectTrigger Trigger;
        public Func<bool, AIGameState> Precondition;
        public Func<AIGameState[], AIGameState> PostConditionCost; //pre chain
        public Func<AIGameState[], AIGameState> PostConditionEffect; //activation in chain
    }

    public class AICard
    {
        public CardName TrueName;
        public CardName EffectiveName;

        public BattlePos Position;
        public bool FaceUp;
        public CardLoc Location;
        public int Level;
        public int Rank;
        public int LinkCount;
        public HashSet<Direction> LinkMarkers;
        public CardBasicType Type;
        public bool Effect;
        public bool Tuner;
        public MonsterAttribute Attr;
        public MonsterRace Race;
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

        protected AICard()
        {

        }
        
        public AICard(AICard copy)
        {
            TrueName = copy.TrueName;
            EffectiveName = copy.EffectiveName;
            Position = copy.Position;
            FaceUp = copy.FaceUp;
            Location = copy.Location;
            Level = copy.Level;
            Rank = copy.Rank;
            LinkCount = copy.LinkCount;
            LinkMarkers = new HashSet<Direction>();
            foreach (Direction dir in copy.LinkMarkers) { LinkMarkers.Add(dir); }
            Type = copy.Type;
            Effect = copy.Effect;
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
        }

        protected AICard(ClientCard card, ClientField field)
        {
            TrueName = (CardName)card.Id;
            EffectiveName = (CardName)card.Alias;
            Position = ((CardPosition)card.Position).ToBattlePos();
            FaceUp = ((CardPosition)card.Position == CardPosition.FaceUpAttack || (CardPosition)card.Position == CardPosition.FaceUpDefence);
            Location = field.GetMonstersInExtraZone().Contains(card) ? CardLoc.ExtraMonsterZone : card.Location.ToCardLoc();
            Level = card.Level;
            Rank = card.Rank;
            
            LinkCount = card.LinkCount;
            LinkMarkers = new HashSet<Direction>();
            foreach (CardLinkMarker d in new CardLinkMarker[]{ CardLinkMarker.BottomLeft, CardLinkMarker.Bottom, CardLinkMarker.BottomRight, CardLinkMarker.Left, CardLinkMarker.Right, CardLinkMarker.TopLeft, CardLinkMarker.Top, CardLinkMarker.TopRight })
            {
                if (card.HasLinkMarker((int)d)) { d.ToDirection(); }
            }

            //TODO card type and on

        }

        public virtual bool NormalSummonCondition()
        {
            return Type.isMonsterType() && Level <= 4;
        }
    }
}
