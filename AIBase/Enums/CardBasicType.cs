using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YGOSharp.OCGWrapper.Enums;

namespace AIBase.Enums
{
    public enum CardBasicType
    {
        Monster,
        SpiritMonster,
        ToonMonster,
        UnionMonster,
        //GeminiMonster,
        FlipMonster,
        RitualMonster,
        FusionMonster,
        SynchroMonster,
        XyzMonster,
        LinkMonster,
        Token,

        NormalSpell,
        ContinuousSpell,
        EquipSpell,
        QuickPlaySpell,
        FieldSpell,
        RitualSpell,

        NormalTrap,
        ContinuousTrap,
        CounterTrap
    }

    public static class CardBasicTypeExtentions
    {
        public static CardBasicType ToCardBasicType(this CardType t)
        {
            if(t == (CardType.Normal | CardType.Monster) || t == (CardType.Effect | CardType.Monster))
            {
                return CardBasicType.Monster;
            }

            if(t == (t | CardType.Spirit))
            {
                return CardBasicType.SpiritMonster;
            }

            if(t == (t | CardType.Toon))
            {
                return CardBasicType.ToonMonster;
            }

            if(t == (t | CardType.Union))
            {
                return CardBasicType.UnionMonster;
            }

            if(t == (t | CardType.Flip))
            {
                return CardBasicType.FlipMonster;
            }

            if(t == (t | CardType.Ritual))
            {
                return CardBasicType.RitualMonster;
            }

            if(t == (t | CardType.Fusion))
            {
                return CardBasicType.FusionMonster;
            }

            if(t == (t | CardType.Synchro))
            {
                return CardBasicType.SynchroMonster;
            }

            if(t == (t | CardType.Xyz))
            {
                return CardBasicType.XyzMonster;
            }

            if(t == (t | CardType.Link))
            {
                return CardBasicType.LinkMonster;
            }

            if(t == (t | CardType.Token))
            {
                return CardBasicType.Token;
            }

            if(t == (CardType.Spell | CardType.Normal))
            {
                return CardBasicType.NormalSpell;
            }

            if(t == (CardType.Spell | CardType.Continuous))
            {
                return CardBasicType.ContinuousSpell;
            }

            if(t == (CardType.Spell | CardType.Equip))
            {
                return CardBasicType.EquipSpell;
            }

            if(t == (CardType.Spell | CardType.QuickPlay))
            {
                return CardBasicType.QuickPlaySpell;
            }

            if(t == (CardType.Spell | CardType.Ritual))
            {
                return CardBasicType.RitualSpell;
            }

            if(t == (CardType.Trap | CardType.Normal))
            {
                return CardBasicType.NormalTrap;
            }

            if(t == (CardType.Trap | CardType.Continuous))
            {
                return CardBasicType.ContinuousTrap;
            }

            if(t == (CardType.Trap | CardType.Counter))
            {
                return CardBasicType.CounterTrap;
            }

            return CardBasicType.Token;
        }
        
        public static bool isMonsterType(this CardBasicType t)
        {
            switch (t)
            {
                case CardBasicType.Monster:
                case CardBasicType.SpiritMonster:
                case CardBasicType.ToonMonster:
                case CardBasicType.UnionMonster:
                case CardBasicType.FlipMonster:
                case CardBasicType.RitualMonster:
                case CardBasicType.FusionMonster:
                case CardBasicType.SynchroMonster:
                case CardBasicType.XyzMonster:
                case CardBasicType.LinkMonster:
                case CardBasicType.Token:
                    return true;

                default: return false;
            }
        }

        public static bool isSpellType(this CardBasicType t)
        {
            switch (t)
            {
                case CardBasicType.NormalSpell:
                case CardBasicType.ContinuousSpell:
                case CardBasicType.EquipSpell:
                case CardBasicType.QuickPlaySpell:
                case CardBasicType.FieldSpell:
                case CardBasicType.RitualSpell:
                    return true;

                default: return false;
            }
        }

        public static bool isTrapType(this CardBasicType t)
        { 
            switch (t)
            {
                case CardBasicType.NormalTrap:
                case CardBasicType.ContinuousTrap:
                case CardBasicType.CounterTrap:
                    return true;

                default: return false;
            }

        }
    }
}
