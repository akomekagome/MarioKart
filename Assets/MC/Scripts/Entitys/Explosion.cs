using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using UniRx;
using UniRx.Triggers;
using System;

public class Explosion : MonoBehaviour
{
    IAttacker attacker;

    public void Init(IAttacker attacker)
    {
        this.attacker = attacker;
    }

    private void Start()
    {
        Destroy(this.gameObject, 1f);

        this.OnTriggerEnterAsObservable()
            .Subscribe(OnDamage);
    }

    private void OnDamage(Collider c)
    {
        var otherAttacker = c.GetComponent<IAttacker>();
        if (otherAttacker == null || otherAttacker.PlayerId == attacker.PlayerId)
            return;

        var damageabl = c.GetComponent<IDamageable>();
        if(damageabl != null)
        {
            var damage = new Damage(DamageType.Explosion, attacker, 2f);
            damageabl.ApplyDamage(damage);
        }
    }
}
