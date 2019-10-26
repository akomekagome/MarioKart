using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankTest : MonoBehaviour
{
    [SerializeField] Transform plane;
    [SerializeField] Transform A;

    private void Start()
    {
        Debug.Log(Vector3.Angle(A.position - plane.position, plane.forward));
    }
}
