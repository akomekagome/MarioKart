using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using MC.Items;
using MC.Players.Constants;

public enum PlayerStateEnum{
    Normal,
    SpeedUp,
    Invincible
}

namespace MC.Players{
    
    public class PlayerState : MonoBehaviour {

        private Subject<PlayerStateEnum> _playerStateObservable = new Subject<PlayerStateEnum>();

        public IObservable<PlayerStateEnum> OnPlayerStateObseravable { get { return _playerStateObservable; }}

        //private List<PlayerStateEnum> _currentPlayerStates = new List<PlayerStateEnum>();

        private ReactiveCollection<PlayerStateEnum> _currentPlayerStates = new ReactiveCollection<PlayerStateEnum>();
        public IReadOnlyReactiveCollection<PlayerStateEnum> CurrentPlayerStates{ get { return _currentPlayerStates; }}

        //private ReactiveCollection<PlayerStateEnum> _currentPlayerStatesObservable = new ReactiveCollection<PlayerStateEnum>();

        //public IReadOnlyReactiveCollection<PlayerStateEnum> CurrentPlayerStatesObservable { get { return _currentPlayerStatesObservable; }}

        //public List<PlayerStateEnum> CurrentPlayestates { get { return _currentPlayerStates; }}

        private Subject<PlayerStateEnum> _stateDuplicationObservable = new Subject<PlayerStateEnum>();
        public IObservable<PlayerStateEnum> StateDuplicationObserbable { get { return _stateDuplicationObservable; } }

        public bool StateDubplicatonJudge(PlayerStateEnum stateEnum){
            return _currentPlayerStates.Contains(stateEnum);
        }

        private void Start()
        {
            
            //_currentPlayerStates.Add(PlayerStateEnum.SpeedUp);
            //Observable.Timer(TimeSpan.FromMilliseconds(10)).Subscribe(_ => _currentPlayerStates.Add(PlayerStateEnum.SpeedUp));
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.O))
                .Subscribe(_ => _currentPlayerStates.Add(PlayerStateEnum.SpeedUp));
            var itemManager = GetComponent<StrengthenItemManager>();
            var core = GetComponent<PlayerCore>();

            itemManager?.CurrnetItem
                       .OfType(default(BaseStrengthenItem))
                       .Subscribe(x =>
                       {
                           x.StateChangeObservable
                            .Subscribe(z =>
                            {
                                if (StateDubplicatonJudge(z)) _stateDuplicationObservable.OnNext(z);
                                else _currentPlayerStates.Add(z);

                                Observable.Timer(TimeSpan.FromSeconds(PlayreItmeconstant.itemEffectIime[z]))
                                          .TakeUntil(StateDuplicationObserbable)
                                          .Subscribe(_ => _currentPlayerStates.Remove(z),
                                                     () => Debug.Log("stop observable")).AddTo(this);
                              
                            });
                       });
        }
    }

}
