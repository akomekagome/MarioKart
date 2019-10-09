using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace MC.Items{
    
    public class Turbos : BaseStrengthenItem{

        private void Start()
        {
            base.itemButtonObservable
                .TakeUntil(base.OnFinishObseravble)
                .Where(x => x)
                .Take(3)
                .Subscribe(_ => base._stateChangeObservable.OnNext(base.ItemEffect),
                           () => base._finishObservable.OnNext(Unit.Default)).AddTo(this);
        }
    }
}