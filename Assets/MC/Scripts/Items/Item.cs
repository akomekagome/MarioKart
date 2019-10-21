using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using MC.Used;
using Zenject;
using MC.GameManager;

namespace MC.Items
{

    public abstract class Item : MonoBehaviour, IItem
    {
        protected ItemEntityGenerator entityGenerator;
        protected Subject<Unit> _finishSubject = new Subject<Unit>();
        protected Usage usage;
        protected Transform playerTf;
        protected IObservable<Unit> useObservable;
        protected ItemType itemType;

        public IObservable<Unit> FinishObservable { get { return _finishSubject.AsObservable(); } }
        public Usage Usage { get { return usage; } }

        public ItemType ItemType { get { return itemType; } }

        public void Init(Transform playerTf, IObservable<Unit> useObservable)
        {
            this.playerTf = playerTf;
            this.useObservable = useObservable;
        }

        protected void Start()
        {
            useObservable
                .Subscribe(_ => OnUse(),
                () => _finishSubject.OnNext(Unit.Default));

            OnStart();
        }
        protected abstract void OnStart();

        protected abstract void OnUse();
    }
}

