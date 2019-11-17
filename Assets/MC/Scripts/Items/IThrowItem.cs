using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace MC.Items
{

    public interface IThrowItem : IItem
    {

        void InitThrowItem(IReadOnlyReactiveProperty<float> itemThrowDirection);
    }
}

