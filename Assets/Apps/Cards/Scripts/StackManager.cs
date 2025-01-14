using UnityEngine;
using System.Collections.Generic;
using Apps.Common.Utils;
using Apps.Players;
using System;
using System.Linq;

namespace Apps.Cards
{
    public class StackManager : SingletonMonoBehaviour<StackManager>
    {
        public struct Rules
        {
            public bool Banish; // Miyako-Ochi
            public bool EightSlash; // Hachi-Giri
        }
        public Rules SelectedRules = new Rules { Banish = false, EightSlash = true };


        public List<PlayingCard> Stack = new List<PlayingCard>();
        public int NCardsRequired = 0;
        public event Action OnStackChanged = new Action(() => { });
        public void ReceiveCards(List<PlayingCard> cards, IPlayer player)
        {
            Debug.Log("Received " + cards.Count + " from " + player.Name);
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
            CheckStackStatus();
        }

        public void ResetStack()
        {
            Debug.Log("Slashed the stack");
            foreach (var card in Stack)
            {
                Destroy(card.gameObject);
            }
            Stack.Clear();
            NCardsRequired = 0;
            OnStackChanged();
        }

        private void OrganizeStack()
        {
            int i = 0;
            foreach (var card in Stack)
            {
                card.transform.SetParent(transform);
                card.transform.localPosition = i * PlayerManager.Instance.CardSeparation;
                i++;
            }
        }

        private void CheckStackStatus()
        {
            if (Stack.Count == 0)
                return;

            // Reset for EightSlash
            if (SelectedRules.EightSlash && Stack.Last().Rank == PlayingCard.Ranks.Eight)
            {
                ResetStack();
                return;
            }
            // Reset for PlayerStall
            bool playersStalled = true;
            foreach (var player in PlayerManager.Instance.Players)
            {
                playersStalled &= player.OutOfPlayableCards;
            }
            if (playersStalled)
            {
                ResetStack();
                return;
            }
        }
    }
}