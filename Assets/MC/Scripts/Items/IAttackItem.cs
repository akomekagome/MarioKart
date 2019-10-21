using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Damages;

namespace MC.Items
{

    public interface IAttackItem : IItem
    {
        void InitAttackItem(IAttacker attacker);
    }
}
