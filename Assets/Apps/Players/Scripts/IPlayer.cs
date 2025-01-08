using UnityEngine;
using System;
using System.Collections.Generic;
using Apps.Cards;

using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;

namespace Apps.Players
{
public enum PlayerId
{
    NA = -1,
    One,
    Two,
    Three,
    Four,
}
public enum PlayerRank
{
    Poor = -1,
    Commoner,
    Rich,
    Tycoon,
}

public interface IPlayer
{

    PlayerId Id { get; }
    string Name { get; }
    PlayerRank Rank { get; }
    string Backstory { get; }
    string CurrentStory { get; }
    HandList Hand { get; }
    PlayingCard.Ranks SelectedRank { get; }

    void PlaySelectedCards();
    event Action<List<PlayingCard>, PlayerId> OnCardsPlayed;
    event Action OnReceivedHand;
}
}
