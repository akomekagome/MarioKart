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

        public IObservable<Vector2> OnMoveDirectionVector2Observable { get { return moveDirectionVector2Observable; }}
        public IObservable<float> OnMoveDirectionFloatObservable{ get { return moveDirectionFloatObservable; }}
        public IObservable<bool> OnMoveButtonObseravable { get { return moveButtonObservable; } }
        public IObservable<bool> OnDriftRightButtonObsrvable { get { return drifRightButtonObsrvable; } }
        public IObservable<bool> OnDriftLeftButtonObsrvable { get { return drifLeftButtonObsrvable; } }
        public IObservable<bool> OnJumpButtonObseravable { get { return jumpButtonObseravable; } }
        public IObservable<bool> OnItemButtonObseravable { get { return itemButtonObseravable; } }
        public IObservable<DriftState> OnDriftButtonObservable { get { return driftstateSubject.AsObservable(); } }
        public IObservable<List<DriftState>> OnDriftButtonsObservable { get { return driftButtonsSubject.AsObservable(); } }

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
        }
    }
    
}