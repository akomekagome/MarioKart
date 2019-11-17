using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

enum AxisType{
    Horizontal,
    Vertical
}

namespace MC.Players{
    
    public class PlayerInput : MonoBehaviour, IPlayerInput{

        private ReactiveProperty<bool> _isJumping = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _hasUsingItme = new ReactiveProperty<bool>();
        private ReactiveProperty<float> _straightAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<float> _bendAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<bool> _isDefending = new ReactiveProperty<bool>();

        public IReadOnlyReactiveProperty<bool> IsJumping { get { return _isJumping.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> HasUsingItem { get { return _hasUsingItme.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> StraightAccelerate { get { return _straightAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> BendAccelerate { get { return _bendAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> IsDefending { get { return _isDefending.ToReadOnlyReactiveProperty(); } }

        public IReadOnlyReactiveProperty<float> ItemThrowDirection => throw new NotImplementedException();

        private void Start()
        {
            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis(AxisType.Vertical.ToString()))
                .Subscribe(x => _straightAccelerate.Value = x);

            this.UpdateAsObservable()
                .Subscribe(_ => _bendAccelerate.Value = Input.GetAxis(AxisType.Horizontal.ToString()));

            this.UpdateAsObservable()
                .Subscribe(_ => _isJumping.Value = Input.GetKeyDown(KeyCode.Space));

            this.UpdateAsObservable()
                .Subscribe(_ => _isDefending.Value = Input.GetKey(KeyCode.RightShift));

            this.UpdateAsObservable()
                .Subscribe(_ => _hasUsingItme.Value = Input.GetKeyDown(KeyCode.R));
        }
    }
    
}
