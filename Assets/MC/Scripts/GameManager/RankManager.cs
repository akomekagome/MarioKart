using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Utils;
using MC.Track;
using MC.Players;
using UniRx;
using System;
using System.Linq;

namespace MC.GameManager
{

    public class RankManager : Singleton<RankManager>
    {
        private List<CheckPoint> checkPoints = new List<CheckPoint>();
        private List<PlayerCore> players = new List<PlayerCore>();
        private Dictionary<PlayerId, RankInfo> playerRankInfos = new Dictionary<PlayerId, RankInfo>();
        private int playerCount;

        public void SetPlayerCount(int count)
        {
            playerCount = count;
        }

        public void SetCheckPoint(CheckPoint checkPoint)
        {
            checkPoints.Add(checkPoint);

            checkPoint.CollisionPlayrObservable
                .Subscribe(x => OnPass(x.PlayerId, checkPoint));
        }

        private CheckPoint SearchBaseCheckPointId(int checkPointId)
        {
            return checkPoints.Find(x => x.CheckPointId == checkPointId);
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

            return PlayerId.Player1;
        }

        public int GetRank(PlayerId playerId)
        {
            return playerRankInfos[playerId].rank;
        }

        public void SetPlayer(PlayerCore core, int rank)
        {
            players.Add(core);

            var lastCheckPoint = checkPoints.Last();
            playerRankInfos.Add(core.PlayerId, new RankInfo(0, rank, lastCheckPoint));

            core.ObserveEveryValueChanged(x => x.transform.position)
                .Subscribe(v => OnMove(core.PlayerId, v));
        }


        private void OnMove(PlayerId playerId, Vector3 position)
        {
            var rank = GetRank(playerId);
            var myRankInfo = playerRankInfos[playerId];
            int i;
            for (i = rank - 1; i >= 1; i--) {
                var otherId = SearchBaseRunk(i);
                var otherRankInfo = playerRankInfos[otherId];
                if (myRankInfo.laps < otherRankInfo.laps)
                    continue;
                else if (myRankInfo.laps > otherRankInfo.laps)
                    break;
                else if (myRankInfo.checkPoint.CheckPointId < otherRankInfo.checkPoint.CheckPointId)
                    continue;
                else if (myRankInfo.checkPoint.CheckPointId > otherRankInfo.checkPoint.CheckPointId)
                    break;

                var otherPos = SearchBasePlayerId(playerId).transform.position;
                var checkPointObj = myRankInfo.checkPoint.gameObject;
                if (RangeFromSurface(position, checkPointObj) >= RangeFromSurface(otherPos, checkPointObj))
                    break;
                else
                    continue;
            }
            if (i == rank - 1)
                return;
            else
            {
                foreach (var x in playerRankInfos
                    .Where(x => x.Value.rank <= i && x.Value.rank > rank)
                    .ToList())
                    playerRankInfos[x.Key].rank++;
                playerRankInfos[playerId].rank = i;
            }

            for (i = rank + 1; i <= playerCount; i++)
            {
                var otherId = SearchBaseRunk(rank);
                var otherRankInfo = playerRankInfos[otherId];
                if (myRankInfo.laps > otherRankInfo.laps)
                    continue;
                else if (myRankInfo.laps < otherRankInfo.laps)
                    break;
                else if (myRankInfo.checkPoint.CheckPointId > otherRankInfo.checkPoint.CheckPointId)
                    continue;
                else if (myRankInfo.checkPoint.CheckPointId < otherRankInfo.checkPoint.CheckPointId)
                    break;

                var otherPos = SearchBasePlayerId(playerId).transform.position;
                var checkPointObj = myRankInfo.checkPoint.gameObject;
                if (RangeFromSurface(position, checkPointObj) <= RangeFromSurface(otherPos, checkPointObj))
                    break;
                else
                    continue;
            }
            if (i == rank + 1)
                return;
            else
            {
                foreach (var x in playerRankInfos
                    .Where(x => x.Value.rank >= i && x.Value.rank < rank)
                    .ToList())
                    playerRankInfos[x.Key].rank++;
                playerRankInfos[playerId].rank = i;
            }
        }

        private float RangeFromSurface(Vector3 postion, GameObject plane)
        {
            var planePositon = plane.transform.position;
            var normal = plane.transform.forward;
            return Vector3.Dot(postion - planePositon, normal);
        }

        private void OnPass(PlayerId playerId, CheckPoint checkPoint)
        {
            var passedPoint = playerRankInfos[playerId].checkPoint;
            if (passedPoint != checkPoint)
                playerRankInfos[playerId].checkPoint = checkPoint;
            else {
                var position = SearchBasePlayerId(playerId).transform.position;
                var checkPosition = checkPoint.transform.position;
                float angle = Vector3.Angle(position - checkPosition, transform.forward);
                if (angle <= 90)
                    return;
                else
                {
                    var checkPointId = checkPoint.CheckPointId;
                    if (checkPointId == 1)
                    {
                        var lastCheckPointId = checkPoints.Max(x => x.CheckPointId);
                        var lastCheckPoint = SearchBaseCheckPointId(lastCheckPointId);
                        playerRankInfos[playerId].laps--;
                        playerRankInfos[playerId].checkPoint = lastCheckPoint;
                    }
                    var beforeCheckPoint = SearchBaseCheckPointId(checkPointId - 1);
                    playerRankInfos[playerId].checkPoint = beforeCheckPoint;
                }
            }
        }
    }
}
