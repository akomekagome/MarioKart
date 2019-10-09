using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Damages;

namespace MC.Items{
    
    public class MIssile : BaseAttackItem {

        protected override void Hit(GameObject hit)
        {
            var damageable = hit.GetComponent<IDamageable>();
            var hitAttacker = hit.GetComponent<IAttacker>();

            if (hitAttacker?.PlayerId == base.attacker.PlayerId ) return;
            var damage = new Damage();
            damageable?.ApplyDamage(damage);
        }
    }
}
