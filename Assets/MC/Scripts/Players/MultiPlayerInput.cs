﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using UniRx.Triggers;
using UniRx;
using System;

namespace MC.Players
{

    public class MultiPlayerInput : MonoBehaviour, IPlayerInput
    {

        private ReactiveProperty<bool> _isJumping = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _hasUsingItme = new ReactiveProperty<bool>();
        private ReactiveProperty<float> _straightAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<float> _bendAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<bool> _isDefending = new ReactiveProperty<bool>();
        private ReactiveProperty<float> itemThrowDirection = new ReactiveProperty<float>();

        public IReadOnlyReactiveProperty<bool> IsJumping { get { return _isJumping.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> HasUsingItem { get { return _hasUsingItme.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> StraightAccelerate { get { return _straightAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> BendAccelerate { get { return _bendAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> IsDefending { get { return _isDefending.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> ItemThrowDirection => itemThrowDirection.ToReadOnlyReactiveProperty();


        // Start is called before the first frame update
        private async UniTask Start()
        {
            var core = GetComponent<PlayerCore>();
            await core.OnInitialized;
            SetPlayerId(1 + (int)core.PlayerId);
        }

        private void SetPlayerId(int playerId)
        {
            var id = playerId;

            var hori = AxisType.Horizontal.ToString() + id;
            var vert = AxisType.Vertical.ToString() + id;
            var jump = "Jump" + id;
            var useItem = "UseItem" + id;
            var itemVert = "ItemVertical" + id;

            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis(vert))
                .Subscribe(x => _straightAccelerate.Value = x);

            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis(hori))
                .Subscribe(x => _bendAccelerate.Value = x);

            this.UpdateAsObservable()
                .Subscribe(_ => _isJumping.Value = Input.GetButton(jump));

            this.UpdateAsObservable()
                .Subscribe(_ => _isDefending.Value = Input.GetKey(KeyCode.RightShift));

            this.UpdateAsObservable()
                .Subscribe(_ => _hasUsingItme.Value = Input.GetButton(useItem));

            this.UpdateAsObservable()
                .Subscribe(v => itemThrowDirection.Value = Input.GetAxis(itemVert));
        }
    }
}
