using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MC.Items;
using MC.Damages;
using MC.Used;
using System;

namespace MC.Players
{

    public class ItemUser : MonoBehaviour
    {
        IPlayerInput input;
        IAttacker attacker;
        PlayerCore core;
        PlayerEffectAffecter effectAffecter;
        PlayerDamageable damageable;

        private void Start()
        {
            core = GetComponent<PlayerCore>();
            var getter = GetComponent<ItemGetter>();
            effectAffecter = GetComponent<PlayerEffectAffecter>();
            damageable = GetComponent<PlayerDamageable>();
            input = GetComponent<IPlayerInput>();

            getter.UseItemObservable
                .Subscribe(InitItem);
        }

        private void InitItem(IItem item)
        {
            //var usage = item.Usage;
            //IObservable<Unit> useObservable = new Subject<Unit>();

            //if (usage is TimeLimitUsage)
            //    useObservable =
            //        input.HasUsingItem
            //        .Where(x => x)
            //        .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(((TimeLimitUsage)usage).TimeLimit)))
            //        .ThrottleFirst(TimeSpan.FromSeconds(((TimeLimitUsage)usage).TimeInterval))
            //        .AsUnitObservable();
            //if (usage is UseLimitUsage)
            //    useObservable =
            //        input.HasUsingItem
            //        .Where(x => x)
            //        .ThrottleFirst(TimeSpan.FromSeconds(((UseLimitUsage)usage).TimeInterval))
            //        .Take(((UseLimitUsage)usage).UseLimit)
            //        .AsUnitObservable();
            //if (usage is UseLimitAnyIntervalUsage)
            //    useObservable =
            //        Observable.FromCoroutine<Unit>(observer => AnyInterval(observer, input.HasUsingItem, ((UseLimitAnyIntervalUsage)usage).IntervalObservable))
            //        .Take(((UseLimitAnyIntervalUsage)usage).UseLimit)
            //        .AsUnitObservable();]

            item.Init(this.transform, input.HasUsingItem);

            if (item is IAttackItem)
                ((IAttackItem)item).InitAttackItem(core);
            if (item is IStrengthenItem)
                ((IStrengthenItem)item).InitStrengthenItem(effectAffecter);
            if (item is IHaveTargetItem)
                ((IHaveTargetItem)item).InitHaveTargetItem(core.RankReadle, core.PlayerDamageables, core.PlayerRank);
            if (item is IDropOutItem)
            {
                var dropOutObservable =
                    damageable.DamageObservable
                    .Where(x => x.DamageType == DamageType.Thunder)
                    .AsUnitObservable();
                ((IDropOutItem)item).InitDropOutItem(dropOutObservable);
            }
            if (item is IDefendingItem)
            {
                var isDefending =
                    input.IsDefending
                    .Where(x => x)
                    .TakeWhile(x => x)
                    .AsUnitObservable();
                ((IDefendingItem)item).InitDefendingItem(isDefending);
            }
            if(item is IThrowItem)
            {
                var itemThrowDirection =
                    input.ItemThrowDirection;
                ((IThrowItem)item).InitThrowItem(itemThrowDirection);
            }
        }

        IEnumerator AnyInterval(IObserver<Unit> observable, IReadOnlyReactiveProperty<bool> hasUsingItem, IObservable<Unit> intervalObservable)
        {
            while (true)
            {
                yield return hasUsingItem.Where(x => x).FirstOrDefault().ToYieldInstruction();
                observable.OnNext(Unit.Default);
                yield return intervalObservable.ToYieldInstruction();
            }
        }
    }
}
