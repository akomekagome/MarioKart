using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Used
{

    public class TimeLimitUsage : Usage
    {
        public float TimeLimit { get; private set; }
        public float TimeInterval { get; private set; }

        public TimeLimitUsage(float timeLimit, float timeInterval)
        {
            this.TimeLimit = timeLimit;
            this.TimeInterval = timeInterval;
        }
    }
}
