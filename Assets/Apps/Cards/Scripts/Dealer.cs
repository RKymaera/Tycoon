using UnityEngine;
using System.Collections.Generic;
using Apps.Common;
using Apps.Players;
using System;
using TMPro;

using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;
using Sirenix.Utilities;

namespace Apps.Cards
{
public class Dealer : MonoBehaviour
{
    public List<PlayingCard> Deck = null;
    public GameObject CardPrefab;

    public event Action<HandList, PlayerId> DealHand = new Action<HandList, PlayerId>((hand, playerId) => { });

    protected void Awake()
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
                if (rank == PlayingCard.Ranks.Joker)
                    continue;
                var card = Instantiate(CardPrefab).GetComponent<PlayingCard>();
                card.Rank = rank;
                card.Suit = suit;
                card.gameObject.name = card.CardName;
                card.gameObject.GetComponentsInChildren<TextMeshPro>().ForEach(text => text.text = card.CardName);
                deck.Add(card);
            }
        }

        // Add 4 Jokers
        var joker = Instantiate(CardPrefab).GetComponent<PlayingCard>();
        joker.gameObject.name = joker.CardName;
        joker.gameObject.GetComponentsInChildren<TextMeshPro>().ForEach(text => text.text = joker.CardName);
        deck.Add(joker);
        deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());
        deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());
        deck.Add(Instantiate(joker.gameObject).GetComponent<PlayingCard>());

        return deck;
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
                Deck[0].Owner = playerId;
                hand.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
            DealHand(hand, playerId);
        }
    }
}
}