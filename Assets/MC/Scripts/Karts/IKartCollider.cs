using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Karts
{

    public interface IKartCollider
    {
	    Vector3 ModifyVelocity(KartStats collidingKart, RaycastHit collisionHit);
    }
}
