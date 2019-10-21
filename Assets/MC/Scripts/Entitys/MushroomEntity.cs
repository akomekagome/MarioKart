using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Effects;
using UniRx.Async;
using UniRx.Triggers;
using UniRx;
using MC.Players;

namespace MC.Entitys
{

    public class MushroomEntity : MonoBehaviour, IInstalledEntity
    {
        private bool installed = false;
        private Effect effect;

        public void Init(Effect effect)
        {
            this.effect = effect;
        }

        public void OnInstall(Vector3 playerPos)
        {

        }

        private async UniTask Start()
        {
            await UniTask.WaitUntil(() => installed);

            this.OnTriggerEnterAsObservable()
                .OfType(default(IEffectAffectable))
                .Subscribe(x => x.Affect(effect));
        }
    }
}
