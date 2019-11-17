using System;
using System.Collections;
using System.Collections.Generic;
using MC.Effects;
using UniRx;
using UnityEngine;
using MC.Entitys;
using MC.Used;

namespace MC.Items
{

    public  class Mushroom : Item, IStrengthenItem, IDropOutItem
    {
        IObservable<Unit> dropOutObservable;
        IEffectAffectable affectable;
        Effect effect;

        public void InitDropOutItem(IObservable<Unit> dropOutObservable)
        {
            this.dropOutObservable = dropOutObservable;
        }

        public void InitStrengthenItem(IEffectAffectable affectable)
        {
            this.affectable = affectable;
        }

        private void Awake()
        {
            base.usage = new UseLimitUsage(1, 0f);
            base.itemType = ItemType.Mushroom;
        }

        protected override void OnStart()
        {
            effect = new AccelerationEffect(2f, 10f);


            base.hasUsingItem
                .SkipWhile(x => !x)
                .Where(x => !x)
                .FirstOrDefault()
                .Subscribe(_ => OnUse());

            dropOutObservable
                .Subscribe(_ => {
                    var entity = entityGenerator.CreateEntity(base.ItemType);
                    ((IInstalledEntity)entity).OnInstall(base.playerTf);
                });
        }

        protected override void OnUse()
        {
            affectable.Affect(effect);
            _finishSubject.OnNext(Unit.Default);
            _finishSubject.OnCompleted();
        }
    }
}
