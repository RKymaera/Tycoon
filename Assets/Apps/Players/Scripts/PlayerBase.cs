using UnityEngine;
using System;
using System.Collections.Generic;
using Apps.Cards;
using TMPro;


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
    [SerializeField] private PlayerRank _rank;
    public PlayerRank Rank => _rank;
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
    [SerializeField] private PlayerManager _playerManager;

    public bool IsOpenHand = true;

    public event Action<List<PlayingCard>, PlayerId> OnCardsPlayed = new Action<List<PlayingCard>, PlayerId>((card, playerId) => { });
    public event Action OnReceivedHand = new Action(() => { });

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
        _dealer.DealHand += ReceivedHand;
        if (_playerManager == null)
        {
            _playerManager = FindAnyObjectByType<PlayerManager>();
        }
        if (_playerManager == null)
        {
            Debug.LogError("PlayerManager not found.");
        }
    }

    public void PlaySelectedCards()
    {
        var selectedCards = new List<PlayingCard>();
        var hand = new HandList();
        foreach (var card in Hand)
        {
            if (card.IsSelected)
            {
                selectedCards.Add(card);
                card.IsSelected = false;
                card.IsPlayed = true;
            }
            else
            {
                hand.Add(card);
            }
        }

        if (selectedCards.Count == 0)
            return;

        OnCardsPlayed(selectedCards, Id);
        _hand = hand;
        OrganizeHand();
    }

    protected void ReceivedHand(HandList hand, PlayerId playerId)
    {
        if (playerId != Id)
            return;
        _hand = hand;
        Debug.Log(Name + " received " + Hand.Count + " cards.");
        OrganizeHand();
        OnReceivedHand();
    }

    protected void OrganizeHand()
    {
        if (Hand == null)
            return;
        int i = 0;
        foreach (var card in Hand)
        {
            card.transform.SetParent(transform);
            card.transform.localPosition = new Vector3(i * _playerManager.CardSeparation.x, i * _playerManager.CardSeparation.y, i * _playerManager.CardSeparation.z);
            i++;
        }
    }
}
}
