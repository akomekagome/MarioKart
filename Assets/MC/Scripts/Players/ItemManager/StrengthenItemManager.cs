using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MC.Items;

namespace MC.Players{
    
    public class StrengthenItemManager : ItemManager{

        protected override void InitItem(GameObject item)
        {
            var itemObj = Instantiate(item);
            var baseitem = itemObj.GetComponent<BaseStrengthenItem>();
            if (baseitem == null) return;
            baseitem.Init(base.itemButtounObservable);
            itemObj.transform.SetParent(base.itemPlace, false);
            base._currentItem.Value = baseitem;
            baseitem.OnFinishObseravble.FirstOrDefault().Subscribe(_ => _currentItem.Value = null);
        }
    }
}