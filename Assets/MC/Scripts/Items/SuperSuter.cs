using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace MC.Items{
    
    public class SuperSuter : BaseStrengthenItem {
        
        void Start (){
            base.itemButtonObservable
                .TakeUntil(base.OnFinishObseravble)
                .Where(x => x)
                .FirstOrDefault()
                .Subscribe(_ => base._stateChangeObservable.OnNext(base.ItemEffect),
                           () => base._finishObservable.OnNext(Unit.Default)).AddTo(this);
        }
    }
}
