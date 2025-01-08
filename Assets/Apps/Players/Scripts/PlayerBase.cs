using UnityEngine;
using System;
using System.Collections.Generic;
using Apps.Cards;
using TMPro;
using UnityEditor.Rendering;
using System.Linq;


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

    // Game-specific properties
    private HandList _hand = new HandList();
    public HandList Hand => _hand;
    private List<PlayingCard> _lastPlayedCards;
    public List<PlayingCard> LastPlayedCards { get { return _lastPlayedCards; } }
    private PlayingCard.Ranks _selectedRank = PlayingCard.Ranks.NA;
    public PlayingCard.Ranks SelectedRank { get { return _selectedRank; } }
    private List<PlayingCard> _selectedCards = new List<PlayingCard>();

    // References to UI elements
    [SerializeField] private GameObject _handContainer;
    [SerializeField] private TextMeshPro _nameText;


    // References to singleton objects
    [SerializeField] private Dealer _dealer;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private StackManager _stackManager;

    public bool IsOpenHand = true;

    public event Action<List<PlayingCard>, PlayerId> OnCardsPlayed = new Action<List<PlayingCard>, PlayerId>((card, playerId) => { });
    public event Action OnReceivedHand = new Action(() => { });

    protected void OnValidate()
    {
        _nameText.text = Name;
        if (_dealer == null)
        {
            _dealer = FindAnyObjectByType<Dealer>();
        }        
        if (_playerManager == null)
        {
            _playerManager = FindAnyObjectByType<PlayerManager>();
        }
        if (_stackManager == null)
        {
            _stackManager = FindAnyObjectByType<StackManager>();
        }
    }

    protected void Awake()
    {
        _dealer.DealHand += ReceivedHand;
        _stackManager.OnStackChanged += UpdateCardSelectability;
    }

    public void PlaySelectedCards()
    {
        foreach (var card in _selectedCards)
        {
            _hand.Remove(card);
        }

        OnCardsPlayed(_selectedCards, Id);
        _lastPlayedCards = _selectedCards;
        _selectedCards = new List<PlayingCard>();
        _selectedRank = PlayingCard.Ranks.NA;
        OrganizeHand();
    }

    protected void ReceivedHand(HandList hand, PlayerId playerId)
    {
        if (playerId != Id)
            return;
        _hand = hand;
        Debug.Log(Name + " received " + Hand.Count + " cards.");
        foreach (var card in Hand)
        {
            card.Owner = Id;
            card.transform.SetParent(_handContainer.transform);
            card.OnCardSelected += CardFromHandSelected;
        }
        OrganizeHand();
        OnReceivedHand();
    }

    protected void CardFromHandSelected(PlayingCard card)
    {
        if (card.Owner != Id)
            return;
        if (card.IsSelected)
        {
            _selectedRank = card.Rank;
            _selectedCards.Add(card);
        }
        else
        {
            _selectedCards.Remove(card);
            if (_selectedCards.Count == 0)
            {
                _selectedRank = PlayingCard.Ranks.NA;
            }
        }

        UpdateCardSelectability();
    }

    private void UpdateCardSelectability()
    {
        foreach (var card in Hand)
        {
            // If a rank is selected, only cards of that rank can be selected
            // If no rank is selected, any card greater than the last card in the stack can be selected
            card.IsSelectable = card.Rank == _selectedRank || 
                (_selectedRank == PlayingCard.Ranks.NA && 
                    (_stackManager.Stack.Count == 0 || card.Rank > _stackManager.Stack.Last().Rank));
        }
    }

    protected void OrganizeHand()
    {
        if (Hand == null)
            return;
        int i = 0;
        foreach (var card in Hand)
        {
            card.transform.localPosition = new Vector3(i * _playerManager.CardSeparation.x, i * _playerManager.CardSeparation.y, i * _playerManager.CardSeparation.z);
            i++;
        }
    }
}
}
