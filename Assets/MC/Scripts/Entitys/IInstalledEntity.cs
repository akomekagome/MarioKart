﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Entitys
{

    public interface IInstalledEntity : Entity
    {
        void OnInstall(Vector3 playerPos);
    }
}