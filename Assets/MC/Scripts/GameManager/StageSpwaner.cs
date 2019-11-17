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
using UnityEngine.UI;
using MC.Presenters;

namespace MC.GameManager
{

    public class StageSpwaner : MonoBehaviour
    {
        private Subject<Unit> onStageInitComplete = new Subject<Unit>();
        [SerializeField] private StageCore stagePrefab;
        [SerializeField] private PlayerCore playerPrefab;
        [SerializeField] private PlayerCore AIPrefab;
        [SerializeField] private PlayerCameraPosition cameraPrefab;
        [SerializeField] private PlayerUI playerUIPrefab;
        [SerializeField] private Canvas canvas;

        [Inject] private PlayerManager playerManager;
        [Inject] private RankManager rankManager;
        [Inject] private DiContainer container;
        [Inject] private RandomItemBoxGenerator randomItemBoxGenerator;
        StageCore stageCore;

        private Vector2 scrrenResolution;

        public IObservable<Unit> OnStageInitCompleteAsObservable()
        {
            return onStageInitComplete.AsObservable();
        }

        
        private IEnumerator Start()
        {
            scrrenResolution = canvas.GetComponent<RectTransform>().rect.size;
            yield return SpawnStage();
            yield return SpawnPlayer();
        }

        IEnumerator SpawnPlayer()
        {
            var createCount = 1;
            // yield return new WaitForSeconds(1);
            var createPositionList = stageCore.GetPlayerStagePosition;

            var playerCameras = SpawnCamera(createCount);
            var playerUIs = SpawnPlayerUI(createCount);
            //playerPrefab.gameObject.SetActive(false);
            rankManager.SetPlayerCount(createPositionList.Count);
            for (var i = 0; i < createPositionList.Count; ++i)
            {
                var playeObj = container.InstantiatePrefab(i < createCount ? playerPrefab : AIPrefab);
                var playerCore = playeObj.GetComponent<PlayerCore>();
                var playerColor = playeObj.GetComponent<PlayerColor>();
                playerColor.SetPlayerColor(GetPlayerColor(i));
                playeObj.transform.position = createPositionList[i].position;
                playeObj.transform.rotation = createPositionList[i].rotation;
                playerCore.Init((PlayerId)i, playerManager, rankManager);
                if (i < createCount)
                {
                    var playerCamera = playerCameras[i];
                    playerCamera.SetPosition(playeObj.transform, new Vector3(0f, 1.75f, -8f));
                    var playerUI = playerUIs[i];
                    playerUI.SetPlayerId((PlayerId)i);
                }
                //playerCore.GetComponent<PlayerColor>().SetPlayerColor(GetPlayerColor(corePlayer.PlayerId));
                //playerCore.GetComponent<MultiPlayerInput>().playerNumber = i + 1;
                //playerCore.gameObject.SetActive(true);
                playerManager.SetPlayer(playerCore);
                rankManager.SetPlayer(playerCore, i + 1);
            }

            // 初期化開始
            //playerManager.Init();
            yield break;
        }

        private Color GetPlayerColor(int id)
        {
            switch (id)
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.blue;
                case 2:
                    return Color.yellow;
                case 3:
                    return Color.green;
            }
            return Color.black;
        }

        private List<PlayerUI> SpawnPlayerUI(int count)
        {
            var playerUIs = Enumerable.Range(0, count)
                .Select(_ => container.InstantiatePrefab(playerUIPrefab).GetComponent<PlayerUI>())
                .ToList();

            var fieldSize = Vector3.zero;
            var fieldcenters = new List<Vector2>();
            switch (count)
            {
                case 1:
                    fieldSize = scrrenResolution;
                    fieldcenters = new List<Vector2>() { Vector2.zero };
                    break;
                case 2:
                    fieldSize = scrrenResolution.SetX(scrrenResolution.x / 2);
                    fieldcenters = new List<Vector2>() { new Vector2 (-scrrenResolution.x / 4, 0),
                    new Vector2(scrrenResolution.x / 4, 0) };
                    break;
                case 3:
                    fieldSize = scrrenResolution / 2f;
                    fieldcenters = new List<Vector2>()
                    {
                        new Vector2(-scrrenResolution.x / 4, scrrenResolution.y / 4),
                        scrrenResolution / 4,
                        -scrrenResolution / 4
                    };
                    break;
                case 4:
                    fieldSize = scrrenResolution / 2f;
                    fieldcenters = new List<Vector2>()
                    {
                        new Vector2(-scrrenResolution.x / 4, scrrenResolution.y / 4),
                        scrrenResolution / 4,
                        -scrrenResolution / 4,
                        new Vector2(scrrenResolution.x / 4, -scrrenResolution.y / 4)
                    };
                    break;
            }

            for(int i = 0; i < playerUIs.Count; i++)
            {
                var rect = playerUIs[i].GetComponent<RectTransform>();
                playerUIs[i].transform.SetParent(canvas.transform, false);
                rect.localPosition = fieldcenters[i];
                rect.sizeDelta = fieldSize;
            }

            return playerUIs;
        }


        private List<PlayerCameraPosition> SpawnCamera(int count)
        {
            var cameras = Enumerable.Range(0, count)
                .Select(_ => container.InstantiatePrefab(cameraPrefab).GetComponent<PlayerCameraPosition>())
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
            stageCore = container.InstantiatePrefab(stagePrefab).GetComponent<StageCore>();
            stageCore.transform.position = Vector3.zero;
            rankManager.SetCheckPoint(stageCore.GetCheckPointPositionn);
            rankManager.SetLapMax(3);
            DebugExtensions.DebugShowList(stageCore.GetRandomItemBoxPosition);
            randomItemBoxGenerator.SetPositon(stageCore.GetRandomItemBoxPosition);
            yield break;
        }
    }
}
