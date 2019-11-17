using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using UniRx.Triggers;
using UniRx;
using MC.Utils;

namespace MC.Entitys
{

    public class GreenMissileEntity : MonoBehaviour, IMovingEntity
    {
        Rigidbody rb;
        IAttacker attacker;
        [SerializeField] Explosion explosionPrefab;

        public void Init(IAttacker attacker)
        {
            this.attacker = attacker;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            this.OnTriggerEnterAsObservable()
                .Select(x => x.GetComponent<IAttacker>())
                .Where(x => x == null || x.PlayerId != attacker.PlayerId)
                .First()
                .Subscribe(_ => OnDamage());
        }

        private void OnDamage()
        {
            var explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.Init(attacker);
            Destroy(this.gameObject);
        }

        public void OnMove(Vector3 position, Vector3 direction)
        {
            transform.position = position.AddSetY(0.5f);
            transform.LookAt(transform.position + direction);
            rb.velocity = direction * 50f;
        }
    }
}
