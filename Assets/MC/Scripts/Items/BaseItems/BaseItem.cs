using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using System;
using UniRx;

namespace MC.Items
{

    public abstract class BaseItem : MonoBehaviour
    {
        protected Subject<Unit> _finishObservable;
        public IObservable<Unit> OnFinishObseravble{ get { return _finishObservable; }}

        //protected IAttacker attacker;
        //protected IObservable<bool> itemObservable;

        //public void Init(IAttacker attacker, IObservable<bool> observable)
        //{
        //    this.attacker = attacker;
        //    this.itemObservable = observable;
        //}
    }
}
