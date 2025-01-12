using System;
using System.Collections.Generic;
using UnityEngine;

public class LightHouse : MonoBehaviour
{
    public static Dictionary<LightHouseType, List<(int, int)>> lightHouseShapeCoordinates = new Dictionary<LightHouseType, List<(int, int)>>
    {
        {LightHouseType.Basic, new List<(int, int)>() {
                          (0,2),
                   (-1,1),(0,1),(1,1),
            (-2,0),(-1,0),(0,0),(1,0),(2,0),
                  (-1,-1),(0,-1),(1,-1),
                          (0,-2)
        }},
        {LightHouseType.Horizontal, new List<(int, int)>() {
                    (-3,1),(-2,1),(-1,1),(0,1),(1,1),(2,1),(3,1),
            (-4, 0),(-3,0),(-2,0),(-1,0),(0,0),(1,0),(2,0),(3,0),(4, 0),
                  (-3,-1),(-2,-1),(-1,-1),(0,-1),(1,-1),(2,-1),(3,-1),
        }},
        {LightHouseType.Vertical, new List<(int, int)>() {
                             (0, 4),
                    (-1, 3), (0, 3), (1, 3),
                    (-1, 2), (0, 2), (1, 2),
                    (-1, 1), (0, 1), (1, 1),
                    (-1, 0), (0, 0), (1, 0),
                   (-1, -1), (0, -1), (1, -1),
                   (-1, -2), (0, -2), (1, -2),
                   (-1, -3), (0, -3), (1, -3),
                             (0, -4)
        }}
    };

    LightHouseType lightHouseShape;

    public LightHouse()
    {

    }
}