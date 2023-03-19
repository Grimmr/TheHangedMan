using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using static WindBot.Game.AI.Decks.SampleExecutor;

namespace WindBot.Game.AI.Decks
{
    [Deck("ABCGradius", "AI_ABCGradius")]
    public class SampleExecutor : DefaultExecutor
    {
        private bool requestHonest = false;
        
        public enum CardID : int
        {
            PotOfExtravagance = 49238328,
            UnionHangar = 66399653,
            VicViperT301 = 10642488,
            GoldGadget = 55010259,
            SilverGadget = 29021114,
            LordBritishSpaceFighter = 35514096,
            VictoryViperXX03 = 93130021,
            VictoryViperXX03Token = 93130022,
            BBusterDrake = 77411244,
            AAssaultCore = 30012506,
            CCrushWyvern = 03405259,
            BlueThunderT45 = 14089428,
            JadeKnight = 44364207,
            FalchionB = 86170989,
            DeltaTri = 12079734,
            Honest = 37742478,
            PlatinumGadget = 40216089,
            ABCDragonBuster = 01561110,
            LimiterRemoval = 2317160
        }

        public SampleExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //always activate conditionals
            AddExecutor(ExecutorType.Activate, (int)CardID.JadeKnight, JadeKnightEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.DeltaTri, DeltaTriEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.VictoryViperXX03, VictoryViperEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.LordBritishSpaceFighter, LordBritishEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.FalchionB, FalchionBEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.AAssaultCore, AAssaultCoreEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.BBusterDrake, BBusterDrakeEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.CCrushWyvern, CCrushWyvernEffect);

            //Shotguns
            AddExecutor(ExecutorType.Activate, (int)CardID.PotOfExtravagance, POEeffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.VicViperT301);
            AddExecutor(ExecutorType.Activate, (int)CardID.UnionHangar, UnionHangerEffect);

            //pre-summon effects
            AddExecutor(ExecutorType.Activate, (int)CardID.PlatinumGadget, GadgetEffect);

            //LimiterRemoval
            AddExecutor(ExecutorType.Activate, (int)CardID.LimiterRemoval, LimiterRemovalEffect);

            //Normals
            AddExecutor(ExecutorType.MonsterSet, (int)CardID.JadeKnight, NormalSummonCheck);
            AddExecutor(ExecutorType.Summon, NormalSummonCheck);

            //Honest special zone
            AddExecutor(ExecutorType.Activate, (int)CardID.Honest, HonestEffect);

            //Gadget Specials
            AddExecutor(ExecutorType.Activate, (int)CardID.SilverGadget, GadgetEffect);
            AddExecutor(ExecutorType.Activate, (int)CardID.GoldGadget, GadgetEffect);

            //set limiter removal
            AddExecutor(ExecutorType.SpellSet, (int)CardID.LimiterRemoval, LimiterRemovalSet);
            
        }

        public bool POEeffect()
        {
            //always try and draw 2
            AI.SelectOption(1);
            return true;
        }

        public bool UnionHangerEffect()
        {
            //initial activation search
            if (ActivateDescription == Util.GetStringId((int) CardID.UnionHangar, 0))
            {
                //this is in preference order
                CardID[] ABC = { CardID.BBusterDrake, CardID.CCrushWyvern, CardID.AAssaultCore };
                IList<CardID> preference = new List<CardID>();
                //first add ABC parts not on field
                foreach (CardID card in ABC)
                {
                    if (!Bot.HasInMonstersZone((int)card) && !Bot.HasInSpellZone((int)card) && !Bot.HasInHand((int) card) && !preference.Contains(card))
                    {
                        preference.Add(card);
                    }
                }
                //then add ABC other parts not in graveyard 
                foreach (CardID card in ABC)
                {
                    if (!Bot.HasInGraveyard((int)card) && !Bot.HasInHand((int)card) && !preference.Contains(card))
                    {
                        preference.Add(card);
                    }
                }
                //then add other ABC
                foreach (CardID card in ABC)
                {
                    if (!preference.Contains(card))
                    {
                        preference.Add(card);
                    }
                }
                AI.SelectCard(preference.Cast<int>().ToArray());
                return true;
            }
            //second equip from deck effect
            if (ActivateDescription == Util.GetStringId((int)CardID.UnionHangar, 1))
            {
                //this is in preference order
                IList<CardID> ABC = ABCDestroyEffectPreference();
                IList<CardID> preference = new List<CardID>();
                //first add ABC parts not in field, hand or grave
                foreach (CardID card in ABC)
                {
                    if (!Bot.HasInMonstersZone((int)card) && !Bot.HasInSpellZone((int)card) && !Bot.HasInHand((int)card) && !Bot.HasInGraveyard((int)card))
                    {
                        preference.Add(card);
                    }
                }
                //then add ABC parts only in hand
                foreach (CardID card in ABC)
                {
                    if (!Bot.HasInMonstersZone((int)card) && !Bot.HasInSpellZone((int)card) && Bot.HasInHand((int)card) && !Bot.HasInGraveyard((int)card) && !preference.Contains(card))
                    {
                        preference.Add(card);
                    }
                }
                //then add based on preference
                foreach (CardID card in ABC)
                {
                    if (!preference.Contains(card))
                    {
                        preference.Add(card);
                    }
                }
                AI.SelectCard(preference.Cast<int>().ToArray());
                return true;
            }

            return false;
        }

