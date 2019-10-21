using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace MC.Stages
{

    public class StageCore : MonoBehaviour
    {
        [SerializeField] private List<Transform> playerStagePosition;
        [SerializeField] private List<Transform> CheckPointPosition;

        public List<Transform> GetPlayerStagePosition (int count) => playerStagePosition.Take(count).ToList();
        public List<Transform> GetCheckPointPositionn => CheckPointPosition.ToList();

    }
}
