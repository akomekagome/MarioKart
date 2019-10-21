using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Used
{

    public class UseLimitUsage : Usage
    {
        public int UseLimit { get; private set; }
        public float TimeInterval { get; private set; }

        public UseLimitUsage(int useLimit, float timeInterval)
        {
            this.UseLimit = useLimit;
            this.TimeInterval = timeInterval;
        }
    }
}
