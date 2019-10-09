using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using MC.Players.Constants;


namespace MC.Items{
    
    public class Turbo : BaseStrengthenItem {
         
        private void Start()
        {
            base.itemButtonObservable
                .TakeUntil(base.OnFinishObseravble)
                .Where(x => x)
                .FirstOrDefault()
                .Subscribe(_ => base._stateChangeObservable.OnNext(base.ItemEffect),
                           () => base._finishObservable.OnNext(Unit.Default));
        }
    }
}