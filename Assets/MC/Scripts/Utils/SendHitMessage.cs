using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace MC.Utils
{
    public class SendHitMessage : MonoBehaviour
    {
        public IObservable<Collision> OnCollisionEnterObservable { get { return this.OnCollisionEnterAsObservable().AsObservable(); } }
        public IObservable<Collision> OnCollisionExitObservable { get { return this.OnCollisionExitAsObservable().AsObservable(); } }
        public IObservable<Collision> OnCollisionStayObservable { get { return this.OnCollisionStayAsObservable().AsObservable(); } }
    };
}