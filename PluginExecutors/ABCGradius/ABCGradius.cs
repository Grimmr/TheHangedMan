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
            AddExecutor(ExecutorType.Activate, (int)CardID.UnionHangar);
            AddExecutor(ExecutorType.Activate, (int)CardID.VicViperT301);

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
            return Card.Id == chooseFromHand(getSummonFromHandPreferenceOrder());
        }

        public bool GadgetEffect()
        {
            AI.SelectCard(chooseFromHand(getSummonFromHandPreferenceOrder()));
            
            return true;
        }
        
        /// <summary>
        /// gets the prority order for summoning monsters from hand. This is just the special summon order but gold gadget is always first
        /// </summary>
        /// <returns>ordered list of preference</returns>
        private CardID[] getSummonFromHandPreferenceOrder()
        {
            CardID[] preference = getSpecialSummonFromHandPreferenceOrder();
            preference[0] = CardID.GoldGadget;
            return preference;
        }

        /// <summary>
        /// gets the prority order for special summoning monsters from hand
        /// </summary>
        /// <returns>ordered list of preference</returns>
        private CardID[] getSpecialSummonFromHandPreferenceOrder()
        {
            CardID[] preference = { CardID.GoldGadget, CardID.SilverGadget, CardID.LordBritishSpaceFighter, CardID.VictoryViperXX03, CardID.BBusterDrake, CardID.CCrushWyvern, CardID.AAssaultCore, CardID.BlueThunderT45, CardID.JadeKnight, CardID.FalchionB, CardID.DeltaTri, CardID.JadeKnight, CardID.VicViperT301, CardID.BBusterDrake, CardID.CCrushWyvern, CardID.AAssaultCore, CardID.AAssaultCore, CardID.Honest };
            //replace gold if activating Gold
            if (Card.Id == (int)CardID.GoldGadget) { preference[0] = CardID.SilverGadget; }

            //reorder ABC based on board state
            for (int i = 0; i < 2; i++)
            {
                //if current priority is already in play
                if (Bot.HasInMonstersZone((int)preference[4]) || Bot.HasInSpellZone((int)preference[4]))
                {
                    //shift blue thunder up destructivly
                    preference[4] = preference[5];
                    preference[5] = preference[6];
                    preference[6] = preference[7];
                }
            }

            //swap falchion and tri if tri effect would get the last part of ABC
            int ABCcount = 0;
            CardID[] ABC = { CardID.AAssaultCore, CardID.BBusterDrake, CardID.CCrushWyvern };
            //count unique ABC parts in grave or field
            foreach (CardID card in ABC) { if (Bot.HasInMonstersZoneOrInGraveyard((int)card) || Bot.HasInSpellZone((int)card)) { ABCcount++; } }
            //swap falchion and tri
            if (ABCcount == 2) { preference[9] = CardID.DeltaTri; preference[10] = CardID.FalchionB; }
            
            //move jade knight if already on board
            if (Bot.HasInMonstersZone((int)CardID.JadeKnight)) { preference[8] = preference[9]; }

            return preference;
        }

        //helpers refactor this into own project
        /// <summary>
        /// returns the first card in options that is also in the bots hand.
        /// </summary>
        /// <param name="options">orderd list of target cards</param>
        /// <returns>the selected card</returns>
        public int chooseFromHand(CardID[] options)
        {
            foreach (CardID card in options)
            {
                if(Bot.HasInHand((int)card))
                {
                    return (int)card;
                }
            }
            //case: no prefered targets 
            return (int)options[0];
        }
    }
}

