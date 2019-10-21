using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using UniRx;
using System;

public class PlayerDamageable : MonoBehaviour, IDamageable
{
    private Subject<Damage> _damageSubject = new Subject<Damage>();
    public IObservable<Damage> DamageObservable { get { return _damageSubject.AsObservable(); } }

    public void ApplyDamage(Damage damage)
    {
        _damageSubject.OnNext(damage);
    }
}
