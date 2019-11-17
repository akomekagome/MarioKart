using System.Collections;
using System.Collections.Generic;
using MC.Damages;
using UnityEngine;
using UniRx;
using MC.Entitys;
using Zenject;
using MC.GameManager;

namespace MC.Items
{

    public class GreenMissile : Item, IAttackItem, IThrowItem
    {
        [Inject] ItemEntityGenerator itemEntityGenerator;
        IAttacker attacker;
        IReadOnlyReactiveProperty<float> itemThrowDirection;

        private void Awake()
        {
            base.itemType = ItemType.GreenMissile;
        }

        public void InitAttackItem(IAttacker attacker)
        {
            this.attacker = attacker;
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
                .FirstOrDefault()
                .Subscribe(_ => OnUse(),
                () => _finishSubject.OnNext(Unit.Default));
        }

        protected override void OnUse()
        {
            var entity = (GreenMissileEntity)itemEntityGenerator.CreateEntity(base.itemType);
            entity.Init(attacker);
            entity.OnMove(playerTf.position, playerTf.forward * (itemThrowDirection.Value >= 0f ? 1f : -1f));
        }
    }
}
