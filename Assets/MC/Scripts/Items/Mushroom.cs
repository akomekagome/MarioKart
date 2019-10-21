using System;
using System.Collections;
using System.Collections.Generic;
using MC.Effects;
using UniRx;
using UnityEngine;
using MC.Entitys;

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

        protected override void OnStart()
        {
            effect = new AccelerationEffect(5f, 10f);
            base.itemType = ItemType.Mushroom;

            dropOutObservable
                .Subscribe(_ => {
                    var entity = entityGenerator.CreateEntity(base.ItemType);
                    ((IInstalledEntity)entity).OnInstall(base.playerTf.position);
                });
        }

        protected override void OnUse()
        {
            affectable.Affect(effect);
        }
    }
}
