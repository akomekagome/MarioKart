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
        private Dictionary<int, Transform> checkPointDic = new Dictionary<int, Transform>();
        private List<PlayerCore> players = new List<PlayerCore>();
        private Dictionary<PlayerId, RankInfo> playerRankInfos = new Dictionary<PlayerId, RankInfo>();
        private int playerCount;
        private int checkPointCount;

        public void SetPlayerCount(int count)
        {
            playerCount = count;
        }

        public IReadOnlyReactiveProperty<int> GetRankReactiveProperty(PlayerId playerId)
        {
            return Observable
                .EveryUpdate()
                .Select(_ => playerRankInfos[playerId].rank)
                .DistinctUntilChanged()
                .ToReactiveProperty();
        }

        public void SetCheckPoint(List<Transform> checkPoints)
        {
            DebugExtensions.DebugShowList(checkPoints);
            checkPointCount = checkPoints.Count;
            checkPointDic = checkPoints.Select((x, index) => new { x, index }).ToDictionary(x => x.index + 1, x => x.x);
        }

        private PlayerCore SearchBasePlayerId(PlayerId playerId)
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

            playerRankInfos.Add(core.PlayerId, new RankInfo(0, rank, checkPointCount));

            if (rank == playerCount)
                foreach (var x in playerRankInfos)
                    x.Value.ObserveEveryValueChanged(y => y.laps)
                        .Subscribe(z => Debug.Log("playerId " + x.Key + "laps " + z));

            if (rank == playerCount)
                foreach (var x in playerRankInfos)
                    x.Value.ObserveEveryValueChanged(y => y.checkPointId)
                        .Subscribe(z => Debug.Log("playerId " + x.Key + "checkPointId " + z));

            core.ObserveEveryValueChanged(x => x.transform.position)
                .DelayFrame(1)
                .Subscribe(v => OnMove(core.PlayerId, v));
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
            for (int i = (checkPointId % checkPointCount) + 1; ; i = (i % checkPointCount) + 1)
            {
                if (i == checkPointId) {
                    Debug.LogError("兄（あん）ちゃんなんかおかしくね");
                    break;
                }
                var nextCheckPoint = checkPointDic[i];
                float angle = Vector3.Angle(position - nextCheckPoint.transform.position, nextCheckPoint.transform.forward);
                if (angle <= 90)
                {
                    playerRankInfos[playerId].checkPointId = i;
                    if (i >= 1 && i < checkPointId)
                        playerRankInfos[playerId].laps++;
                }
                else
                {
                    break;
                }
            }
            for (int i = checkPointId; ; i = i == 1 ? checkPointCount : i - 1)
            {
                if (i == (checkPointId % checkPointCount) + 1)
                {
                    Debug.LogError("兄（あん）ちゃんなんかおかしくね");
                    break;
                }
                var nextCheckPoint = checkPointDic[i];
                float angle = Vector3.Angle(position - nextCheckPoint.transform.position, nextCheckPoint.transform.forward);
                if (angle >= 90)
                {
                    playerRankInfos[playerId].checkPointId = i;
                    if (i > checkPointId && i < checkPointCount)
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
            if (count > rank) Debug.Log("rank" + rank + " result " + ComparePositionDown(rank + 1, myRankInfo, position) + " change" + " mylap " + myRankInfo.laps + " otherlap " + playerRankInfos[SearchBaseRunk(count)].laps +  " playerId "+ playerId);
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
            var checkPointTf = checkPointDic[myRankInfo.checkPointId];
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
            //1 0
            //if (myRankInfo.laps > otherRankInfo.laps)
            //    return true;
            //else if (myRankInfo.laps < otherRankInfo.laps)
            //    return false;
            //else if (myRankInfo.checkPointId > otherRankInfo.checkPointId)
            //    return true;
            //else if (myRankInfo.checkPointId < otherRankInfo.checkPointId)
            //    return false;
            if (myRankInfo.laps > otherRankInfo.laps)
                return false;
            else if (myRankInfo.laps < otherRankInfo.laps)
                return true;
            else if (myRankInfo.checkPointId > otherRankInfo.checkPointId)
                return false;
            else if (myRankInfo.checkPointId < otherRankInfo.checkPointId)
                return true;

            var otherPos = SearchBasePlayerId(otherId).transform.position;
            var checkPointTf = checkPointDic[myRankInfo.checkPointId];
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

        //private void OnPass(PlayerId playerId, CheckPoint checkPoint)
        //{
        //    var position = SearchBasePlayerId(playerId).transform.position;
        //    var checkPosition = checkPoint.transform.position;
        //    float angle = Vector3.Angle(position - checkPosition, transform.forward);
        //    if (angle <= 90)
        //    {
        //        playerRankInfos[playerId].checkPoint = checkPoint;
        //    }
        //    else
        //    {
        //        var maxCount = checkPoints.Count();
        //        var passedCheckPoint = playerRankInfos[playerId].checkPoint;
        //        var passedCheckPointId = passedCheckPoint.CheckPointId;
        //        var checkPointId = checkPoint.CheckPointId;
        //        if ((passedCheckPointId % maxCount) + 1 == checkPointId)
        //            return;
        //        else if (passedCheckPointId == checkPointId)
        //        {
        //            if (checkPointId == 1)
        //            {
        //                var lastCheckPoint = checkPoints.Last();
        //                playerRankInfos[playerId].laps--;
        //                playerRankInfos[playerId].checkPoint = lastCheckPoint;
        //            }
        //            var beforeCheckPoint = SearchBaseCheckPointId(checkPointId - 1);
        //            playerRankInfos[playerId].checkPoint = beforeCheckPoint;
        //        }
        //        else
        //            Debug.LogError("例外やで兄（あん）ちゃん: passed" + passedCheckPointId + " pass: " + checkPointId + "name: " + checkPoint.gameObject.name);
        //    }
        //}
    }
}
