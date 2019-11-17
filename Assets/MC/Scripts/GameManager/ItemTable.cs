using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemTable
{
    Dictionary<int, Dictionary<ItemType, float>> ItemTableDic = new Dictionary<int, Dictionary<ItemType, float>>()
    {
        { 1, new Dictionary<ItemType, float>(){ { ItemType.AtomicBom, 0f}, { ItemType.GreenMissile, 8f},{ ItemType.Mine, 2f},{ ItemType.Mushroom, 2f},{ ItemType.SmokeGrenade, 8f} } },
        { 2, new Dictionary<ItemType, float>(){ { ItemType.AtomicBom, 2f}, { ItemType.GreenMissile, 7f},{ ItemType.Mine, 2f},{ ItemType.Mushroom, 2f},{ ItemType.SmokeGrenade, 7f} } },
        { 3, new Dictionary<ItemType, float>(){ { ItemType.AtomicBom, 6f}, { ItemType.GreenMissile, 3f},{ ItemType.Mine, 4f},{ ItemType.Mushroom, 4f},{ ItemType.SmokeGrenade, 3f} } },
        { 4, new Dictionary<ItemType, float>(){ { ItemType.AtomicBom, 10f}, { ItemType.GreenMissile, 0f},{ ItemType.Mine, 5f},{ ItemType.Mushroom, 5f},{ ItemType.SmokeGrenade, 0f} } }
    };


    public ItemType GetItem(int rank)
    {
        var itemWeightPairs = ItemTableDic[rank];
        var sortedPairs = itemWeightPairs.OrderByDescending(x => x.Value).ToArray();

        // ドロップアイテムの抽選
        float total = sortedPairs.Select(x => x.Value).Sum();

        float randomPoint = Random.Range(0, total);

        // randomPointの位置に該当するキーを返す
        foreach (KeyValuePair<ItemType, float> elem in sortedPairs)
        {
            if (randomPoint < elem.Value)
            {
                return elem.Key;
            }

            randomPoint -= elem.Value;
        }

        return sortedPairs[sortedPairs.Length - 1].Key;
    }
}
