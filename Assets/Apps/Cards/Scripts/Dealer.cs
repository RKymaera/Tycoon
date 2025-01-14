using UnityEngine;
using System.Collections.Generic;
using Apps.Common.Utils;
using Apps.Players;
using System;
using TMPro;


namespace Apps.Cards
{
    using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;
    public class Dealer : SingletonMonoBehaviour<Dealer>
    {
        public List<PlayingCard> Deck = null;
        public GameObject CardPrefab;

        public event Action<HandList, PlayerId> DealHand = new Action<HandList, PlayerId>((hand, playerId) => { });

        protected void Start()
        {
            Deck = CreateDeck();
            Debug.Log("Deck created with " + Deck.Count + " cards.");

            Shuffle();
            DealHandsToPlayers();
        }

        public List<PlayingCard> CreateDeck()
        {
            if (Deck != null && Deck.Count > 0)
                return Deck;

            var deck = new List<PlayingCard>();
            foreach (PlayingCard.Suits suit in Enum.GetValues(typeof(PlayingCard.Suits)))
            {
                if (suit == PlayingCard.Suits.Joker)
                    continue;

                foreach (PlayingCard.Ranks rank in Enum.GetValues(typeof(PlayingCard.Ranks)))
                {
                    // Jokers are added later
                    if (rank == PlayingCard.Ranks.NA || rank == PlayingCard.Ranks.Joker)
                        continue;
                    deck.Add(InstantiateCard(rank, suit));
                }
            }

            // Add 4 Jokers
            var joker = InstantiateCard(PlayingCard.Ranks.Joker, PlayingCard.Suits.Joker);
            deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());
            deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());
            foreach (var text in joker.gameObject.GetComponentsInChildren<TextMeshPro>())
            {
                text.color = PlayingCard.RedColour;
            }
            deck.Add(joker);
            deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());

            return deck;
        }

        private PlayingCard InstantiateCard(PlayingCard.Ranks rank, PlayingCard.Suits suit)
        {
            var card = Instantiate(CardPrefab).GetComponent<PlayingCard>();
            card.Rank = rank;
            card.Suit = suit;
            card.gameObject.name = card.CardName;
            var cardTexts = card.gameObject.GetComponentsInChildren<TextMeshPro>();
            foreach (var text in cardTexts)
            {
                text.text = card.CardName;
                text.color = card.CardColour;
            }
            return card;
        }

        private void Shuffle()
        {
            // Fisher-Yates shuffle
            for (int i = 0; i < Deck.Count; i++)
            {
                var temp = Deck[i];
                int randomIndex = UnityEngine.Random.Range(i, Deck.Count);
                Deck[i] = Deck[randomIndex];
                Deck[randomIndex] = temp;
            }
        }

        private void DealHandsToPlayers()
        {
            int nCards = Deck.Count / 4;
            foreach (PlayerId playerId in Enum.GetValues(typeof(PlayerId)))
            {
                if (playerId == PlayerId.NA)
                    continue;

                var hand = new HandList();
                for (int i = 0; i < nCards; i++)
                {
                    hand.Add(Deck[0]);
                    Deck.RemoveAt(0);
                }
                DealHand(hand, playerId);
            }
        }
    }
}