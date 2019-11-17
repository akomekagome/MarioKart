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
        protected IReadOnlyReactiveProperty<bool> hasUsingItem;
        protected ItemType itemType;

        public IObservable<Unit> FinishObservable { get { return _finishSubject.FirstOrDefault().AsObservable(); } }
        public Usage Usage { get { return usage; } }

        private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
        public IObservable<Unit> Initialized => _initializAsyncSubject;

        public ItemType ItemType { get { return itemType; } }

        public void Init(Transform playerTf, IReadOnlyReactiveProperty<bool> hasUsingItem)
        {
            this.playerTf = playerTf;
            this.hasUsingItem = hasUsingItem;
            _initializAsyncSubject.OnNext(Unit.Default);
            _initializAsyncSubject.OnCompleted();
        }


        protected async void Start()
        {
            gameObject.SetActive(false);
            await Initialized;
            gameObject.SetActive(true);
            OnStart();

            FinishObservable.Subscribe(_ => Destroy(this.gameObject));
        }

        protected abstract void OnStart();

        protected abstract void OnUse();
    }
}

