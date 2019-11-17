using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Effects;

namespace MC.Stages
{

    public class AccelerationPlate : MonoBehaviour
    {
        private void Start()
        {
            var effect = new AccelerationEffect(1, 5);

            this.OnTriggerEnterAsObservable()
                .Select(x => x.GetComponent<IEffectAffectable>())
                .Where(x => x != null)
                .Subscribe(x => x.Affect(effect));
        }
    }
}
