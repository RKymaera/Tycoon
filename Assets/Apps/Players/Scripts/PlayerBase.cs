using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;

using Apps.Cards;


// Shorthand for the namespace
namespace Apps.Players
{
    using HandList = SortedSet<PlayingCard>;
    public class PlayerBase : MonoBehaviour, IPlayer
    {
        [SerializeField] private PlayerId _id;
        public PlayerId Id => _id;
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private PlayerRank _rank = PlayerRank.Commoner;
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
        private bool _outOfPlayableCards = false;
        public bool OutOfPlayableCards => _outOfPlayableCards;
        private bool _hasTurnPriority = false;

        // References to UI elements
        [SerializeField] private GameObject _handContainer;
        [SerializeField] private TextMeshPro _nameText;

        public bool IsOpenHand = true;

        public event Action<List<PlayingCard>, IPlayer> OnCardsPlayed = new Action<List<PlayingCard>, IPlayer>((card, player) => { });
        public event Action OnReceivedHand = new Action(() => { });
        public event Action<IPlayer> OnFinishedRound = new Action<IPlayer>((player) => { });


        protected void Awake()
        {
            _nameText.text = Name;

            // Connect to signals
            Dealer.Instance.DealHand += ReceivedHand;
            StackManager.Instance.OnStackChanged += UpdateCardSelectability;
            OnCardsPlayed += StackManager.Instance.ReceiveCards;
            OnFinishedRound += PlayerManager.Instance.PlayerFinishedRound;
        }

        public void StartTurn()
        {
            _hasTurnPriority = true;
        }

        public void PlaySelectedCards()
        {
            if (!_hasTurnPriority)
                return;

            _hasTurnPriority = false;

            // Allow passes but ignore plays if not enough cards are selected
            if (_selectedCards.Count > 0 && StackManager.Instance.NCardsRequired > 0 &&
                _selectedCards.Count != StackManager.Instance.NCardsRequired)
                return;

            foreach (var card in _selectedCards)
            {
                _hand.Remove(card);
            }

            // Send the cards
            OnCardsPlayed(_selectedCards, this);

            if (_hand.Count == 0)
            {
                OnFinishedRound(this);
                return;
            }

            _selectedCards = new List<PlayingCard>();
            _selectedRank = PlayingCard.Ranks.NA;
            OrganizeHand();
        }

        public void MoveToNextRound(PlayerRank awardedRank)
        {
            _rank = awardedRank;

            _outOfPlayableCards = false;
            Hand.Clear();
            _selectedCards.Clear();
            _selectedRank = PlayingCard.Ranks.NA;
            _hasTurnPriority = false;
        }

        private void ReceivedHand(HandList hand, PlayerId playerId)
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

        private void CardFromHandSelected(PlayingCard card)
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
                if (StackManager.Instance.NCardsRequired > 0 &&
                    _selectedCards.Count == StackManager.Instance.NCardsRequired)
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
            if (StackManager.Instance.NCardsRequired > 1)
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

            int nSelectableCards = 0;
            foreach (var card in Hand)
            {
                bool isHighEnough = StackManager.Instance.Stack.Count == 0 ||
                    card.Rank > StackManager.Instance.Stack.Last().Rank;
                // If only one card is required, all those cards are selectable, otherwise check the count
                card.IsSelectable = isHighEnough && (StackManager.Instance.NCardsRequired <= 1
                    || rankCounts[card.Rank] >= StackManager.Instance.NCardsRequired);
                nSelectableCards += card.IsSelectable ? 1 : 0;
            }
            _outOfPlayableCards = nSelectableCards == 0;
        }

        private void OrganizeHand()
        {
            if (Hand == null)
                return;
            int i = 0;
            foreach (var card in Hand)
            {
                card.transform.localPosition = i * PlayerManager.Instance.CardSeparation;
                i++;
            }
        }
    }
}
