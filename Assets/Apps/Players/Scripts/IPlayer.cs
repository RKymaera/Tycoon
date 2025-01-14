using UnityEngine;
using System;
using System.Collections.Generic;
using Apps.Cards;


namespace Apps.Players
{
    using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;
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
