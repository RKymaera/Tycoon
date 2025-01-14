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
            public bool TycoonStarts; // Start the round with Tycoon
        }
        public Rules SelectedRules = new Rules { Banish = false, EightSlash = true, TycoonStarts = true };


        public List<PlayingCard> Stack = new List<PlayingCard>();
        public int NCardsRequired = 0;
        public event Action OnStackChanged = new Action(() => { });

        private int _consecutivePasses = 0;

        public void ReceiveCards(List<PlayingCard> cards, IPlayer player)
        {
            Debug.Log("Received " + cards.Count + " from " + player.Name);
            if (cards.Count == 0)
            {
                // Reset if all players pass
                _consecutivePasses++;
                bool allPlayersPassed = _consecutivePasses == PlayerManager.Instance.Players.Count;
                if (allPlayersPassed)
                {
                    ResetStack();
                }
                PlayerManager.Instance.PlayerTurnTaken(player, allPlayersPassed);
                return;
            }

            _consecutivePasses = 0;

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
            PlayerManager.Instance.PlayerTurnTaken(player, CheckStackStatus());
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

        private bool CheckStackStatus()
        {
            if (Stack.Count == 0)
                return false;

            // Reset for 2's and EightSlash
            if (Stack.Last().Rank == PlayingCard.Ranks.Two ||
                (SelectedRules.EightSlash && Stack.Last().Rank == PlayingCard.Ranks.Eight))
            {
                ResetStack();
                return true;
            }
            return false;
        }
    }
}