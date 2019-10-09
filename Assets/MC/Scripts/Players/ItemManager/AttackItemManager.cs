using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Items;
using UniRx;

namespace MC.Players{
    
    public class AttackItemManager : ItemManager{
        
        protected override void InitItem(GameObject item)
        {
            var itemObj = Instantiate(item);
            var baseitem = itemObj.GetComponent<BaseAttackItem>();
            if (baseitem == null) return;
            baseitem.Init(base.itemButtounObservable, base.core);
            itemObj.transform.SetParent(base.itemPlace, false);
            base._currentItem.Value = baseitem;
            baseitem.OnFinishObseravble.FirstOrDefault().Subscribe(_ => _currentItem.Value = null);
        }
    }
}
