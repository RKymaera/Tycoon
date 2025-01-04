using UnityEngine;


namespace Apps.Cards
{
public class PlayingCard : ScriptableObject
{
#region Enums
    // Sorted by value
    public enum Ranks
    {
        Three = 3,
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
        Ace,
        Two,
        Joker,
    }

    public static string ToString(Ranks rank)
    {
        switch(rank)
        {
            case Ranks.Three:
            case Ranks.Four:
            case Ranks.Five:
            case Ranks.Six:
            case Ranks.Seven:
            case Ranks.Eight:
            case Ranks.Nine:
            case Ranks.Ten:
                return ((int)rank).ToString();
            case Ranks.Jack:
                return "J";
            case Ranks.Queen:
                return "Q";
            case Ranks.King:
                return "K";
            case Ranks.Ace:
                return "A";
            case Ranks.Two:
                return "2";
            case Ranks.Joker:
                return "Joker";
        }
        return null;
    }

    public enum Suits
    {
        Joker = 0,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public static string ToString(Suits suit)
    {
        switch(suit)
        {
            case Suits.Clubs:
                return "♣";
            case Suits.Diamonds:
                return "♦";
            case Suits.Hearts:
                return "♥";
            case Suits.Spades:
                return "♠";
            case Suits.Joker:
                return "Joker";
        }
        return null;
    }
#endregion


    public Ranks Rank = Ranks.Joker;
    public Suits Suit = Suits.Joker;
    public bool IsPlayed = false;


#region Operators
    // Equivalence operators
    // For the purposes of this game, we will only be comparing the rank of the cards for equivalence operators
    public static bool operator ==(PlayingCard a, PlayingCard b) => a.Rank == b.Rank;
    public static bool operator !=(PlayingCard a, PlayingCard b) => a.Rank != b.Rank;

    // Equals operator is overridden to reflect the Rank and Suit of the card
    public override bool Equals(object obj) => obj is PlayingCard card && card.Rank == Rank && card.Suit == Suit;


    // Sort operators
    // The ranks are sorted by card value for this game, when other games are implemented, this may change
    public static bool operator <(PlayingCard a, PlayingCard b) => a.Rank < b.Rank;
    public static bool operator >(PlayingCard a, PlayingCard b) => a.Rank > b.Rank;

    // Hash code is overridden to reflect the Rank and Suit of the card
    public override int GetHashCode() => (int)Rank * 100 + (int)Suit;


    public static string ToString(PlayingCard card)
    {
        if (card.Rank == Ranks.Joker)
            return "Joker";
        return ToString(card.Rank) + ToString(card.Suit);
    }
#endregion
}
}