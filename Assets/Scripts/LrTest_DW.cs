using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LrTest_DW : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private LineDrawer_DW line;

    private void Start()
    {
        line.SetUpLine(points);
    }
}
