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
}
}