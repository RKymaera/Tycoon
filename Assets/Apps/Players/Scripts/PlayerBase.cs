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
        // Allow passes but ignore plays if not enough cards are selected
        if (_selectedCards.Count != 0 && _selectedCards.Count != _stackManager.NCardsRequired)
            return;

        foreach (var card in _selectedCards)
        {
            _hand.Remove(card);
        }

        OnCardsPlayed(_selectedCards, Id);
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
            card.OnCardSelectedChanged += CardFromHandSelected;
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
            // If the selected rank is different, deselect all other cards
            if (_selectedRank != card.Rank)
            {
                _selectedCards.ForEach(c => c.IsSelected = false);
                _selectedCards = new List<PlayingCard>();
            }
            // If the selection is full, remove the first card so it can be replaced
            if (_stackManager.NCardsRequired > 0 &&
                _selectedCards.Count == _stackManager.NCardsRequired)
            {
                _selectedCards.First().IsSelected = false;
                _selectedCards.RemoveAt(0);
            }
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
        // Count the number of cards of each rank if more than one copy is required
        var rankCounts = new Dictionary<PlayingCard.Ranks, int>();
        if (_stackManager.NCardsRequired > 1)
        {
            foreach (var card in Hand)
            {
                if (!rankCounts.ContainsKey(card.Rank))
                {
                    rankCounts[card.Rank] = 0;
                }
                rankCounts[card.Rank]++;
            }
        }

        foreach (var card in Hand)
        {
            // Cards greater than the last card in the stack and that have enough copies
            // can be selected
            card.IsSelectable = _stackManager.Stack.Count == 0 ||
                card.Rank > _stackManager.Stack.Last().Rank;
            if (_stackManager.NCardsRequired > 1)
            {
                card.IsSelectable &= rankCounts[card.Rank] >= _stackManager.NCardsRequired;
            }
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
