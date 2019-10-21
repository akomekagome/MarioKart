using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using MC.Players;

namespace MC.Items
{

    public class RandomItemBox : MonoBehaviour
    {
        private void Start()
        {
            this.OnTriggerEnterAsObservable()
                .Select(x => x.GetComponent<PlayerCore>())
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    var itemType = ItemType.Mushroom;
                    x.ReceiveRandomItem(itemType);
                    Destroy(this.gameObject);
                });
        }
    }
}

