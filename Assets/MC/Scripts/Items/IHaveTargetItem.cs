using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Players;

namespace MC.Items
{

    public interface IHaveTargetItem : IItem
    {
        void InitHaveTargetItem(IPlayerRankReadle rankReadle, IPlayerDamageables damageables);
    }
}
