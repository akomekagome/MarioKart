using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Effects;

namespace MC.Items
{

    public interface IStrengthenItem : IItem
    {
        void InitStrengthenItem(IEffectAffectable affectable);
    }
}
