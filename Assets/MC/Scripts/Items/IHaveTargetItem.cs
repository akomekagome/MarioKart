using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Players;
using UniRx;

namespace MC.Items
{

    public interface IHaveTargetItem : IItem
    {
        void InitHaveTargetItem(IPlayerRankReadle rankReadle, IPlayerDamageables damageables, IReadOnlyReactiveProperty<int> playerRank);
    }
}