        public bool NormalSummonCheck()
        {
            return Card.Id == (int)chooseFromHand(getMainMonsterToFieldPreferenceOrder());
        }

        public bool GadgetEffect()
        {   
            AI.SelectCard(getMainMonsterToFieldPreferenceOrder().Cast<int>().ToArray());
            return true;
        }

        public bool JadeKnightEffect()
        {
            AI.SelectCard(getMainMonsterToHandPreferenceOrder().Cast<int>().ToArray());
            return true;
        }

        public bool DeltaTriEffect()
        {
            //option 0
            IList<CardID> preference = (List<CardID>)(new List<CardID> { CardID.DeltaTri }.Concat(triEquipPreference()));
            AI.SelectCard(preference.Cast<int>().ToArray());

            return true;
        }

        public bool VictoryViperEffect()
        {
            ClientCard activeVictoryViper = Duel.CurrentChain.Last();
            
            if(!Bot.HasInMonstersZone((int)CardID.VictoryViperXX03Token))
            {
                AI.SelectOption(2);
                return true;
            }
            
            foreach(ClientCard mon in Enemy.GetMonsters())
            {
                if((mon.Position == (int)CardPosition.FaceUpAttack && activeVictoryViper.Attack <= mon.Attack && activeVictoryViper.Attack + 400 >= mon.Attack) || (mon.Position == (int)CardPosition.FaceUpDefence && activeVictoryViper.Attack <= mon.Defense && activeVictoryViper.Attack + 400 > mon.Defense))
                {
                    AI.SelectOption(0);
                    return true;
                }
            }

            AI.SelectOption(2);
            return true;
        }

        public bool LordBritishEffect()
        {
            ClientCard activeLordBritsih = Duel.CurrentChain.Last();
            foreach (ClientCard mon in Enemy.GetMonsters())
            {
                if ((mon.Position == (int)CardPosition.FaceUpAttack && activeLordBritsih.Attack <= mon.Attack && activeLordBritsih.Attack + 400 >= mon.Attack) || (mon.Position == (int)CardPosition.FaceUpDefence && activeLordBritsih.Attack <= mon.Defense && activeLordBritsih.Attack + 400 > mon.Defense))
                {
                    AI.SelectOption(0);
                    return true;
                }
            }

            if  (Enemy.GetMonsters().Count == 0 && activeLordBritsih.Attack >= 1200)
            {
                AI.SelectOption(0);
                return true;
            }

            AI.SelectOption(2);
            AI.SelectOption(1);

            return true;
        }

        public bool FalchionBEffect()
        {
            if(Bot.GetRemainingCount((int)CardID.CCrushWyvern, 3) >= 0)
            {
                AI.SelectOption(0);
                AI.SelectCard((int)CardID.CCrushWyvern);
                return true;
            }

            AI.SelectOption(1);
            AI.SelectCard(getMainMonsterToFieldPreferenceOrder().Cast<int>().ToArray());

            return true;
        }

