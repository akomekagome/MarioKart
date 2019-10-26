using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Players;
using System;

public enum CompareType
{
    CompareX,
    CompareY,
    CompareZ
}

namespace MC.Track
{

    public class CheckPoint : MonoBehaviour
    {
        private int _checkPointId;
        public int CheckPointId { get { return _checkPointId; } }
        public int SetCheckPointId(int Id) => _checkPointId = Id;

        private Subject<PlayerCore> _collisionPlayerSubject = new Subject<PlayerCore>();
        public IObservable<PlayerCore> CollisionPlayrObservable { get { return _collisionPlayerSubject.AsObservable(); } }

        private void Start()
        {
            this.OnTriggerStayAsObservable()
                .Select(x => x.GetComponent<PlayerCore>())
                .Where(x => x != null)
                .Subscribe(_collisionPlayerSubject);
        }
    }
}
