using AIBase.Enums;
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
        public Dictionary<CardLoc, IList<AICard>> Locations;

        public AICard FightingCard;

        public int LP;

        public AIPlayerField(ClientField field)
        {
            Locations = new Dictionary<CardLoc, IList<AICard>>();
            Locations[CardLoc.Hand] = new List<AICard>();
            if (field.Hand != null)
            {
                foreach (ClientCard card in field.Hand)
                {
                    if (card == null) { Locations[CardLoc.Hand].Add(null); }
                    else { Locations[CardLoc.Hand].Add(AICard.FromClientCard(card, field)); }
                }
            }

            Locations[CardLoc.MonsterZone] = new List<AICard>();
            if (field.MonsterZone != null)
            {
                foreach (ClientCard card in field.MonsterZone)
                {
                    if (card == null) { Locations[CardLoc.MonsterZone].Add(null); continue; }
                    AICard monster = AICard.FromClientCard(card, field);
                    Locations[CardLoc.MonsterZone].Add(monster);

                    if (field.BattlingMonster == card)
                    {
                        FightingCard = monster;
                    }
                }
            }

            Locations[CardLoc.SpellZone] = new List<AICard>();
            if (field.SpellZone != null)
            {
                foreach (ClientCard card in field.SpellZone)
                {
                    if (card == null) { Locations[CardLoc.SpellZone].Add(null); }
                    else { Locations[CardLoc.SpellZone].Add(AICard.FromClientCard(card, field)); }
                }
            }

            Locations[CardLoc.Graveyard] = new List<AICard>();
            if (field.Graveyard != null)
            {
                foreach (ClientCard card in field.Graveyard)
                {
                    if (card == null) { Locations[CardLoc.Graveyard].Add(null); }
                    else { Locations[CardLoc.Graveyard].Add(AICard.FromClientCard(card, field)); }
                }
            }

            Locations[CardLoc.Banished] = new List<AICard>();
            if (field.Banished != null)
            {
                foreach (ClientCard card in field.Banished)
                {
                    if (card == null) { Locations[CardLoc.Banished].Add(null); }
                    else { Locations[CardLoc.Banished].Add(AICard.FromClientCard(card, field)); }
                }
            }

            Locations[CardLoc.Deck] = new List<AICard>();
            if (field.Deck != null)
            {
                foreach (ClientCard card in field.Deck)
                {
                    if (card == null) { Locations[CardLoc.Deck].Add(null); }
                    else { Locations[CardLoc.Deck].Add(AICard.FromClientCard(card, field)); }
                }
            }

            Locations[CardLoc.ExtraDeck] = new List<AICard>();
            if (field.ExtraDeck != null)
            {
                foreach (ClientCard card in field.ExtraDeck)
                {
                    if (card == null) 
                    {
                        Locations[CardLoc.ExtraDeck].Add(null);
                    }
                    else
                    {
                        Locations[CardLoc.ExtraDeck].Add(AICard.FromClientCard(card, field));
                    }
                }
            }

            LP = field.LifePoints;
        }

        public AIPlayerField(AIPlayerField copy)
        {
            Locations = new Dictionary<CardLoc, IList<AICard>>();
            Locations[CardLoc.Hand] = new List<AICard>();
            foreach(AICard card in copy.Locations[CardLoc.Hand])
            {
                if (card == null) { Locations[CardLoc.Hand].Add(null); }
                else { Locations[CardLoc.Hand].Add(new AICard(card)); }
            }

            Locations[CardLoc.MonsterZone] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.MonsterZone])
            {
                if (card == null) { Locations[CardLoc.MonsterZone].Add(null); }
                else { Locations[CardLoc.MonsterZone].Add(new AICard(card)); }
            }

            Locations[CardLoc.SpellZone] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.SpellZone])
            {
                if (card == null) { Locations[CardLoc.SpellZone].Add(null); }
                else { Locations[CardLoc.SpellZone].Add(new AICard(card)); }
            }

            Locations[CardLoc.Graveyard] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.Graveyard])
            {
                if (card == null) { Locations[CardLoc.Graveyard].Add(null); }
                else { Locations[CardLoc.Graveyard].Add(new AICard(card)); }
            }

            Locations[CardLoc.Banished] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.Banished])
            {
                if (card == null) { Locations[CardLoc.Banished].Add(null); }
                else { Locations[CardLoc.Banished].Add(new AICard(card)); }
            }

            Locations[CardLoc.Deck] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.Deck])
            {
                if (card == null) { Locations[CardLoc.Deck].Add(null); }
                else { Locations[CardLoc.Deck].Add(new AICard(card)); }
            }

            Locations[CardLoc.ExtraDeck] = new List<AICard>();
            foreach (AICard card in copy.Locations[CardLoc.ExtraDeck])
            {
                if (card == null) { Locations[CardLoc.ExtraDeck].Add(null); }
                else { Locations[CardLoc.ExtraDeck].Add(new AICard(card)); }
            }

            FightingCard = copy.FightingCard;

            LP = copy.LP;
        }

        public int FreeMonsterZones()
        {
            int count = 0;
            foreach(AICard card in Locations[CardLoc.MonsterZone])
            {
                if(card == null) { count++; }
            }

            return count;
        }

        public void MoveCard(AICard target, CardLoc from, CardLoc to, int pos=-1)
        {
            foreach(AICard c in Locations[from])
            {
                if( c != null && c.source == target.source )
                {
                    Locations[from].Remove(c);
                    break;
                }
            }

            if (pos != -1)
            {
                Locations[to][pos] = target;
            }
            else
            {
                Locations[to].Add(target);
            }
        }
    }
}
