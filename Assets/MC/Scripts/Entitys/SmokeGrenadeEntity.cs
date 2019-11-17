using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using UniRx.Triggers;
using UniRx;
using MC.Utils;
using System;

namespace MC.Entitys
{

    public class SmokeGrenadeEntity : MonoBehaviour, IInstalledEntity, IThrowEntity
    {
        private Rigidbody rb;
        [SerializeField] GameObject smokePrefab;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            this.OnCollisionEnterAsObservable()
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "Ground")
                .Skip(1)
                .Subscribe(_ => {
                    rb.velocity = Vector3.zero;
                    var smoke = Instantiate(smokePrefab);
                    smoke.transform.position = transform.position;
                    Destroy(this.gameObject, 10f);
                    Destroy(smoke.gameObject, 10f);
                });
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
