using UnityEngine;
using System.Collections.Generic;
using Apps.Common.Utils;
using Apps.Players;
using System;

namespace Apps.Cards
{
    public class StackManager : SingletonMonoBehaviour<StackManager>
    {
        public List<PlayingCard> Stack = new List<PlayingCard>();
        public int NCardsRequired = 0;
        public event Action OnStackChanged = new Action(() => { });

        protected override void Awake()
        {
            base.Awake();
            foreach (var player in PlayerManager.Instance.Players)
            {
                player.OnCardsPlayed += OnReceivedCards;
            }
        }

        public void OnReceivedCards(List<PlayingCard> cards, PlayerId playerId)
        {
            if (cards.Count == 0)
                return;

            if (NCardsRequired != 0 && cards.Count != NCardsRequired)
            {
                Debug.LogError("Received " + cards.Count + " cards, but expected " + NCardsRequired + " cards.");
                return;
            }

            NCardsRequired = cards.Count;
            foreach (var card in cards)
            {
                Stack.Add(card);
                card.IsPlayed = true;
                card.transform.SetParent(transform);
            }
            OrganizeStack();
            OnStackChanged();
        }

        public void ResetStack()
        {
            foreach (var card in Stack)
            {
                Destroy(card.gameObject);
            }
            Stack.Clear();
            NCardsRequired = 0;
            OnStackChanged();
        }

        protected void OrganizeStack()
        {
            int i = 0;
            foreach (var card in Stack)
            {
                card.transform.SetParent(transform);
                card.transform.localPosition = i * PlayerManager.Instance.CardSeparation;
                i++;
            }
        }
    }
}