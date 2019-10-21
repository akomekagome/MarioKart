using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace MC.Used
{

    public class UseLimitAnyIntervalUsage : Usage
    {
        public int UseLimit { get; private set; }
        public IObservable<Unit> IntervalObservable { get; private set; }

        public UseLimitAnyIntervalUsage(int useLimit, IObservable<Unit> intervalObservable)
        {
            this.UseLimit = useLimit;
            this.IntervalObservable = intervalObservable;
        }
    }
}
