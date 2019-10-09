using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using MC.Items;

namespace MC.Players{
    
    public abstract class ItemManager : MonoBehaviour {

        protected ReactiveProperty<BaseItem> _currentItem = new ReactiveProperty<BaseItem>(null);
        public IReadOnlyReactiveProperty<BaseItem> CurrnetItem { get { return _currentItem.ToReadOnlyReactiveProperty(); }}

        protected Subject<PlayerStateEnum> _usedItemEffect = new Subject<PlayerStateEnum>();
        public IObservable<PlayerStateEnum> UsedItemEffectobservable { get { return _usedItemEffect.AsObservable(); } }

        [SerializeField] protected Transform itemPlace;
        protected PlayerCore core;
        protected IObservable<bool> itemButtounObservable;

        private void Start()
        {
            core = GetComponent<PlayerCore>();
            var input = GetComponent<IPlayerInput>();

            itemButtounObservable = input.OnItemButtonObseravable
                                         .TakeUntil(core.OnPlayerDeadAsObservable);

            core.OnGetItemObservable
                .Subscribe(x =>
                {
                    var item = ItemPrehabs.Instance.GetItemPrehabs(x.ItemEnum);
                    if (_currentItem.Value == null) InitItem(item);
                });
        }

        protected abstract void InitItem(GameObject item);
    }
}