        public bool AAssaultCoreEffect()
        {
            //equip effect
            if (ActivateDescription == Util.GetStringId((int)CardID.AAssaultCore, 0))
            {
                return ABCPartUnionEffect();
            }

            //destroy effect
            if (ActivateDescription == Util.GetStringId((int)CardID.AAssaultCore, 1))
            {
                IList<CardID> ABC = new List<CardID> { CardID.BBusterDrake, CardID.CCrushWyvern, CardID.AAssaultCore };
                Dictionary<CardID, int> count = new Dictionary<CardID, int>();
                count[CardID.AAssaultCore] = 0;
                count[CardID.BBusterDrake] = 0;
                count[CardID.CCrushWyvern] = 0;
                foreach (ClientCard card in Bot.Graveyard.Concat(Bot.GetMonsters()).Concat(Bot.GetSpells()).ToList()) 
                {
                    if (ABC.Contains((CardID)card.Id)) { count[(CardID)card.Id]++; }
                }
                CardID validTarget = CardID.LordBritishSpaceFighter;
                foreach (CardID card in ABC)
                {
                    if (count[card] > 1)
                    {
                        validTarget = card;
                        break;
                    }
                }
                if (validTarget == CardID.LordBritishSpaceFighter)
                {
                    return false;
                }
                else
                {
                    AI.SelectCard((int)validTarget);
                    return true;
                }
            }

            //unreachable return
            return false;
        }

        public bool BBusterDrakeEffect()
        {
            //equip effect
            if (ActivateDescription == Util.GetStringId((int)CardID.BBusterDrake, 0))
            {
                return ABCPartUnionEffect();
            }
            //destroy effect
            if (ActivateDescription == Util.GetStringId((int)CardID.BBusterDrake, 1))
            {
                AI.SelectCard(getMainMonsterToHandPreferenceOrder().Cast<int>().ToArray());
                return true;
            }

            //unreachable return
            return false;
        }

        public bool CCrushWyvernEffect()
        {
            //equip effect
            if (ActivateDescription == Util.GetStringId((int)CardID.CCrushWyvern, 0))
            {
                return ABCPartUnionEffect();
            }
            //destroy effect
            if (ActivateDescription == Util.GetStringId((int)CardID.CCrushWyvern, 1))
            {
                AI.SelectCard(getMainMonsterToFieldPreferenceOrder().Cast<int>().ToArray());
                return true;
            }

            //unreachable return
            return false;
        }

        public bool HonestEffect()
        {
            //return to hand
            if (ActivateDescription == Util.GetStringId((int)CardID.Honest, 0))
            {
                return (Duel.Turn == 1 || Duel.Phase == DuelPhase.Main2) && Bot.GetMonsters().Count() > 1;
            }

            //unreachable return
            return false;
        }

        public bool LimiterRemovalSet()
        {
            return Duel.Phase == DuelPhase.Main2;
        }

        public bool LimiterRemovalEffect()
        {

        }

