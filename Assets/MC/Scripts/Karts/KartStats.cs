using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Karts
{
    [System.Serializable]
    public struct KartStats
    {
	    public float acceleration;
	    public float braking;
	    public float coastingDrag;
	    public float gravity;
	    public float grip;
	    public float hopHeight;
	    public float reverseAcceleration;
	    public float reverseSpeed;
	    public float topSpeed;
	    public float turnSpeed;
	    public float weight;
    }
}
