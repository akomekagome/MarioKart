using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Damages
{

    public class PlayerAttacker : IAttacker
    {
        public PlayerId PlayerId { get; private set; }

        public PlayerAttacker(PlayerId playerId)
        {
            this.PlayerId = PlayerId;
        }
    }
}
