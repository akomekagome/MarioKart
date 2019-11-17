using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Players
{

    public class PlayerColor : MonoBehaviour
    {
        [SerializeField] private Renderer miniMapRenderer;

        public void SetPlayerColor(Color color)
        {
            miniMapRenderer.material.color = color;
        }
    }
}
