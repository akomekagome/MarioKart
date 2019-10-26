using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace MC.Players
{

    public interface IPlayerRankReadle
    {
        PlayerId SearchBaseRunk(int rank);
        IReadOnlyReactiveProperty<int> GetRankReactiveProperty(PlayerId playerId);
    }
}
