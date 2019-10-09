using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Damages{
    
    public interface IDamageable
    {
        void ApplyDamage(Damage damage);
    }
}
