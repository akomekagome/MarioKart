using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Karts
{
    public struct GroundInfo
    {
        /// <summary>
        /// Whether or not the capsule is actually touching the ground.  Used to determine whether gravity should be applied or not.
        /// </summary>
        public bool isCapsuleTouching;
        /// <summary>
        /// Whether or not the capsule is close enough to the ground to be treated as though on the ground.  Used to determine most ground based things such as whether on not the input adjusts the velocity of the kart.
        /// </summary>
        public bool isGrounded;
        /// <summary>
        /// Whether or not the kart is close enough to the ground to start behaving like it will when on the ground.  Used for adjusting the orientation of the kart.
        /// </summary>
        public bool isCloseToGround;
        /// <summary>
        /// The current normal of the ground.
        /// </summary>
        public Vector3 normal;
    }
}



