using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using MC.Used;

public enum ItemType
{
    Mushroom,
    Mine,
    GreenMissile,
    SmokeGrenade,
    RedMissile,
    AtomicBom
}

namespace MC.Items
{
    public interface IItem : IUseable
    {
        IObservable<Unit> FinishObservable { get; }
        ItemType ItemType { get; }
        void Init(Transform playerTf, IReadOnlyReactiveProperty<bool> hasUsingItem);
    }
}