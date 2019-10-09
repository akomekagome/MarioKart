using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Items{
    
    public interface IItemObject{

        ItemType ItemType { get; }
    }
}