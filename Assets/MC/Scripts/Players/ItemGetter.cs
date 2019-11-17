using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Items;
using System;
using Zenject;
using MC.GameManager;
using UniRx.Async;
using MC.Utils;
using System.Linq;

namespace MC.Players
{

    public class ItemGetter : MonoBehaviour
    {
        [Inject]ItemGenerator itemGenerator;

        private ReactiveCollection<IItem> _ownItems = new ReactiveCollection<IItem>();
        public IReadOnlyReactiveCollection<IItem> OwnItems { get { return _ownItems; } }

        private Subject<IItem> _useItemSubject = new Subject<IItem>();
        public IObservable<IItem> UseItemObservable { get { return _useItemSubject.AsObservable(); } }

        private Subject<float> _turnSlotItemSubject = new Subject<float>();
        public IObservable<float> TurnSlotItemObsrvable => _turnSlotItemSubject.AsObservable();

        PlayerCore core;

        private void Start()
        {
            core = GetComponent<PlayerCore>();

            core.GetItemObservable
                .Where(_ => OwnItems.Count < 2)
                .Select(x => itemGenerator.CreateItem(x))
                .Do(x =>
                {
                    _ownItems.Add(x);
                    _turnSlotItemSubject.OnNext(2f);
                })
                .Delay(TimeSpan.FromSeconds(2f))
                .SelectMany(x => UniTask.WaitUntil(() => x == OwnItems[0])
                .ToObservable()
                .Do(_ => {
                    _useItemSubject.OnNext(x);
                    x.FinishObservable
                    .Subscribe(_2 => _ownItems.Remove(x));
                }))
                .Subscribe();
        }
    }
}
