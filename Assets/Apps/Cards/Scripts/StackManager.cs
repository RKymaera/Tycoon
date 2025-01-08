using UnityEngine;
using System.Collections.Generic;
using Apps.Common;
using Apps.Players;
using System;
using TMPro;

using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;
using Sirenix.Utilities;
using System.Linq;
using UnityEngine.UI;

namespace Apps.Cards
{
public class StackManager : MonoBehaviour
{
    public List<PlayingCard> Stack = new List<PlayingCard>();
    public int NCardsRequired = 0;
    public event Action OnStackChanged = new Action(() => { });
    [SerializeField] private PlayerManager _playerManager;
    
    protected void Awake()
    {
        if (_playerManager == null)
        {
            _playerManager = FindAnyObjectByType<PlayerManager>();
        }
        if (_playerManager == null)
        {
            Debug.LogError("PlayerManager not found.");
        }
        foreach (var player in _playerManager.Players)
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
            card.transform.localPosition = new Vector3(i * _playerManager.CardSeparation.x, i * _playerManager.CardSeparation.y, i * _playerManager.CardSeparation.z);
            i++;
        }
    }
}
}