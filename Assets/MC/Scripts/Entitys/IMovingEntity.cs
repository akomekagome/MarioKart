using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Entitys
{

    public interface IMovingEntity : Entity
    {
        void OnMove(Vector3 position, Vector3 direction);
    }
}
