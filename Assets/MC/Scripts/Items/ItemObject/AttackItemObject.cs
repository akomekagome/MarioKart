using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Items{
    
    public class AttackItemObject : IItemObject{
        
        [SerializeField] private ItemEnum item;

        public ItemType ItemType { get { return new AttackItemType(item); } }
    }
}