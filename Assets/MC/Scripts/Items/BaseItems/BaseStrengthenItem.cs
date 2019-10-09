using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace MC.Items{
    
    public abstract class BaseStrengthenItem : BaseItem
    {
        protected IObservable<bool> itemButtonObservable;

        [SerializeField] private float _itemEffectTime = 4f;
        public float ItemEffectTime { get { return _itemEffectTime; }}

        [SerializeField] private PlayerStateEnum _itemEffect;
        public PlayerStateEnum ItemEffect{ get { return _itemEffect; }}

        protected Subject<PlayerStateEnum> _stateChangeObservable = new Subject<PlayerStateEnum>();
        public IObservable<PlayerStateEnum> StateChangeObservable{ get { return _stateChangeObservable; }}

        //public virtual IObservable<bool> OnuseItemObservable { get; }

        public void Init(IObservable<bool> observable){
            this.itemButtonObservable = observable;
        }
    }
   
}