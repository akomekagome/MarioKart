﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Damages;
using MC.Items;
using System;

public enum PlayerId
{
    Player1 = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3,
    Player5 = 4,
    Player6 = 5,
    Player7 = 6,
    Player8 = 7,
    Player9 = 8,
    Player10 = 9,
    Player11 = 10,
    Player12 = 11
}

namespace MC.Players{

    public class PlayerCore : MonoBehaviour, IPlayerDamageables, IPlayerRankReadle
    {
        private PlayerId _playerId = PlayerId.Player1;
        public PlayerId PlayerId { get { return _playerId; } }

        public PlayerId SetPlayerId(PlayerId playerId) => this._playerId = playerId;

        private int playerRunk = 1;
        public int PlayerRunK { get { return playerRunk; } }

        private ReactiveProperty<bool> _hasControl = new ReactiveProperty<bool>();
        public IReadOnlyReactiveProperty<bool> HasControl { get { return _hasControl.ToReadOnlyReactiveProperty(); } }

        private Subject<ItemType> onGetItemSubject = new Subject<ItemType>();
        public IObservable<ItemType> OnGetItemObservable { get { return onGetItemSubject; } }

        public Subject<Damage> _onPlayerDamageSubject = new Subject<Damage>();
        public IObservable<Damage> OnPlayerDamagedObservable { get { return _onPlayerDamageSubject; } }

        private ReactiveProperty<bool> _playerControllale = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> PlayerControllable { get{ return _playerControllale; } }

        private Subject<RandomItemBox> _getRandomItemBoxSubject = new Subject<RandomItemBox>();
        public IObservable<RandomItemBox> GetRandomItemBoxObservable { get { return _getRandomItemBoxSubject.AsObservable(); } }

        private Subject<ItemType> _getItemSubject = new Subject<ItemType>();
        public IObservable<ItemType> GetItemObservable { get { return _getItemSubject.AsObservable(); } }

        ReactiveProperty<bool> playerAliveReactiveProperty = new BoolReactiveProperty(true);

        //public IObservable<int> OnPlayerDeadAsObservable
        //{
        //    get { return playerAliveReactiveProperty.Where(x => !x).Select(_ => PlayerId); }
        //}

        private void Start()
        {
            
        }

        public void ApplyDamage(Damage damage)
        {
            _onPlayerDamageSubject.OnNext(damage);
        }

        public void ReceiveRandomItem(ItemType itemType)
        {
            _getItemSubject.OnNext(itemType);
        }
    }
}

