using UnityEngine;
using System;
using System.Collections.Generic;
using Apps.Cards;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine.UI;
using System.Collections;


// Shorthand for the namespace
namespace Apps.Players
{
using HandList = System.Collections.Generic.SortedSet<Apps.Cards.PlayingCard>;
public class PlayerBase : MonoBehaviour, IPlayer
{
    [SerializeField] private PlayerId _id;
    public PlayerId Id => _id;
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private string _backstory;
    public string Backstory => _backstory;
    [SerializeField] private string _currentStory;
    public string CurrentStory => _currentStory;
    private HandList _hand;
    public HandList Hand => _hand;
    private List<PlayingCard> _lastPlayedCards;
    public List<PlayingCard> LastPlayedCards { get { return _lastPlayedCards; } }
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private Dealer _dealer;

    public bool IsOpenHand = true;

    public event Action<Cards.PlayingCard, PlayerId> PlayCard;

    protected void Awake()
    {
        _nameText.text = Name;
        if (_dealer == null)
        {
            _dealer = FindAnyObjectByType<Dealer>();
        }
        if (_dealer == null)
        {
            Debug.LogError("Dealer not found.");
        }
        _dealer.DealHand += OnReceivedHand;
    }

    public void OnReceivedHand(HandList hand, PlayerId playerId)
    {
        if (playerId != Id)
            return;
        _hand = hand;
        Debug.Log(Name + " received " + Hand.Count + " cards.");
        OrganizeHand();
    }

    public void PlayNextCard()
    {
        if (Hand.Count == 0)
            return;
    }

    protected void OrganizeHand()
    {
        if (Hand == null)
            return;
        int i = 0;
        foreach (var card in Hand)
        {
            card.transform.SetParent(transform);
            card.transform.localPosition = new Vector3((i-Hand.Count/2) * 0.1f, 0, 0);
            i++;
        }
    }
}
}
