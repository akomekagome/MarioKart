using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MC.Damages;
using UniRx.Triggers;
using UniRx;
using MC.Utils;

namespace MC.Entitys
{

    public class MineEntity : MonoBehaviour ,IInstalledEntity, IThrowEntity
    {
        private IAttacker attacker;
        private Rigidbody rb;
        [SerializeField] Explosion explosionPrefab;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            this.OnTriggerEnterAsObservable()
                .Select(x => x.GetComponent<IAttacker>())
                .Where(x => x != null)
                .Subscribe(OnDamage);

            this.OnCollisionEnterAsObservable()
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "Ground")
                .Skip(1)
                .Subscribe(_ => rb.velocity = Vector3.zero);
        }

        private void OnDamage(IAttacker otherAttacker)
        {
            if (otherAttacker.PlayerId == attacker.PlayerId)
                return;
            var explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            explosion.Init(attacker);
            Destroy(this.gameObject);
        }

        public void Init(IAttacker attacker)
        {
            this.attacker = attacker;
        }

        public void OnInstall(Transform playerTf)
        {
            transform.position = playerTf.position.AddSetY(0.5f);
            rb.velocity = -playerTf.forward * 2f;
        }

        public void OnThrow(Transform playerTf)
        {
            transform.position = playerTf.position.AddSetY(0.5f);
            rb.velocity = playerTf.forward * 40f + Vector3.up * 5f;
        }
    }
}
