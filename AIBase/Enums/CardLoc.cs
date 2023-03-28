using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Enums
{
    public enum CardLoc
    {
        Hand,
        MonsterZone,
        ExtraMonsterZone,
        SpellZone,
        FieldZone,
        Graveyard,
        Banished,
        Deck,
        ExtraDeck,
        Nowhere
    }

    public static class CardLocExtentions
    {
        public static CardLoc ToCardLoc(this CardLocation l)
        {
            switch (l) 
            { 
                case CardLocation.Deck: return CardLoc.Deck;
                case CardLocation.Hand: return CardLoc.Hand;
                case CardLocation.MonsterZone: return CardLoc.MonsterZone;
                case CardLocation.SpellZone: return CardLoc.SpellZone;
                case CardLocation.Grave: return CardLoc.Graveyard;
                case CardLocation.Extra: return CardLoc.ExtraDeck;
                case CardLocation.Removed: return CardLoc.Banished;
                case CardLocation.FieldZone: return CardLoc.FieldZone;
            }

            return CardLoc.Nowhere;
        }
    }
}
