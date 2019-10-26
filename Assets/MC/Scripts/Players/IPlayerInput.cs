using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace MC.Players{
    
    public interface IPlayerInput{

        //IObservable<Vector2> OnMoveDirectionVector2Observable{ get; }
        //IObservable<float> OnMoveDirectionFloatObservable{ get; }
        //IObservable<bool> OnMoveButtonObseravable{ get; }
        //IObservable<bool> OnDriftRightButtonObsrvable { get; }
        //IObservable<bool> OnDriftLeftButtonObsrvable { get; }
        //IObservable<bool> OnJumpButtonObseravable { get; }
        //IObservable<bool> OnItemButtonObseravable { get; }
        //IObservable<DriftState> OnDriftButtonObservable { get; }
        //IObservable<List<DriftState>> OnDriftButtonsObservable { get; }
        IReadOnlyReactiveProperty<bool> IsJumping { get; }
        IReadOnlyReactiveProperty<bool> HasUsingItem { get; }
        IReadOnlyReactiveProperty<float> StraightAccelerate { get; }
        IReadOnlyReactiveProperty<float> BendAccelerate { get; }
        IReadOnlyReactiveProperty<bool> IsDefending { get; }
    }
}