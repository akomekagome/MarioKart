using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Damages{
    
    public struct Damage{

        public DamageType DamageType { get; private set; }
        public IAttacker Attacker { get; private set; }
        public float InoperableTime { get; private set; }

        public Damage(DamageType damageType, IAttacker attacker, float inoperableTime)
        {
            this.DamageType = damageType;
            this.Attacker = attacker;
            this.InoperableTime = inoperableTime;
        }
    }
}
