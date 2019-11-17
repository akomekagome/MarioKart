using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using MC.Entitys;
using Zenject;
using MC.GameManager;

namespace MC.Items
{

    public class SmokeGrenade : Item, IThrowItem
    {
        IReadOnlyReactiveProperty<float> itemThrowDirection;
        [Inject] ItemEntityGenerator itemEntityGenerator;

        public void InitThrowItem(IReadOnlyReactiveProperty<float> itemThrowDirection)
        {
            this.itemThrowDirection = itemThrowDirection;
        }

        private void Awake()
        {
            base.itemType = ItemType.SmokeGrenade;
        }

        protected override void OnStart()
        {
            base.hasUsingItem
                .SkipWhile(x => !x)
                .Where(x => !x)
                .First()
                .Subscribe(_ => OnUse(),
                () => _finishSubject.OnNext(Unit.Default));
        }

        protected override void OnUse()
        {
            var entity = (SmokeGrenadeEntity)itemEntityGenerator.CreateEntity(base.itemType);
            if (itemThrowDirection.Value > 0f)
                entity.OnThrow(base.playerTf);
            else
                entity.OnInstall(base.playerTf);
        }
    }
}
