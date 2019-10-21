using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Utils;
using MC.Items;

namespace MC.GameManager
{
    [System.Serializable]
    public class ItemTable : Serialize.TableBase<ItemType, GameObject, ItemPair>
    {

    }

    [System.Serializable]
    public class ItemPair : Serialize.KeyAndValue<ItemType, GameObject>
    {

        public ItemPair(ItemType key, GameObject value) : base(key, value)
        {

        }
    }

    public class ItemGenerator : MonoBehaviour
    {
        [SerializeField] private ItemTable itemTable;

        public IItem CreateItem(ItemType itemType)
        {
            var go = Instantiate(itemTable.GetTable()[itemType]);
            var item = go.GetComponent<IItem>();
            return item;
        }
    }
}
