using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MC.GameManager;
using UniRx;
using UniRx.Async;
using UnityEngine.UI;

namespace MC.Presenters
{

    public class RankPresenter : MonoBehaviour
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

            rankManager.GetRankReactiveProperty(playerId)
                .Subscribe(x => text.text = x.ToString());
        }
    }
}
