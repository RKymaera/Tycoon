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

public interface IPlayer
{

    PlayerId Id { get; }
    string Name { get; }
    string Backstory { get; }
    string CurrentStory { get; }
    HandList Hand { get; }
    List<PlayingCard> LastPlayedCards { get; }

    void OnReceivedHand(HandList hand, PlayerId playerId);
    void PlayNextCard();
    event Action<Cards.PlayingCard, PlayerId> PlayCard;
}
}
