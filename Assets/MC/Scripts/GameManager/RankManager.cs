using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Utils;
using MC.Track;
using MC.Players;
using UniRx;
using System;
using System.Linq;
using UniRx.Triggers;

namespace MC.GameManager
{

    public class RankManager : MonoBehaviour, IPlayerRankReadle
    {
        private List<Transform> checkPoints = new List<Transform>();
        private List<PlayerCore> players = new List<PlayerCore>();
        private Dictionary<PlayerId, RankInfo> playerRankInfos = new Dictionary<PlayerId, RankInfo>();
        private int playerCount;
        private int checkPointCount;
        private int lapMax;
        public int LapMax => lapMax;

        private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
        public IObservable<Unit> Initialized => _initializAsyncSubject;

        public void SetPlayerCount(int count)
        {
            playerCount = count;
        }

        public void SetLapMax(int max)
        {
            lapMax = max;
        }

        public IObservable<Unit> GetGoalObservable(PlayerId playerId)
        {
            return Observable
                .EveryUpdate()
                .Select(_ => playerRankInfos[playerId].laps)
                .Where(x => x == LapMax)
                .AsUnitObservable();
        }

        public IReadOnlyReactiveProperty<int> GetRankReactiveProperty(PlayerId playerId)
        {
            return Observable
                .EveryUpdate()
                .Select(_ => playerRankInfos[playerId].rank)
                .DistinctUntilChanged()
                .ToReactiveProperty();
        }

        public RankInfo GetRankInfo(PlayerId playerId)
        {
            return playerRankInfos[playerId];
        }

        public Transform GetCheckPoint(int index)
        {
            return checkPoints[index % checkPointCount];
        }

        public void SetCheckPoint(List<Transform> checkPoints)
        {
            this.checkPoints = checkPoints;
            checkPointCount = checkPoints.Count;
            //checkPointDic = checkPoints.Select((x, index) => new { x, index }).ToDictionary(x => x.index + 1, x => x.x);
        }

        public PlayerCore SearchBasePlayerId(PlayerId playerId)
        {
            return players.Find(x => x.PlayerId == playerId);
        }

        public PlayerId SearchBaseRunk(int rank)
        {
            foreach (var key in playerRankInfos.Keys)
                if (playerRankInfos[key].rank == rank)
                    return key;
            Debug.LogError("範囲外やで: " + rank);
            return PlayerId.Player1;
        }

        public int GetRank(PlayerId playerId)
        {
            return playerRankInfos[playerId].rank;
        }

        public void SetPlayer(PlayerCore core, int rank)
        {
            players.Add(core);

            playerRankInfos.Add(core.PlayerId, new RankInfo(0, rank, checkPointCount - 1));

            //if (rank == playerCount)
            //    foreach (var x in playerRankInfos)
            //        x.Value.ObserveEveryValueChanged(y => y.laps)
            //            .Subscribe(z => Debug.Log("playerId " + x.Key + "laps " + z));

            //if (rank == playerCount)
            //    foreach (var x in playerRankInfos)
            //        x.Value.ObserveEveryValueChanged(y => y.checkPointId)
            //            .Subscribe(z => Debug.Log("playerId " + x.Key + "checkPointId " + z));

            core.ObserveEveryValueChanged(x => x.transform.position)
                .DelayFrame(1)
                .Subscribe(v => OnMove(core.PlayerId, v));

            _initializAsyncSubject.OnNext(Unit.Default);
            _initializAsyncSubject.OnCompleted();
        }


        private void OnMove(PlayerId playerId, Vector3 position)
        {
            CompareCheckPoint(playerId, position);
            CompareRank(playerId, position);
        }

