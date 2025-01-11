using System;
using System.Collections.Generic;
using UnityEngine;

public class LightHouse : MonoBehaviour
{
    Dictionary<LightHouseShape, List<(int, int)>> m_LightHouseRevealShapes = new Dictionary<LightHouseShape, List<(int, int)>>
    {
        {LightHouseShape.Basic, new List<(int, int)>() {
              (0,2),
       (-0,1),(0,1),(1,1),
(-2,0),(-1,0),(0,0),(1,0),(2,0),
      (-1,-1),(0,-1),(1,-1),
              (0,-2)
        }},
        {LightHouseShape.Horizontal, new List<(int, int)>() {
        (-3,1),(-2,1),(-1,1),(0,1),(1,1),(2,1),(3,1),
(-4, 0),(-3,0),(-2,0),(-1,0),(0,0),(1,0),(2,0),(3,0),(4, 0),
      (-3,-1),(-2,-1),(-1,-1),(0,-1),(1,-1),(2,-1),(3,-1),
        }},
        {LightHouseShape.Vertical, new List<(int, int)>() {
                 (0, 4),
        (-1, 3), (0, 3), (1, 3),
        (-1, 2), (0, 2), (1, 2),
        (-1, 1), (0, 1), (1, 1),
(-2, 0),(-1, 0), (0, 0), (1, 0), (2, 0),
       (-1, -1), (0, -1), (1, -1),
       (-1, -2), (0, -2), (1, -2),
       (-1, -3), (0, -3), (1, -3),
                 (0, -4)
        }}
    };

    LightHouseShape lightHouseShape;

    public LightHouse()
    {

    }
}

enum LightHouseShape
{
    Basic,
    Horizontal,
    Vertical,
}