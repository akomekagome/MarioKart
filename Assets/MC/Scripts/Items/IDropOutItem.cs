using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;


namespace MC.Items
{

    public interface IDropOutItem : IItem
    {
        void InitDropOutItem(IObservable<Unit> dropOutObservable);
    }
}

