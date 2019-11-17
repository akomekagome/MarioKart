using System;
using System.Collections;
using System.Collections.Generic;
using MC.Damages;
using UniRx;
using UnityEngine;
using Zenject;
using MC.GameManager;
using MC.Entitys;

namespace MC.Items
{

    public class Mine : Item, IAttackItem, IDropOutItem, IThrowItem
    {
        [Inject] ItemEntityGenerator itemEntityGenerator;
        private IAttacker attacker;
        private IObservable<Unit> dropOutObservable;
        private IReadOnlyReactiveProperty<float> itemThrowDirection;

        private void Awake()
        {
            base.itemType = ItemType.Mine;
        }

        public void InitAttackItem(IAttacker attacker)
        {
            this.attacker = attacker;
        }

        public void InitDropOutItem(IObservable<Unit> dropOutObservable)
        {
            this.dropOutObservable = dropOutObservable;
        }

        public void InitThrowItem(IReadOnlyReactiveProperty<float> itemThrowDirection)
        {
            this.itemThrowDirection = itemThrowDirection;
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
            var entity = (MineEntity)itemEntityGenerator.CreateEntity(base.itemType);
            entity.Init(attacker);
            if (itemThrowDirection.Value > 0f)
                entity.OnThrow(base.playerTf);
            else
                entity.OnInstall(base.playerTf);
        }
    }
}
