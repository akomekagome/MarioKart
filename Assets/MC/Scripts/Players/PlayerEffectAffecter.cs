using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Effects;
using UniRx;
using System;
using System.Linq;

public class PlayerEffectAffecter : MonoBehaviour, IEffectAffectable
{
    private Subject<Effect> _effectAeefctSubject = new Subject<Effect>();
    public IObservable<Effect> EffectAeefctObservable { get { return _effectAeefctSubject.AsObservable(); } }

    private ReactiveCollection<Effect> _currentEffects = new ReactiveCollection<Effect>();
    public IReadOnlyReactiveCollection<Effect> CurrentEffects { get { return _currentEffects; } }

    private void Start()
    {
        _currentEffects
            .ObserveAdd()
            .Select(x => x.Value)
            .Subscribe(x =>
            {
                if (x is AccelerationEffect)
                    Observable
                    .Timer(TimeSpan.FromSeconds(((AccelerationEffect)x).Time))
                    .Subscribe(_ => _currentEffects.Remove(x));
            });
    }

    public void Affect(Effect effect)
    {
        _currentEffects.Add(effect);
    }
}
