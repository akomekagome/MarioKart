using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using MC.Items;

namespace MC.GameManager
{

    public class RandomItemBoxGenerator : MonoBehaviour
    {
        [SerializeField] private RandomItemBox randomItemBox;
        private ReactiveCollection<Transform> RandomItemBoxPosition = new ReactiveCollection<Transform>();

        private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
        public IObservable<Unit> Initialized => _initializAsyncSubject;
        public void SetPositon(List<Transform> transforms)
        {

            foreach (var tf in transforms)
                RandomItemBoxPosition.Add(tf);
        }
        private void Awake()
        {
            RandomItemBoxPosition
                .ObserveAdd()
                .Select(x => x.Value)
                .Subscribe(CreateRandomBox);
        }

        private void CreateRandomBox(Transform tf)
        {
            var randomBox = Instantiate(randomItemBox);
            randomBox.transform.position = tf.position;
            randomBox.transform.rotation = tf.rotation;
            randomBox.FinishObservable
                .Delay(TimeSpan.FromSeconds(3))
                .Subscribe(x => CreateRandomBox(tf));

        }
    }
}
