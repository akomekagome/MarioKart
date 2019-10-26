using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MC.Utils
{

    public static class DebugExtensions
    {
        public static void DebugShowList<T>(List<T> list)
        {
            Debug.Log(string.Join(", ", list.Select(obj => obj.ToString())));
        }
    }
}

