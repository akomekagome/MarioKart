using System.Collections;
using System.Collections.Generic;
using MC.Damages;
using MC.Players;
using UnityEngine;
using UniRx;
using MC.Entitys;
using MC.GameManager;
using Zenject;

namespace MC.Items{

    public class AtomicBom : Item, IAttackItem, IHaveTargetItem
    {
        IAttacker attacker;
        IPlayerRankReadle rankReadle;
        IPlayerDamageables damageables;
        IReadOnlyReactiveProperty<int> playerRank;
        [Inject] ItemEntityGenerator itemEntityGenerator;

        private void Awake()
        {
            base.itemType = ItemType.AtomicBom;
        }

        public void InitAttackItem(IAttacker attacker)
        {
            this.attacker = attacker;
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
            var entity = (AtomicBomEntity)itemEntityGenerator.CreateEntity(base.itemType);
            entity.Init(attacker, rankReadle, damageables, playerRank);
            entity.OnMove();
        }

        public void InitHaveTargetItem(IPlayerRankReadle rankReadle, IPlayerDamageables damageables, IReadOnlyReactiveProperty<int> playerRank)
        {
            this.rankReadle = rankReadle;
            this.damageables = damageables;
            this.playerRank = playerRank;
        }
    }
}
