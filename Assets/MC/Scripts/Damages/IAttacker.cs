using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MC.Damages{
    
    public interface IAttacker{
        int PlayerId { get; }
        int PlayerRunK { get; }
        ReactiveProperty<bool> PlayerControllable { get; }
    }
}
