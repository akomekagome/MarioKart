using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Entitys
{

    public interface IThrowEntity : Entity
    {
        void OnThrow(Transform playerTf);
    }
}
