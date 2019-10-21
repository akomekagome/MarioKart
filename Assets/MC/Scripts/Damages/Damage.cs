using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Damages{
    
    public struct Damage{

        public DamageType DamageType { get; private set; }

        public Damage(DamageType damageType)
        {
            this.DamageType = damageType;
        }
    }
}
