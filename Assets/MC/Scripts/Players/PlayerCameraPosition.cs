using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Players
{

    public class PlayerCameraPosition : MonoBehaviour
    {
        private Camera playerCamera;

        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
        }

        public void SetPosition(Transform transform, Vector3 position)
        {
            this.transform.SetParent(transform, false);
            this.transform.localPosition = position;
        }

        public void SetCanvas(Canvas canvas)
        {
        }

        public void SetRect(Rect rect)
        {
            playerCamera.rect = rect;
        }
    }
}
