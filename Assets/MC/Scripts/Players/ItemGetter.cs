using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Items;
using System;
using Zenject;
using MC.GameManager;

namespace MC.Players
{

    public class ItemGetter : MonoBehaviour
    {
        [Inject]ItemGenerator itemGenerator;

        private ReactiveCollection<IItem> _ownItems = new ReactiveCollection<IItem>();
        public IReadOnlyReactiveCollection<IItem> OwnItems { get { return _ownItems; } }

        private Subject<IItem> _useItemSubject = new Subject<IItem>();
        public IObservable<IItem> UseItemObservable { get { return _useItemSubject.AsObservable(); } }

        private void Start()
        {
            var core = GetComponent<PlayerCore>();

            core.GetItemObservable
                .Where(_ => OwnItems.Count < 2)
                .Select(x => itemGenerator.CreateItem(x))
                .Subscribe(x => _ownItems.Add(x));

            OwnItems.ObserveCountChanged()
                .Where(x => x > 0)
                .Select(_ => OwnItems[0])
                .DistinctUntilChanged()
                .Subscribe(x => {
                    _useItemSubject.OnNext(x);
                    x?.FinishObservable
                    .FirstOrDefault()
                    .Subscribe(_ => _ownItems.RemoveAt(0));
                });
        }
    }
}
