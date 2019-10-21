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
        
        private Subject<Vector2> moveDirectionVector2Observable = new Subject<Vector2>();
        private Subject<float> moveDirectionFloatObservable = new Subject<float>();
        private Subject<bool> moveButtonObservable = new Subject<bool>();
        private Subject<bool> drifRightButtonObsrvable = new Subject<bool>();
        private Subject<bool> drifLeftButtonObsrvable = new Subject<bool>();
        private Subject<bool> jumpButtonObseravable = new Subject<bool>();
        private Subject<bool> itemButtonObseravable = new Subject<bool>();
        private Subject<DriftState> driftstateSubject = new Subject<DriftState>();
        private Subject<List<DriftState>> driftButtonsSubject = new Subject<List<DriftState>>();
        private ReactiveProperty<bool> _isAccelerating = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _isJumping = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _hasUsingItme = new ReactiveProperty<bool>();
        private ReactiveProperty<float> _straightAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<float> _bendAccelerate = new ReactiveProperty<float>();
        private ReactiveProperty<bool> _isDefending = new ReactiveProperty<bool>();


        public IObservable<Vector2> OnMoveDirectionVector2Observable { get { return moveDirectionVector2Observable; }}
        public IObservable<float> OnMoveDirectionFloatObservable{ get { return moveDirectionFloatObservable; }}
        public IObservable<bool> OnMoveButtonObseravable { get { return moveButtonObservable; } }
        public IObservable<bool> OnDriftRightButtonObsrvable { get { return drifRightButtonObsrvable; } }
        public IObservable<bool> OnDriftLeftButtonObsrvable { get { return drifLeftButtonObsrvable; } }
        public IObservable<bool> OnJumpButtonObseravable { get { return jumpButtonObseravable; } }
        public IObservable<bool> OnItemButtonObseravable { get { return itemButtonObseravable; } }
        public IObservable<DriftState> OnDriftButtonObservable { get { return driftstateSubject.AsObservable(); } }
        public IObservable<List<DriftState>> OnDriftButtonsObservable { get { return driftButtonsSubject.AsObservable(); } }
        public IReadOnlyReactiveProperty<bool> IsAccelerating { get { return _isAccelerating.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> IsJumping { get { return _isJumping.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> HasUsingItem { get { return _hasUsingItme.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> StraightAccelerate { get { return _straightAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<float> BendAccelerate { get { return _bendAccelerate.ToReadOnlyReactiveProperty(); } }
        public IReadOnlyReactiveProperty<bool> IsDefending { get { return _isDefending.ToReadOnlyReactiveProperty(); } }

        private void Start()
        {
            this.UpdateAsObservable()
                .Select(_ => Input.GetKey(KeyCode.W))
                .Subscribe(moveButtonObservable);

            this.UpdateAsObservable()
                .Select(_ => new Vector2(Input.GetAxis(AxisType.Horizontal.ToString()),
                                         Input.GetAxis(AxisType.Vertical.ToString())))
                .Subscribe(moveDirectionVector2Observable);

            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis(AxisType.Horizontal.ToString()))
                .Subscribe(moveDirectionFloatObservable);

            this.UpdateAsObservable()
                .Select(_ => Input.GetKeyDown(KeyCode.E))
                .Subscribe(drifRightButtonObsrvable);

            this.UpdateAsObservable()
                .Select(_ => Input.GetKeyDown(KeyCode.Q))
                .Subscribe(drifLeftButtonObsrvable);

            this.UpdateAsObservable()
                .Select(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(jumpButtonObseravable);

            this.UpdateAsObservable()
                .Select(_ => Input.GetKeyDown(KeyCode.S))
                .Subscribe(itemButtonObseravable);

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    var buffer = new List<DriftState>();
                    if (Input.GetKey(KeyCode.E)) buffer.Add(DriftState.FacingRight);
                    else if (Input.GetKey(KeyCode.Q)) buffer.Add(DriftState.FacingLeft);
                    driftButtonsSubject.OnNext(buffer);
                });

            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis(AxisType.Vertical.ToString()))
                .Subscribe(x => _straightAccelerate.Value = Math.Max(0f, x));

            this.UpdateAsObservable()
                .Subscribe(_ => _bendAccelerate.Value = Input.GetAxis(AxisType.Horizontal.ToString()));

            this.UpdateAsObservable()
                .Subscribe(_ => _isJumping.Value = Input.GetKeyDown(KeyCode.Space));

            this.UpdateAsObservable()
                .Subscribe(_ => _isAccelerating.Value = Input.GetKey(KeyCode.W));

            this.UpdateAsObservable()
                .Subscribe(_ => _isDefending.Value = Input.GetKey(KeyCode.RightShift));

            this.UpdateAsObservable()
                .Subscribe(_ => _hasUsingItme.Value = Input.GetKeyDown(KeyCode.R));
        }
    }
    
}
