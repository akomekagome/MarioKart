using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Damages;
using MC.Items;
using System;

namespace MC.Players{

    public class PlayerCore : MonoBehaviour, IDamageable, IAttacker
    {
        private int playerId = 1;
        public int PlayerId { get { return playerId; } }

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

        ReactiveProperty<bool> playerAliveReactiveProperty = new BoolReactiveProperty(true);

        public IObservable<int> OnPlayerDeadAsObservable
        {
            get { return playerAliveReactiveProperty.Where(x => !x).Select(_ => PlayerId); }
        }

        private void Start()
        {
            this.OnCollisionEnterAsObservable()
                .Select(x => x.gameObject.GetComponent<RandomBox>())
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    var itemObject = x.GetRandomItem(PlayerId);
                    onGetItemSubject.OnNext(itemObject.ItemType);
                });

            this.OnPlayerDamagedObservable
                .Do(_ => PlayerControllable.Value = true)
                .Delay(TimeSpan.FromSeconds(3))
                .Subscribe(_ => PlayerControllable.Value = false);
        }

        public void ApplyDamage(Damage damage)
        {
            _onPlayerDamageSubject.OnNext(damage);
        }
    }
}

