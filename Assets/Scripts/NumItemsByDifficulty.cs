using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumItemsByDifficulty
{
    public readonly int boats;
    public readonly int sharks;
    public readonly int treasureSmall;
    public readonly int treasureMedium;
    public readonly int treasureLarge;

    // adjust difficulty settings and treasure settings here
    private static readonly Dictionary<Difficulty, (int numBoats, float sharkPercentage, int numTreasureSmall, int numTreasureMedium, int numTreasureLarge)> itemsByDifficulty = new Dictionary<Difficulty, (int, float, int, int, int)>
    {
        { Difficulty.Easy, (2, 0.1f, 3, 2, 1) }, // total treasure: 120, 6 squares
        { Difficulty.Medium, (3, 0.15f, 9, 8, 2) }, // total treasure: 550, 17 squares
        { Difficulty.Hard, (5, 0.20f, 11, 17, 3) } // total treasure = 600, 31 squares
    };


    public NumItemsByDifficulty(Difficulty difficulty, int totalTiles)
    {
        var settings = itemsByDifficulty[difficulty];

        boats = settings.numBoats;
        sharks = (int)Math.Floor(totalTiles * settings.sharkPercentage);
        treasureSmall = settings.numTreasureSmall;
        treasureMedium = settings.numTreasureMedium;
        treasureLarge = settings.numTreasureLarge;
    }
}
