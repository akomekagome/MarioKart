using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MC.Players;
using Zenject;

namespace MC.GameManager
{
    public class PlayerManager : MonoBehaviour
    {
        private List<PlayerCore> players = new List<PlayerCore>();

        public void SetPlayer(PlayerCore playerCore)
        {
            players.Add(playerCore);
        }
    }
}
