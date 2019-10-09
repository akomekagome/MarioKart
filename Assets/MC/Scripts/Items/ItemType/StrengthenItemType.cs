using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Items{
    
    public class StrengthenItemType : ItemType{

        public ItemEnum ItemEnum{ get { return _itemEnum; }}
        private ItemEnum _itemEnum;

        public StrengthenItemType(ItemEnum itemEnum){
            this._itemEnum = itemEnum;
        }
    }
}
