using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Async;
using MC.GameManager;
using UnityEngine.UI;

public class LapPresenter : MonoBehaviour
{
    [Inject] private RankManager rankManager;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private Text text;
    private PlayerId playerId;

    private async void Start()
    {
        await UniTask.WhenAll(rankManager.Initialized.ToUniTask(),
            playerUI.Initialized.ToUniTask());

        playerId = playerUI.PlayerId;
        var lapMax = rankManager.LapMax;

        rankManager.GetRankInfo(playerId)
            .ObserveEveryValueChanged(x => x.laps)
            .Subscribe(x => text.text = Mathf.Max(1, x) + "/" + lapMax);
    }
}
