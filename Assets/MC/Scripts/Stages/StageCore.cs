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
        [SerializeField] private List<Transform> RandomItemBoxPosition;

        public List<Transform> GetPlayerStagePosition => playerStagePosition.ToList();
        public List<Transform> GetCheckPointPositionn => CheckPointPosition.ToList();
        public List<Transform> GetRandomItemBoxPosition => RandomItemBoxPosition.ToList();

    }
}
