using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using MC.Players;
using MC.GameManager;


namespace MC.Items
{

    public class RandomItemBox : MonoBehaviour
    {

        private Subject<Unit> _finishSubject = new Subject<Unit>();
        public IObservable<Unit> FinishObservable => _finishSubject.FirstOrDefault().AsObservable();

        private void Start()
        {
            this.OnTriggerEnterAsObservable()
                .Select(x => x.GetComponent<PlayerCore>())
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    var rank = x.PlayerRank.Value;
                    var itemTable = new ItemTable();
                    var itemType = itemTable.GetItem(rank);
                    x.ReceiveRandomItem(itemType);
                    _finishSubject.OnNext(Unit.Default);
                    _finishSubject.OnCompleted();
                    Destroy(this.gameObject);
                });
        }
    }
}

