using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using UniRx.Triggers;
using UniRx;
using System;
using UniRx.Async;

public class AtomicExplosion : MonoBehaviour
{
    private IAttacker myAttacker;

    private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
    private IObservable<Unit> Initialized => _initializAsyncSubject;

    public void Init(IAttacker attacker)
    {
        this.myAttacker = attacker;
    }

    private void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Subscribe(OnDamage);
    }

    private void OnDamage(Collider c)
    {
        var otherAttacker = c.GetComponent<IAttacker>();

        if (otherAttacker == null || myAttacker == null || otherAttacker.PlayerId == myAttacker.PlayerId)
            return;

        var damageabl = c.GetComponent<IDamageable>();
        if (damageabl != null)
        {
            var damage = new Damage(DamageType.Explosion, myAttacker, 4f);
            damageabl.ApplyDamage(damage);
        }
    }
}
