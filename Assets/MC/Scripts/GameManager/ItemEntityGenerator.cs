using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Entitys;

namespace MC.GameManager
{
    [System.Serializable]
    public class ItemEntityTable : Serialize.TableBase<ItemType, GameObject, ItemEntityPair>
    {

    }

    [System.Serializable]
    public class ItemEntityPair : Serialize.KeyAndValue<ItemType, GameObject>
    {

        public ItemEntityPair(ItemType key, GameObject value) : base(key, value)
        {

        }
    }

    public class ItemEntityGenerator : MonoBehaviour
    {
        [SerializeField] private ItemEntityTable itemEntityTable;

        public Entity CreateEntity(ItemType itemType)
        {
            var go = Instantiate(itemEntityTable.GetTable()[itemType]);
            var item = go.GetComponent<Entity>();
            return item;
        }
    }
}
