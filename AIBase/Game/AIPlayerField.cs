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

        public int LP;

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
