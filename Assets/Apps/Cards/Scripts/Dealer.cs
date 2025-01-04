using UnityEngine;
using System.Collections.Generic;
using Apps.Common;


namespace Apps.Cards
{
public class Dealer : MonoBehaviour
{
    public List<PlayingCard> Deck = null;

    protected void Awake()
    {
        Deck = CreateDeck();
        Debug.Log("Deck created with " + Deck.Count + " cards.");

        Shuffle();
        DealHandToPlayers();
    }

    public List<PlayingCard> CreateDeck()
    {
        if (Deck != null && Deck.Count > 0)
            return Deck;

        var deck = new List<PlayingCard>();
        foreach (PlayingCard.Suits suit in System.Enum.GetValues(typeof(PlayingCard.Suits)))
        {
            if (suit == PlayingCard.Suits.Joker)
                continue;

            foreach (PlayingCard.Ranks rank in System.Enum.GetValues(typeof(PlayingCard.Ranks)))
            {
                if (rank == PlayingCard.Ranks.Joker)
                    continue;
                var card = ScriptableObject.CreateInstance<PlayingCard>();
                card.Rank = rank;
                card.Suit = suit;
                deck.Add(card);
            }
        }

        // Add 2 Jokers
        deck.Add(ScriptableObject.CreateInstance<PlayingCard>());
        deck.Add(ScriptableObject.CreateInstance<PlayingCard>());

        return deck;
    }

    private void Shuffle()
    {
        // Fisher-Yates shuffle
        for (int i = 0; i < Deck.Count; i++)
        {
            var temp = Deck[i];
            int randomIndex = Random.Range(i, Deck.Count);
            Deck[i] = Deck[randomIndex];
            Deck[randomIndex] = temp;
        }
    }

    private void DealHandToPlayers()
    {

    }
}
}