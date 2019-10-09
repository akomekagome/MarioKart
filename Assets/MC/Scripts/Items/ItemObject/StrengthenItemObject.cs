using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Items{
    
    public class StrengthenItemObject : IItemObject {

        [SerializeField] private ItemEnum item;
        
        public ItemType ItemType { get { return new StrengthenItemType(item); } }
    }
}
