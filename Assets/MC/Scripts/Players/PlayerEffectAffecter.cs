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

    public ReactiveCollection<Effect> CurrentEffects = new ReactiveCollection<Effect>();

    private void Start()
    {
        CurrentEffects
            .ObserveAdd()
            .Select(x => x.Value)
            .Subscribe(x =>
            {
                Debug.Log(((AccelerationEffect)x)?.Time);
                if (x is AccelerationEffect)
                    Observable
                    .Timer(TimeSpan.FromSeconds(((AccelerationEffect)x).Time))
                    .Subscribe(_ => CurrentEffects.Remove(x));
            });
    }

    public void Affect(Effect effect)
    {
        CurrentEffects.Add(effect);
    }
}