        private void CompareCheckPoint(PlayerId playerId, Vector3 position)
        {
            var myRankInfo = playerRankInfos[playerId];
            var checkPointId = myRankInfo.checkPointId;
            for (int i = 1;; i++)
            {
                if (i == checkPointCount) {
                    Debug.LogError("兄（あん）ちゃんなんかおかしくね");
                    break;
                }
                var nextId = (checkPointId + i) % checkPointCount;
                var nextCheckPoint = checkPoints[nextId];
                float angle = Vector3.Angle(position - nextCheckPoint.transform.position, nextCheckPoint.transform.forward);
                if (angle <= 90)
                {
                    playerRankInfos[playerId].checkPointId = nextId;
                    if (nextId >= 0 && nextId < checkPointId)
                        playerRankInfos[playerId].laps++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; ; i++)
            {
                if (i == checkPointCount)
                {
                    Debug.LogError("兄（あん）ちゃんなんかおかしくね");
                    break;
                }
                var nextId = (checkPointId - i) % checkPointCount;
                var nextCheckPoint = checkPoints[nextId];
                float angle = Vector3.Angle(position - nextCheckPoint.transform.position, nextCheckPoint.transform.forward);
                if (angle >= 90)
                {
                    playerRankInfos[playerId].checkPointId = nextId;
                    if (nextId > checkPointId && nextId < checkPointCount)
                        playerRankInfos[playerId].laps--;
                }
                else
                    break;
            }
        }

        private void CompareRank(PlayerId playerId, Vector3 position)
        {
            var rank = GetRank(playerId);
            var myRankInfo = playerRankInfos[playerId];
            int count = rank;
            while (ComparePositionUp(count - 1, myRankInfo, position)) { count -= 1; }
            if (count < rank)
            {
                foreach (var x in playerRankInfos
                    .Where(x => x.Value.rank >= count && x.Value.rank < rank)
                    .ToList())
                    playerRankInfos[x.Key].rank += 1;
                playerRankInfos[playerId].rank = count;
            }

            count = rank;
            while (ComparePositionDown(count + 1, myRankInfo, position)) { count += 1; }
            //if (count > rank) Debug.Log("rank" + rank + " result " + ComparePositionDown(rank + 1, myRankInfo, position) + " change" + " mylap " + myRankInfo.laps + " otherlap " + playerRankInfos[SearchBaseRunk(count)].laps +  " playerId "+ playerId);
            if (count > rank)
            {
                foreach (var x in playerRankInfos
                    .Where(x => x.Value.rank <= count && x.Value.rank > rank)
                    .ToList())
                    playerRankInfos[x.Key].rank--;
                playerRankInfos[playerId].rank = count;
            }
        }

        private bool ComparePositionUp(int count, RankInfo myRankInfo, Vector3 position)
        {
            if (count < 1) return false;
            var otherId = SearchBaseRunk(count);
            var otherRankInfo = playerRankInfos[otherId];
            if (myRankInfo.laps < otherRankInfo.laps)
                return false;
            else if (myRankInfo.laps > otherRankInfo.laps)
                return true;
            else if (myRankInfo.checkPointId < otherRankInfo.checkPointId)
                return false;
            else if (myRankInfo.checkPointId > otherRankInfo.checkPointId)
                return true;

            var otherPos = SearchBasePlayerId(otherId).transform.position;
            var checkPointTf = checkPoints[myRankInfo.checkPointId];
            if (RangeFromSurface(position, checkPointTf) <= RangeFromSurface(otherPos, checkPointTf))
                return false;
            else
            {
                return true;
            }
        }

        private bool ComparePositionDown(int count, RankInfo myRankInfo, Vector3 position)
        {
            if (count > playerCount) return false;
            var otherId = SearchBaseRunk(count);
            var otherRankInfo = playerRankInfos[otherId];
            if (myRankInfo.laps > otherRankInfo.laps)
                return false;
            else if (myRankInfo.laps < otherRankInfo.laps)
                return true;
            else if (myRankInfo.checkPointId > otherRankInfo.checkPointId)
                return false;
            else if (myRankInfo.checkPointId < otherRankInfo.checkPointId)
                return true;

            var otherPos = SearchBasePlayerId(otherId).transform.position;
            var checkPointTf = checkPoints[myRankInfo.checkPointId];
            if (RangeFromSurface(position, checkPointTf) >= RangeFromSurface(otherPos, checkPointTf))
                return false;
            else
                return true;
        }

        private float RangeFromSurface(Vector3 postion, Transform tf)
        {
            var planePositon = tf.position;
            var normal = tf.forward;
            return Vector3.Dot(postion - planePositon, normal);
        }
    }
}
