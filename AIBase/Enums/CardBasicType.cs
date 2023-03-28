using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Enums
{
    public enum CardBasicType
    {
        NormalMonster,
        EffectMonster,
        SpiritMonster,
        ToonMonster,
        UnionMonster,
        GeminiMonster,
        FlipMonster,
        RitualMonster,
        FusionMonster,
        SynchroMonster,
        XyzMonster,
        PendulumMonster,
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
        public static bool isMonsterType(this CardBasicType t)
        {
            switch (t)
            {
                case CardBasicType.NormalMonster:
                case CardBasicType.EffectMonster:
                case CardBasicType.SpiritMonster:
                case CardBasicType.ToonMonster:
                case CardBasicType.UnionMonster:
                case CardBasicType.GeminiMonster:
                case CardBasicType.FlipMonster:
                case CardBasicType.RitualMonster:
                case CardBasicType.FusionMonster:
                case CardBasicType.SynchroMonster:
                case CardBasicType.XyzMonster:
                case CardBasicType.PendulumMonster:
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
