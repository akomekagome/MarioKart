using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MC.Utils;
using UnityEngine.UI;
using UniRx;
using Zenject;
using MC.GameManager;
using MC.Players;

[System.Serializable]
public class ItemSpriteTable : Serialize.TableBase<ItemType, Sprite, ItemSpritePair>
{

}

[System.Serializable]
public class ItemSpritePair : Serialize.KeyAndValue<ItemType, Sprite>
{

    public ItemSpritePair(ItemType key, Sprite value) : base(key, value)
    {

    }
}

public class ItemPresenter : MonoBehaviour
{
    [SerializeField] private ItemSpriteTable itemSprites;
    [SerializeField] private List<RectTransform> itemSlotFrame;
    [Inject] PlayerManager playerManager;
    [SerializeField] private PlayerUI playerUI;
    private OrderedDictionary<ItemType, Sprite> itemSpriteOrderdDic = new OrderedDictionary<ItemType, Sprite>();
    private List<RectTransform> slotAreaRectTransforms = new List<RectTransform>();
    private List<Vector2> panelSizes = new List<Vector2>();
    private PlayerId playerId;

    private async void Start()
    {
        await playerUI.Initialized;
        playerId = playerUI.PlayerId;

        foreach (var kvp in itemSprites.GetTable())
            itemSpriteOrderdDic.Add(kvp.Key, kvp.Value);

        foreach (var frame  in itemSlotFrame)
        {
            var size = frame.rect.size;
            var slotArea = new GameObject("SlotArea", typeof(RectTransform));
            slotArea.SetActive(false);
            var slotAreaRectTransform = slotArea.GetComponent<RectTransform>();
            slotAreaRectTransform.anchorMax = Vector2.zero;
            slotAreaRectTransform.anchorMin = Vector2.zero;
            slotAreaRectTransform.SetParent(frame);
            slotAreaRectTransform.localPosition = Vector3.zero;
            var slotAreaSize = size;
            slotAreaSize.y *= itemSpriteOrderdDic.Count;
            slotAreaRectTransform.sizeDelta = slotAreaSize;
            slotAreaRectTransform.pivot = Vector2.zero;
            slotAreaRectTransforms.Add(slotAreaRectTransform);
            panelSizes.Add(size);

            for (var i = 0; i <= itemSpriteOrderdDic.Count; i++)
            {
                var index = i % itemSpriteOrderdDic.Count;
                var sloatpanel = new GameObject("Panel", typeof(Image));
                var sloatpanelRectTransform = sloatpanel.GetComponent<RectTransform>();
                sloatpanelRectTransform.anchorMax = Vector2.zero;
                sloatpanelRectTransform.anchorMin = Vector2.zero;
                sloatpanelRectTransform.SetParent(slotAreaRectTransform);
                sloatpanelRectTransform.pivot = Vector2.zero;
                sloatpanelRectTransform.sizeDelta = size;
                sloatpanelRectTransform.localPosition = Vector2.zero.SetY(size.y * i);
                var image = sloatpanel.GetComponent<Image>();
                image.sprite = itemSpriteOrderdDic[index];
            }
        }

        playerManager.OnPlayerSpawnedAsObservable
                .Where(x => x.PlayerId == playerId)
                .Select(x => x.GetComponent<ItemGetter>())
                .Subscribe(x =>
                {
                    x.TurnSlotItemObsrvable
                    .Subscribe(y =>
                    {
                        var time = y;
                        var itemindex = x.OwnItems.Count - 1;
                        var item = x.OwnItems[itemindex];
                        var itemType = item.ItemType;
                        var index = FindIndex(itemType, itemSpriteOrderdDic);
                        var size = panelSizes[itemindex];
                        var slotAreaRectTransform = slotAreaRectTransforms[itemindex];
                        var slotAreaSize = slotAreaRectTransform.rect.size;
                        var endPosition = Vector2.zero.SetY(-size.y * index);
                        var startPosition = endPosition +
                        (Mathf.Approximately(time % 1f, 0f) ?
                        Vector2.zero : Vector2.zero.SetY(1f - (time % 1f) * slotAreaSize.y));
                        slotAreaRectTransform.gameObject.SetActive(true);
                        slotAreaRectTransform.localPosition = startPosition;

                        Observable.EveryUpdate()
                        .TakeUntil(item.FinishObservable)
                        .Select(_ => x.OwnItems.ToList().IndexOf(item))
                        .DistinctUntilChanged()
                        .Subscribe(z => {
                            slotAreaRectTransform.gameObject.SetActive(false);
                            slotAreaRectTransform = slotAreaRectTransforms[z];
                            slotAreaRectTransform.gameObject.SetActive(true);
                            var panelSize = panelSizes[z];
                            var postion = Vector2.zero.SetY(-panelSize.y * index);
                            slotAreaRectTransform.localPosition = postion;
                        }, () => slotAreaRectTransform.gameObject.SetActive(false)).AddTo(this);

                        Observable.EveryUpdate()
                        .Select(_ => Time.deltaTime)
                        .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(time)))
                        .Subscribe(f =>
                        {
                            var movelocalPosition = slotAreaRectTransform.localPosition.AddSetY(-slotAreaSize.y * f);
                            if (movelocalPosition.y < -slotAreaSize.y)
                                movelocalPosition.y += slotAreaSize.y;
                            slotAreaRectTransform.localPosition = movelocalPosition;
                        }, () => {
                            slotAreaRectTransform.localPosition = endPosition;
                        }).AddTo(this);
                    });
                });
    }

    private int FindIndex(ItemType itemType ,OrderedDictionary<ItemType, Sprite> orderedDic)
    {
        foreach (var x in orderedDic.Select((kvp, index) => (kvp, index)))
            if (x.kvp.Key == itemType)
                return x.index;
        Debug.LogError("imageが足りてないで(あん)兄ちゃん");
        return 0;
    }
}

