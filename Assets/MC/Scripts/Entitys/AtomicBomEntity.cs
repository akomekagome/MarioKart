using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;
using MC.Players;
using MC.Utils;
using UniRx;
using UniRx.Triggers;

namespace MC.Entitys
{

    public class AtomicBomEntity : MonoBehaviour, Entity
    {
        [SerializeField] private AtomicExplosion atomicExplosion;
        IAttacker attacker;
        IPlayerRankReadle rankReadle;
        IPlayerDamageables damageables;
        IReadOnlyReactiveProperty<int> playerRank;
        Rigidbody rb;

        public void Init(IAttacker attacker, IPlayerRankReadle rankReadle, IPlayerDamageables damageables, IReadOnlyReactiveProperty<int> playerRank)
        {
            this.attacker = attacker;
            this.rankReadle = rankReadle;
            this.damageables = damageables;
            this.playerRank = playerRank;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            this.OnTriggerEnterAsObservable()
                .Where(x => LayerMask.LayerToName(x.gameObject.layer) == "Ground")
                .First()
                .Subscribe(_ => OnCollison());
        }

        private void OnCollison()
        {
            var atomic = Instantiate(atomicExplosion);
            atomic.Init(attacker);
            atomic.transform.position = transform.position;
            Destroy(this.gameObject);
        }

        public void OnMove()
        {
            var myRank = playerRank.Value;
            var tragetId = rankReadle.SearchBaseRunk(myRank != 1 ? 1 : 2);
            var target = rankReadle.SearchBasePlayerId(tragetId);
            transform.position = target.transform.position.AddSetY(40f);

            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    rb.velocity = (target.transform.position - transform.position).normalized * 60f;
                }).AddTo(this.gameObject);
        }
    }
}
