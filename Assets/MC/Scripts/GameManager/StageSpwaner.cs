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

namespace MC.GameManager
{

    public class StageSpwaner : MonoBehaviour
    {
        private Subject<Unit> onStageInitComplete = new Subject<Unit>();
        [SerializeField] private StageCore stagePrefab;
        [SerializeField] private PlayerCore playerPrefab;
        [SerializeField] private CheckPoint checkPointPrefab;
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
            yield return SpawnCheckPoint();
            yield return SpawnPlayer();
        }

        IEnumerator SpawnPlayer()
        {
            //var createCount = GameMatchSetting.Instance.CurrentModePlayerLimit;
            var createCount = 1;
            // yield return new WaitForSeconds(1);
            var createPositionList = stageCore.GetPlayerStagePosition(createCount);

            //playerPrefab.gameObject.SetActive(false);
            for (var i = 0; i < createCount; ++i)
            {
                var playerCore = Instantiate(playerPrefab);
                playerCore.transform.position = createPositionList[i].position;
                playerCore.transform.rotation = createPositionList[i].rotation;
                playerCore.SetPlayerId((PlayerId)i);
                //playerCore.GetComponent<PlayerColor>().SetPlayerColor(GetPlayerColor(corePlayer.PlayerId));
                //playerCore.GetComponent<MultiPlayerInput>().playerNumber = i + 1;
                //playerCore.gameObject.SetActive(true);
                Debug.Log(playerManager);
                playerManager.SetPlayer(playerCore);
                rankManager.SetPlayer(playerCore, i + 1);
                rankManager.SetPlayerCount(createCount);
            }

            // 初期化開始
            //playerManager.Init();
            yield break;
        }

        IEnumerator SpawnCheckPoint()
        {
            var createPositionList = stageCore.GetCheckPointPositionn;
            for (var i = 0; i < createPositionList.Count; i++)
            {
                var checkPoint = Instantiate(checkPointPrefab);
                checkPoint.transform.position = createPositionList[i].position;
                checkPoint.transform.rotation = createPositionList[i].rotation;
                checkPoint.SetCheckPointId(i + 1);
                rankManager.SetCheckPoint(checkPoint);
            }
            yield break;
        }

        IEnumerator SpawnStage()
        {
            stageCore = Instantiate(stagePrefab);
            stageCore.transform.position = Vector3.zero;
            //spownItemPositions = stageCore.GetRandomItemSpownPosition();
            yield break;
        }
    }
}
