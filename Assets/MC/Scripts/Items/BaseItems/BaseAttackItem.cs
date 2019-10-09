using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MC.Damages;
using UniRx;
using UniRx.Triggers;

namespace MC.Items{
    
    public abstract class BaseAttackItem : BaseItem{
        
        protected IObservable<bool> itemButtonObservable;
        protected IAttacker attacker;

        public void Init(IObservable<bool> observable, IAttacker attacker)
        {
            this.itemButtonObservable = observable;
            this.attacker = attacker;
        }

        protected void Start()
        {
            this.OnTriggerEnterAsObservable()
                .Subscribe(hit => Hit(hit.gameObject));
        }

        protected abstract void Hit(GameObject hit);
    }
    
}