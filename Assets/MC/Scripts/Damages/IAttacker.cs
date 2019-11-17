using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MC.Damages{
    
    public interface IAttacker{
        PlayerId PlayerId { get; }
    }
}
