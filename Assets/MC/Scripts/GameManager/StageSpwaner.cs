using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Async;
using MC.Stages;
using Zenject;
using MC.Players;
using MC.Track;
using System.Linq;
using MC.Utils;

namespace MC.GameManager
{

    public class StageSpwaner : MonoBehaviour
    {
        private Subject<Unit> onStageInitComplete = new Subject<Unit>();
        [SerializeField] private StageCore stagePrefab;
        [SerializeField] private PlayerCore playerPrefab;
        [SerializeField] private CheckPoint checkPointPrefab;
        [SerializeField] private PlayerCameraPosition cameraPrefab;
        [Inject] private PlayerManager playerManager;
        [Inject] private RankManager rankManager;
        StageCore stageCore;

        public IObservable<Unit> OnStageInitCompleteAsObservable()
        {
            return onStageInitComplete.AsObservable();
        }

        
        private IEnumerator Start()
        {
            yield return SpawnStage();
            yield return SpawnPlayer();
        }

        IEnumerator SpawnPlayer()
        {
            var createCount = 2;
            // yield return new WaitForSeconds(1);
            var createPositionList = stageCore.GetPlayerStagePosition(createCount);

            var playerCameras = SpawnCamera(createCount);

            //playerPrefab.gameObject.SetActive(false);
            for (var i = 0; i < createCount; ++i)
            {
                var playerCore = Instantiate(playerPrefab);
                playerCore.transform.position = createPositionList[i].position;
                playerCameras[i].SetPosition(playerCore.transform, new Vector3(0f, 1.75f, -8f));
                playerCore.transform.rotation = createPositionList[i].rotation;
                playerCore.Init((PlayerId)i, playerManager, rankManager);
                //playerCore.GetComponent<PlayerColor>().SetPlayerColor(GetPlayerColor(corePlayer.PlayerId));
                //playerCore.GetComponent<MultiPlayerInput>().playerNumber = i + 1;
                //playerCore.gameObject.SetActive(true);
                playerManager.SetPlayer(playerCore);
                rankManager.SetPlayerCount(createCount);
                rankManager.SetPlayer(playerCore, i + 1);
            }

            // 初期化開始
            //playerManager.Init();
            yield break;
        }

        private List<PlayerCameraPosition> SpawnCamera(int count)
        {
            var cameras = Enumerable.Range(0, count)
                .Select(_ => Instantiate(cameraPrefab))
                .ToList();

            var cameraRectTransform = new List<Rect>();
            switch (count)
            {
                case 1:
                    cameraRectTransform = new List<Rect>() { new Rect(0, 0, 1, 1) };
                    break;
                case 2:
                    cameraRectTransform = new List<Rect>() { new Rect(0, 0, 0.5f, 1),
                        new Rect(0.5f, 0, 1, 1) };
                    break;
                case 3:
                    cameraRectTransform = new List<Rect>() { new Rect(0, 0.5f, 0.5f, 1),
                        new Rect(0.5f, 0.5f, 1, 1),
                        new Rect(0, 0, 0.5f, 0.5f)
                    };
                    break;
                case 4:
                    cameraRectTransform = new List<Rect>() { new Rect(0, 0.5f, 0.5f, 1),
                        new Rect(0.5f, 0.5f, 1, 1),
                        new Rect(0, 0, 0.5f, 0.5f),
                        new Rect(0.5f, 0f, 1, 0.5f)
                    };
                    break;
            }
            foreach (var item in cameras.Select((value, index) => new { value, index }))
                item.value.SetRect(cameraRectTransform[item.index]);

            return cameras;
        }

        IEnumerator SpawnStage()
        {
            stageCore = Instantiate(stagePrefab);
            stageCore.transform.position = Vector3.zero;
            rankManager.SetCheckPoint(stageCore.GetCheckPointPositionn);
            yield break;
        }
    }
}
