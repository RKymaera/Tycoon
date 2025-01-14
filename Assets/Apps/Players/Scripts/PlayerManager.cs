using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Apps.Common.Utils;
using Apps.Cards;


namespace Apps.Players
{
    public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
    {
        [SerializeField] public List<PlayerBase> Players;
        public Vector3 CardSeparation = new Vector3(0.01f, 0, .001f);

        private int _nRounds = 0;
        private List<IPlayer> _playerOrder = new List<IPlayer>();
        private List<IPlayer> _playersFinished = new List<IPlayer>();

        protected void Start()
        {
            _playerOrder = Players.Cast<IPlayer>().ToList();
            //Start with a random player
            Players[Random.Range(0, Players.Count)].StartTurn();
        }

        public void PlayerTurnTaken(IPlayer player, bool stackReset)
        {
            IPlayer nextPlayer = stackReset ? player :
                player.Id == _playerOrder.Last().Id ? _playerOrder.First() : _playerOrder[_playerOrder.IndexOf(player) + 1];
            nextPlayer.StartTurn();
        }

        public void PlayerFinishedRound(IPlayer player)
        {
            _playersFinished.Add(player);
            if (_playersFinished.Count == Players.Count)
            {
                CompleteRound();
            }
        }

        private void CompleteRound()
        {
            Debug.Log("All players have finished the round.");
            if (StackManager.Instance.SelectedRules.Banish && _nRounds > 0 && _playersFinished[0].Rank != PlayerRank.Tycoon)
            {
                IPlayer tycoon = _playersFinished.FirstOrDefault(p => p.Rank == PlayerRank.Tycoon);
                _playersFinished = _playersFinished.Where(p => p.Rank != PlayerRank.Tycoon).ToList();
                _playersFinished.Add(tycoon);
            }

            // Reset all players
            for (int i = 0; i < _playersFinished.Count; i++)
            {
                _playersFinished[i].MoveToNextRound((PlayerRank)i);
            }

            // Start the next round
            Dealer.Instance.DealCards();
            _playerOrder = _playersFinished;
            // Start with the Tycoon or the Poor
            _playerOrder[StackManager.Instance.SelectedRules.TycoonStarts ? 0 : Players.Count - 1].StartTurn();

            _nRounds++;
            _playersFinished.Clear();
        }

    }
}