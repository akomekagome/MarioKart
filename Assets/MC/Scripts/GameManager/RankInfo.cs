using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Track;

public class RankInfo
{
    public int laps;
    public int rank;
    public int checkPointId;

    public RankInfo(int laps, int rank, int checkPointId)
    {
        this.laps = laps;
        this.rank = rank;
        this.checkPointId = checkPointId;
    }
}
