using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Track;

public class RankInfo
{
    public int laps;
    public int rank;
    public CheckPoint checkPoint;

    public RankInfo(int laps, int rank, CheckPoint checkPoint)
    {
        this.laps = laps;
        this.rank = rank;
        this.checkPoint = checkPoint;
    }
}
