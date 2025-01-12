using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesHolder : MonoBehaviour
{
    private MinesweeperTile water_shade;
    private MinesweeperTile water_medium;
    private MinesweeperTile water_darker;
    private MinesweeperTile water_darkest;
    private MinesweeperTile black_tile;
    private MinesweeperTile shore_tile;

    private void Awake()
    {
        water_shade = Resources.Load<MinesweeperTile>("Water_Shade");
        water_medium = Resources.Load<MinesweeperTile>("Water_Medium");
        water_darker = Resources.Load<MinesweeperTile>("Water_Darker");
        water_darkest = Resources.Load<MinesweeperTile>("Water_Darkest");
        black_tile = Resources.Load<MinesweeperTile>("BlackTile");
        shore_tile = Resources.Load<MinesweeperTile>("ShoreTile");
    }

    public MinesweeperTile GetWaterShadeTile()
    {
        return water_shade;
    }

    public MinesweeperTile GetWaterMediumTile()
    {
        return water_medium;
    }

    public MinesweeperTile GetWaterDarkerTile()
    {
        return water_darker;
    }

    public MinesweeperTile GetWaterDarkestTile()
    {
        return water_darkest;
    }

    public MinesweeperTile GetBlackTile()
    {
        return black_tile;
    }

    public MinesweeperTile GetShoreTile()
    {
        return shore_tile;
    }

    public MinesweeperTile GetShark()
    {
        return Resources.Load<MinesweeperTile>("Shark");
    }
    public MinesweeperTile GetSharkShadow()
    {
        return Resources.Load<MinesweeperTile>("SharkShadow");
    }

    public MinesweeperTile GetBoat()
    {
        return Resources.Load<MinesweeperTile>("Boat");
    }

    public MinesweeperTile GetLightHouse()
    {
        return Resources.Load<MinesweeperTile>("LightHouse");
    }

    public MinesweeperTile GetCoin()
    {
        return Resources.Load<MinesweeperTile>("Coin");
    }
}
