using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesHolder : MonoBehaviour
{
    private Tile water_shade;
    private Tile water_medium;
    private Tile water_darker;
    private Tile water_darkest;
    private Tile black_tile;

    private void Awake()
    {
        water_shade = Resources.Load<Tile>("Water_Shade");
        water_medium = Resources.Load<Tile>("Water_Medium");
        water_darker = Resources.Load<Tile>("Water_Darker");
        water_darkest = Resources.Load<Tile>("Water_Darkest");
        black_tile = Resources.Load<Tile>("BlackTile");
    }

    public Tile GetWaterShadeTile()
    {
        return water_shade;
    }

    public Tile GetWaterMediumTile()
    {
        return water_medium;
    }

    public Tile GetWaterDarkerTile()
    {
        return water_darker;
    }

    public Tile GetWaterDarkestTile()
    {
        return water_darkest;
    }

    public Tile GetBlackTile()
    {
        return black_tile;
    }
}