        private bool ABCPartUnionEffect()
        {
            if (Bot.HasInMonstersZone((int)CardID.ABCDragonBuster))
            {
                AI.SelectCard((int)CardID.ABCDragonBuster);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            //always summon these in deffence mode if it is possible
            IList<CardID> DefenceCards = new List<CardID> { CardID.JadeKnight, CardID.BBusterDrake, CardID.CCrushWyvern };
            if (DefenceCards.Contains((CardID)cardId))
            {
                CardPosition[] preferedDefPos = { CardPosition.FaceUpDefence, CardPosition.Defence, CardPosition.FaceDownDefence };
                foreach (CardPosition pref in preferedDefPos)
                {
                    if (positions.Contains(pref))
                    {
                        return pref;
                    }
                }
            }
            

            return base.OnSelectPosition(cardId, positions);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            
            
            return base.OnPreBattleBetween(attacker, defender);
        }

        private IList<CardID> ABCDestroyEffectPreference()
        {
            CardID[] ABC = { CardID.AAssaultCore, CardID.BBusterDrake, CardID.CCrushWyvern };
            
            bool ABCinHand = false;
            foreach (CardID card in ABC) { if (Bot.HasInHand((int)card)) { ABCinHand = true; break; } };
            bool ABCinGrave = false;
            foreach (CardID card in ABC) { if (Bot.HasInGraveyard((int)card)) { ABCinGrave = true; break; } };

            if (ABCinHand && !ABCinGrave) { return new List<CardID> { CardID.CCrushWyvern, CardID.BBusterDrake, CardID.AAssaultCore }; }
            if (!ABCinHand && !ABCinGrave) { return new List<CardID> { CardID.BBusterDrake, CardID.CCrushWyvern, CardID.AAssaultCore }; }
            if (ABCinHand  && ABCinGrave) { return new List<CardID> { CardID.CCrushWyvern, CardID.BBusterDrake, CardID.AAssaultCore }; }
            if (!ABCinHand && ABCinGrave)
            {
                Dictionary<CardID, int> count = new Dictionary<CardID, int>();
                count[CardID.AAssaultCore] = 0;
                count[CardID.BBusterDrake] = 0;
                count[CardID.CCrushWyvern] = 0;
                foreach (ClientCard card in Bot.Graveyard.Concat(Bot.GetMonsters()).Concat(Bot.GetSpells()))
                {
                    if (ABC.Contains((CardID)card.Id)) { count[(CardID)card.Id]++; }
                    
                }
                bool allABCinGraveUnique = true;
                foreach (ClientCard card in Bot.Graveyard)
                {
                    if (ABC.Contains((CardID)card.Id) && count[(CardID)card.Id] != 1) { allABCinGraveUnique = false; }
                }

                if (allABCinGraveUnique)
                {
                    return new List<CardID> { CardID.BBusterDrake, CardID.AAssaultCore, CardID.CCrushWyvern };
                }
                else
                {
                    return new List<CardID> { CardID.AAssaultCore, CardID.BBusterDrake, CardID.CCrushWyvern };
                }
            }

            //unreachable but needed sub par msvc reachability
            return new List<CardID> { CardID.AAssaultCore, CardID.BBusterDrake, CardID.CCrushWyvern };
        }

        private IList<CardID> triEquipPreference()
        {
            IList<CardID> preference = ABCDestroyEffectPreference();
            foreach (CardID pref in preference)
            {
                foreach (ClientCard equiped in Duel.CurrentChain.Last().EquipCards)
                {
                    if (equiped.Id == (int)pref)
                    {
                        //move to back
                        preference.Remove(pref);
                        preference.Add(pref);
                    }
                }
            }
            return preference;
        }

        private IList<CardID> getMainMonsterToHandPreferenceOrder()
        {
            IList<CardID> preference = new List<CardID>();

            //vic viper is best add if we can get secondary effect
            foreach (ClientCard card in Bot.Hand.Concat(Bot.GetMonsters()))
            {
                if (card.Attribute == (int)CardAttribute.Light && card.Race == (ulong)CardRace.Machine)
                {
                    preference.Add(CardID.VicViperT301);
                }
            }

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

            //tri + jadeknight + vicviper goes here if skipped earlyer
            if (!preference.Contains(CardID.DeltaTri)) { preference.Add(CardID.DeltaTri); }
            if (!preference.Contains(CardID.JadeKnight)) { preference.Add(CardID.JadeKnight); }
            if (!preference.Contains(CardID.VicViperT301)) { preference.Add(CardID.VicViperT301); }

            //any duplicate ABC pieces 
            if (!preference.Contains(CardID.BBusterDrake)) { preference.Add(CardID.BBusterDrake); }
            if (!preference.Contains(CardID.CCrushWyvern)) { preference.Add(CardID.CCrushWyvern); }
            if (!preference.Contains(CardID.AAssaultCore)) { preference.Add(CardID.AAssaultCore); }

            //honest always goes here
            preference.Add(CardID.Honest);

            return preference;
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
        private IList<CardID> getMainMonsterToFieldPreferenceOrder()
        {
            IList<CardID> preference = new List<CardID>();

            //there is only one of each gadget so we don't care about clashes (eg: summoning gold off of gold)
            preference.Add(CardID.GoldGadget);
            preference.Add(CardID.SilverGadget);

            //lord british and victory viper have no complications
            preference.Add(CardID.LordBritishSpaceFighter);
            preference.Add(CardID.VictoryViperXX03);

            //each ABC part should only be here if it isn't already in play
            if (!Bot.HasInMonstersZone((int)CardID.BBusterDrake) && !Bot.HasInSpellZone((int)CardID.BBusterDrake) && !Bot.HasInGraveyard((int)CardID.BBusterDrake)) { preference.Add(CardID.BBusterDrake); }
            if (!Bot.HasInMonstersZone((int)CardID.CCrushWyvern) && !Bot.HasInSpellZone((int)CardID.CCrushWyvern) && !Bot.HasInGraveyard((int)CardID.CCrushWyvern)) { preference.Add(CardID.CCrushWyvern); }
            if (!Bot.HasInMonstersZone((int)CardID.AAssaultCore) && !Bot.HasInSpellZone((int)CardID.AAssaultCore) && !Bot.HasInGraveyard((int)CardID.AAssaultCore)) { preference.Add(CardID.AAssaultCore); }

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

