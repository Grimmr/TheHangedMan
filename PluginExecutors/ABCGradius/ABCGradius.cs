using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("ABCGradius", "AI_ABCGradius")]
    public class SampleExecutor : DefaultExecutor
    {
        public enum CardID : int
        {
            PotOfExtravagance = 49238328,
            UnionHangar = 66399653,
            VicViperT301 = 10642488,
            GoldGadget = 55010259,
            SilverGadget = 29021114,
            LordBritishSpaceFighter = 35514096,
            VictoryViperXX03 = 93130021,
            BBusterDrake = 77411244,
            AAssaultCore = 30012506,
            CCrushWyvern = 03405259,
            BlueThunderT45 = 14089428,
            JadeKnight = 44364207,
            FalchionB = 86170989,
            DeltaTri = 12079734,
            Honest = 37742478
        }
        public SampleExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //Shotguns
            AddExecutor(ExecutorType.Activate, (int)CardID.PotOfExtravagance, POEeffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.VicViperT301);

            //SetJade
            AddExecutor(ExecutorType.MonsterSet, (int)CardID.JadeKnight, NormalSummonCheck);

            //Normals
            AddExecutor(ExecutorType.Summon, NormalSummonCheck);

            //Gadget Specials
            AddExecutor(ExecutorType.Activate, (int)CardID.SilverGadget, GadgetEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.GoldGadget, GadgetEffect);
            
        }

        public bool POEeffect()
        {
            //always try and draw 2
            AI.SelectOption(1);
            return true;
        }

        public bool NormalSummonCheck()
        {
            return Card.Id == (int)chooseFromHand(getMainMonsterPreferenceOrder());
        }

        public bool GadgetEffect()
        {
            IList<CardID> DefenceCards = new List<CardID> {CardID.JadeKnight, CardID.BBusterDrake, CardID.CCrushWyvern};
            
            CardID target = chooseFromHand(getMainMonsterPreferenceOrder());
            AI.SelectCard((int)target);
            if (DefenceCards.Contains(target)) { AI.SelectPosition(CardPosition.FaceUpDefence); }
            return true;
        }

        /// <summary>
        /// gets the prority order for summoning or searching monsters
        /// 
        /// 1)  gadget gold/silver
        /// 2)  lord british
        /// 3)  Victory Viper
        /// 4)  ABC not on field (B > C > A)
        /// 5)  Bluethunder
        /// 6)  jadeknight if not on field
        /// 7)  tri if it would get the final ABC part
        /// 8)  falchionB
        /// 9)  tri if it would not get the final ABC part
        /// 10) jadeknight if already on board
        /// 11) vic viper
        /// 12) ABC already on field ( B > C > A)
        /// 13) honest
        /// </summary>
        /// 
        /// <returns>ordered list of preference</returns>
        private IList<CardID> getMainMonsterPreferenceOrder()
        {
            IList<CardID> preference = new List<CardID>();

            //there is only one of each gadget so we don't care about clashes (eg: summoning gold off of gold)
            preference.Add(CardID.GoldGadget);
            preference.Add(CardID.SilverGadget);

            //lord british and victory viper have no complications
            preference.Add(CardID.LordBritishSpaceFighter);
            preference.Add(CardID.VictoryViperXX03);

            //each ABC part should only be here if it isn't already in play
            if (!Bot.HasInMonstersZone((int)CardID.BBusterDrake) && !Bot.HasInSpellZone((int)CardID.BBusterDrake)) { preference.Add(CardID.BBusterDrake); }
            if (!Bot.HasInMonstersZone((int)CardID.CCrushWyvern) && !Bot.HasInSpellZone((int)CardID.BBusterDrake)) { preference.Add(CardID.CCrushWyvern); }
            if (!Bot.HasInMonstersZone((int)CardID.AAssaultCore) && !Bot.HasInSpellZone((int)CardID.BBusterDrake)) { preference.Add(CardID.AAssaultCore); }

            //bluethunder is always wanted here
            preference.Add(CardID.BlueThunderT45);

            //jadeknight is wanted here if it's not on board already
            if (!Bot.HasInMonstersZone((int)CardID.JadeKnight)) { preference.Add(CardID.JadeKnight); }

            //tri is wanted if it would fetch the last ABC part (in this case the block above would only have added the one ABC part we need)
            IList<CardID> ABC = new List<CardID> { CardID.AAssaultCore, CardID.BBusterDrake, CardID.CCrushWyvern };
            IList<CardID> ABCWanted = new List<CardID>();
            foreach (CardID wanted in preference)
            {
                if (ABC.Contains(wanted)) { ABCWanted.Add(wanted); }
            }
            if (ABCWanted.Count == 1)
            {
                if (Bot.HasInGraveyard((int)ABCWanted[0]))
                {
                    preference.Add(CardID.DeltaTri);
                }
            }

            //FaclchionB is always wanted here
            preference.Add(CardID.FalchionB);

            //tri + jadeknight goes here if skipped earlyer
            if (!preference.Contains(CardID.DeltaTri)) { preference.Add(CardID.DeltaTri); }
            if (!preference.Contains(CardID.JadeKnight)) { preference.Add(CardID.JadeKnight); }

            //vicviper always goes here
            preference.Add(CardID.VicViperT301);

            //any duplicate ABC pieces 
            if (!preference.Contains(CardID.BBusterDrake)) { preference.Add(CardID.BBusterDrake); }
            if (!preference.Contains(CardID.CCrushWyvern)) { preference.Add(CardID.CCrushWyvern); }
            if (!preference.Contains(CardID.AAssaultCore)) { preference.Add(CardID.AAssaultCore); }

            //honest always goes here
            preference.Add(CardID.Honest);

            return preference;
        }

        //helpers refactor this into own project
        /// <summary>
        /// returns the first card in options that is also in the bots hand.
        /// </summary>
        /// <param name="options">orderd list of target cards</param>
        /// <returns>the selected card</returns>
        public CardID chooseFromHand(IList<CardID> options)
        {
            foreach (CardID card in options)
            {
                if(Bot.HasInHand((int)card))
                {
                    return card;
                }
            }
            //case: no prefered targets 
            return options[0];
        }
    }
}

