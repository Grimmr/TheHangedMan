using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game;

namespace AIBase.Game
{
    public class AIPlayerField
    {
        public IList<AICard> Hand;
        public IList<AICard> MonsterZones;
        public IList<AICard> SpellZones;
        public IList<AICard> Graveyard;
        public IList<AICard> Banished;
        public IList<AICard> Deck;
        public IList<AICard> ExtraDeck;

        public AICard FightingCard;

        public int LP;

        public AIPlayerField(ClientField field)
        {
            Hand = new List<AICard>();
            if (field.Hand != null)
            {
                foreach (ClientCard card in field.Hand)
                {
                    if (card == null) { Hand.Add(null); }
                    else { Hand.Add(AICard.FromClientCard(card, field)); }
                }
            }

            MonsterZones = new List<AICard>();
            if (field.MonsterZone != null)
            {
                foreach (ClientCard card in field.MonsterZone)
                {
                    if (card == null) { MonsterZones.Add(null); continue; }
                    AICard monster = AICard.FromClientCard(card, field);
                    MonsterZones.Add(monster);

                    if (field.BattlingMonster == card)
                    {
                        FightingCard = monster;
                    }
                }
            }

            SpellZones = new List<AICard>();
            if (field.SpellZone != null)
            {
                foreach (ClientCard card in field.SpellZone)
                {
                    if (card == null) { SpellZones.Add(null); }
                    else { SpellZones.Add(AICard.FromClientCard(card, field)); }
                }
            }

            Graveyard = new List<AICard>();
            if (field.Graveyard != null)
            {
                foreach (ClientCard card in field.Graveyard)
                {
                    if (card == null) { Graveyard.Add(null); }
                    else { Graveyard.Add(AICard.FromClientCard(card, field)); }
                }
            }

            Banished = new List<AICard>();
            if (field.Banished != null)
            {
                foreach (ClientCard card in field.Banished)
                {
                    if (card == null) { Banished.Add(null); }
                    else { Banished.Add(AICard.FromClientCard(card, field)); }
                }
            }

            Deck = new List<AICard>();
            if (field.Deck != null)
            {
                foreach (ClientCard card in field.Deck)
                {
                    if (card == null) { Deck.Add(null); }
                    else { Deck.Add(AICard.FromClientCard(card, field)); }
                }
            }

            ExtraDeck = new List<AICard>();
            if (field.ExtraDeck != null)
            {
                foreach (ClientCard card in field.ExtraDeck)
                {
                    if (card == null) { ExtraDeck.Add(null); }
                    else { ExtraDeck.Add(AICard.FromClientCard(card, field)); }
                }
            }

            LP = field.LifePoints;
        }

        public AIPlayerField(AIPlayerField copy)
        {
            Hand = new List<AICard>();
            foreach(AICard card in copy.Hand)
            {
                if (card == null) { Hand.Add(null); }
                else { Hand.Add(new AICard(card)); }
            }

            MonsterZones = new List<AICard>();
            foreach (AICard card in copy.MonsterZones)
            {
                if (card == null) { MonsterZones.Add(null); }
                else { MonsterZones.Add(new AICard(card)); }
            }

            SpellZones = new List<AICard>();
            foreach (AICard card in copy.SpellZones)
            {
                if (card == null) { SpellZones.Add(null); }
                else { SpellZones.Add(new AICard(card)); }
            }

            Graveyard = new List<AICard>();
            foreach (AICard card in copy.Graveyard)
            {
                if (card == null) { Graveyard.Add(null); }
                else { Graveyard.Add(new AICard(card)); }
            }

            Banished = new List<AICard>();
            foreach (AICard card in copy.Banished)
            {
                if (card == null) { Banished.Add(null); }
                else { Banished.Add(new AICard(card)); }
            }

            Deck = new List<AICard>();
            foreach (AICard card in copy.Deck)
            {
                if (card == null) { Deck.Add(null); }
                else { Deck.Add(new AICard(card)); }
            }

            ExtraDeck = new List<AICard>();
            foreach (AICard card in copy.ExtraDeck)
            {
                if (card == null) { ExtraDeck.Add(null); }
                else { ExtraDeck.Add(new AICard(card)); }
            }

            FightingCard = copy.FightingCard;

            LP = copy.LP;
        }

        public int FreeMonsterZones()
        {
            int count = 0;
            foreach(AICard card in MonsterZones)
            {
                if(card == null) { count++; }
            }

            return count;
        }
    }
}
