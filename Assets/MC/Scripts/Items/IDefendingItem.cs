using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

 namespace MC.Items
{

    public interface IDefendingItem : IItem
    {
        void InitDefendingItem(IObservable<Unit> isDefending);
    }
}
