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

    private ReactiveProperty<bool> _hasControl = new ReactiveProperty<bool>(true);
    public IReadOnlyReactiveProperty<bool> HasControl { get { return _hasControl.ToReadOnlyReactiveProperty(); } }

    public void ApplyDamage(Damage damage)
    {
        _damageSubject.OnNext(damage);
    }

    private void Start()
    {
        DamageObservable
            .Subscribe(x => {
                _hasControl.Value = false;
                Observable.Timer(TimeSpan.FromSeconds(x.InoperableTime))
                .Subscribe(_ => _hasControl.Value = true);
            });
    }
}
