using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MC.Players.Constants{
    
    public static class PlayreItmeconstant{
        
        public static readonly Dictionary<PlayerStateEnum, float> itemEffectIime = new Dictionary<PlayerStateEnum, float>(){
            {PlayerStateEnum.SpeedUp, 5f}
        };
    }
}
