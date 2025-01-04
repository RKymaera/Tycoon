using UnityEngine;


namespace Apps.Cards
{
public class PlayingCard : ScriptableObject
{
    public enum Ranks
    {
        Joker = 0,
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
    }

    public enum Suits
    {
        Joker = 0,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public Ranks Rank = Ranks.Joker;
    public Suits Suit = Suits.Joker;
    public bool IsPlayed = false;

    public static string ToString(PlayingCard card)
    {
        return card.Rank.ToString() + " of " + card.Suit.ToString();
    }
}
}