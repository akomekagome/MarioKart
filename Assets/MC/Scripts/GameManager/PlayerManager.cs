using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Players;
using Zenject;
using UniRx;
using System;
using System.Linq;

namespace MC.GameManager
{
    public class PlayerManager : MonoBehaviour, IPlayerDamageables
    {
        private ReactiveCollection<PlayerCore> players = new ReactiveCollection<PlayerCore>();
        private List<PlayerId> goalPlayers = new List<PlayerId>();

        private IConnectableObservable<PlayerCore> onPlayerSpawned;

        /// <summary>
        /// プレイヤが登録されたことを通知する
        /// </summary>
        public IObservable<PlayerCore> OnPlayerSpawnedAsObservable
        {
            get
            {
                return onPlayerSpawned;
            }
        }

        private Subject<List<PlayerId>> goalPlayesSubject = new Subject<List<PlayerId>>();
        public IObservable<List<PlayerId>> GoalPlayersObservable { get { return goalPlayesSubject.AsObservable(); } }

        private Subject<PlayerCore> _playerWinner = new Subject<PlayerCore>();
        public IObservable<PlayerCore> OnWinnerPlayerAsObservable()
        {
            return _playerWinner.AsObservable();
        }

        private Subject<PlayerCore> onPlayerDeadSubject = new Subject<PlayerCore>();

        public IObservable<PlayerCore> OnPlayerDeadObservable
        {
            get { return onPlayerDeadSubject.AsObservable(); }
        }

        public void SetPlayer(PlayerCore player)
        {
            var playerId = player.PlayerId;
            // 多重登録禁止
            if (players.ToList().Exists(x => x.PlayerId == playerId))
                return;
            // 作られたプレイヤーを管理リストに追加
            players.Add(player);

            player.goalObservable
                .Subscribe(_ =>
                {
                    GoalPlayer(playerId);
                });
        }

        /// <summary>
        /// PlayerIdからPlayerCoreを取得
        /// </summary>
        public PlayerCore FindPlayer(PlayerId id)
        {
            return players.FirstOrDefault(x => x.PlayerId == id);
        }

        private void Awake()
        {
            //過去に発行した値をすべて保存しておく
            onPlayerSpawned = players
                    .ObserveAdd()
                    .Select(x => x.Value)
                    .Replay();
            onPlayerSpawned.Connect();
        }

        /// <summary>
        /// 生きているプレイヤー一覧を返す
        /// </summary>
        public List<PlayerCore> GetNonGoalPlayers()
        {
            return players
            .Where(x => !goalPlayers.Exists(y => y == x.PlayerId))
            .ToList();
        }

        /// <summary>
        /// 死んでいるプレイヤー一覧を返す
        /// </summary>
        public List<PlayerCore> GetGoalPlayers()
        {
            return goalPlayers.Select(x => FindPlayer(x)).ToList();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
        }

        private void GoalPlayer(PlayerId id)
        {
            if (GetNonGoalPlayers().Count <= 1)
                return;

            var player = FindPlayer(id);

            players.Remove(player);
            goalPlayers.Add(id);

            if (players.Count == 1)  // 残り一人になったら
            {
                var lastPlayer = players[0];
                var lastPlayerId = lastPlayer.PlayerId;
                players.Remove(lastPlayer);
                goalPlayers.Add(lastPlayerId);
            }
        }
    }
}
