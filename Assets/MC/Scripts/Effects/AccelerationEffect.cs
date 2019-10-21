using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Effects
{

    public class AccelerationEffect : Effect {

        public float Time { get; private set; }
        public float AdditionalSpeed { get; private set; }

        public AccelerationEffect(float time, float additionalSpeed)
        {
            this.Time = time;
            this.AdditionalSpeed = additionalSpeed;
        }
    }
}
