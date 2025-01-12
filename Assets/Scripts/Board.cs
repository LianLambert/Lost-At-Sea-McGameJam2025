using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class Board : MonoBehaviour
{
    public int rows;
    public int columns;
    public Difficulty difficulty;
    private NumItemsByDifficulty numItems;
    private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

    // tilemap fields
    private Tilemap tilemap;
    private TilesHolder boardTileHolder;
    private Vector3Int origin => tilemap.origin;

    public void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        boardTileHolder = GetComponent<TilesHolder>();
    }

    public void Start()
    {
        tilemap.ClearAllTiles();

        GenerateEmptyBoard();

        // populate board given items (calculated by difficulty)
        numItems = new NumItemsByDifficulty(difficulty, rows * columns);
        PopulateBoardItems();

        // update danger levels
        UpdateBoardDangerLevels();
        AdjustCamera(rows, columns);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetTileOnClick();
        }
    }

    private void AdjustCamera(int width, int height)
    {
        var camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(width / 2f, height / 2f, camera.transform.position.z);
            camera.orthographicSize = Mathf.Max(width, height) / 2f;
        }
    }

    private void GetTileOnClick()
    {
        // Get the position of the mouse in world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Ensure the z-coordinate is 0 for 2D

        // Convert the world position to a cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // Get the tile at the clicked cell position
        MinesweeperTile clickedTile = tilemap.GetTile(cellPosition) as MinesweeperTile;

        if (clickedTile != null && !clickedTile.isRevealed)
        {
            RevealTile(clickedTile, true);
        }

        Debug.Log(clickedTile);
        if (clickedTile != null)
        {
            Debug.Log(GetCoordsByTile(clickedTile));
            PlaceLighthouse(clickedTile, LightHouseType.Basic);
        }
    }

    // adds shore tiles and empty water tiles
    private void GenerateEmptyBoard()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                MinesweeperTile newTile;

                // borders should be visible shore tiles
                if (y == 0 || x == 0 || y == rows - 1 || x == columns - 1)
                {
                    newTile = Object.Instantiate<MinesweeperTile>(boardTileHolder.GetShoreTile());
                    newTile.Initialize(true, 0, TileContent.Shore);
                }
                else
                {
                    newTile = Object.Instantiate<MinesweeperTile>(boardTileHolder.GetWaterShadeTile());
                    newTile.Initialize(false, 0, TileContent.Empty);
                }

                tilemap.SetTile(new Vector3Int(origin.x + x, origin.y + y, origin.z), newTile);
            }
        }
    }

    private void PopulateBoardItems()
    {
        PopulateTileContents(TileContent.Boat, numItems.boats);
        PopulateTileContents(TileContent.Shark, numItems.sharks);
        PopulateTileContents(TileContent.TreasureSmall, numItems.treasureSmall);
        PopulateTileContents(TileContent.TreasureMedium, numItems.treasureMedium);
        PopulateTileContents(TileContent.TreasureLarge, numItems.treasureLarge);
    }

    private void PopulateTileContents(TileContent item, int numItems)
    {
        int numPopulatedItems = 0;

        // do not count shore tiles
        int totalTiles = (rows - 2) * (columns - 2);

        while (numPopulatedItems < numItems)
        {
            // pick a random tile (not a shore tile)
            int randomYCoord = Random.Range(1, rows-1);
            int randomXCoord = Random.Range(1, columns-1);
            Vector3Int position = new(randomXCoord, randomYCoord, 0);

            if (usedPositions.Contains(position))
            {
                continue;
            }

            MinesweeperTile randomTile = (MinesweeperTile)tilemap.GetTile(position);

            // check if the tile is empty
            if (randomTile.tileContent == TileContent.Empty)
            {
                randomTile.tileContent = item;
                randomTile.isRevealed = false;
                usedPositions.Add(position);
                numPopulatedItems++;
                tilemap.SetTile(position, randomTile);

                // if all positions are used, throw an exception or stop
                if (usedPositions.Count >= totalTiles)
                {
                    throw new InvalidOperationException("All available positions are occupied, can't populate more items.");
                }
            }
        }
    }

    private void UpdateBoardDangerLevels()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3Int position = new(x, y, 0);
                MinesweeperTile tile = (MinesweeperTile)tilemap.GetTile(position);
                int dangerLevel = GetTileDangerLevel(x, y);
                tile.dangerLevel = dangerLevel;
                UpdateTileSprite(tile);
            }
        }
    }

    private int GetTileDangerLevel(int tileXCoord, int tileYCoord)
    {
        int tileDangerLevel = 0;

        List<MinesweeperTile> tileNeighbours = GetTileNeighbours(tileXCoord, tileYCoord);

        foreach (MinesweeperTile neighbouringTile in tileNeighbours)
        {
            if (neighbouringTile.tileContent == TileContent.Shark)
            {
                tileDangerLevel++;
            }
        }

        return tileDangerLevel;
    }

    private List<MinesweeperTile> GetTileNeighbours(int tileXCoord, int tileYCoord)
    {
        Vector3Int ogPosition = new Vector3Int(tileXCoord, tileYCoord, 0);

        List<Vector3Int> offsets = new()
        {
            new Vector3Int(-1, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 0), // top row
            new Vector3Int( 0, -1, 0),                           new Vector3Int( 0, 1, 0), // middle row
            new Vector3Int( 1, -1, 0), new Vector3Int( 1, 0, 0), new Vector3Int( 1, 1, 0)  // bottom row
        };

        List<MinesweeperTile> neighbours = new List<MinesweeperTile>();

        foreach (var offset in offsets)
        {
            Vector3Int neighbourPosition = ogPosition + offset;

            if (tilemap.HasTile(neighbourPosition))
            {
                MinesweeperTile tile = tilemap.GetTile(neighbourPosition) as MinesweeperTile;
                neighbours.Add(tile);
            }
        }

        return neighbours;
    }

    // returns Vector3Int(-1, -1, -1) if no Tile at coordinate
    private Vector3Int GetCoordsByTile(MinesweeperTile targetTile)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                MinesweeperTile tile = tilemap.GetTile(position) as MinesweeperTile;

                // check if tile matches target tile
                if (tile == targetTile)
                {
                    return position;
                }
            }
        }

        return new Vector3Int(-1, -1, -1);
    }

    public void RevealTile(MinesweeperTile tile, bool wasClicked)
    {
        // visibly reveal Tile
        tile.isRevealed = true;

        // update tile sprite
        UpdateTileSprite(tile);

        // call functions based on if shark, treasure or boat revealed
        switch (tile.tileContent)
        {
            case TileContent.Empty:

                // to do: decide if we want to use this
                //if (tile.dangerLevel == 0)
                //{
                //    Vector3Int tileCoords = GetCoordsByTile(tile);

                //    foreach (MinesweeperTile neighbourTile in GetTileNeighbours(tileCoords.x, tileCoords.y))
                //    {
                //        RevealTile(neighbourTile, false);
                //    }
                //}
                break;
            case TileContent.Boat:
                // to do: implement function!
                break;
            case TileContent.Shark:
                if (wasClicked)
                {
                    // to do: implement function
                }
                break;
            case TileContent.TreasureSmall:
            case TileContent.TreasureMedium:
            case TileContent.TreasureLarge:
                // to do: implement function
                break;
            // if Lighthouse or Shore, revealing it doesn't do anything
            default:
                break;
        }

    }

    public void PlaceLighthouse(MinesweeperTile tile, LightHouseType lighthouseType)
    {
        // first update tile content (prevents shark penalty)
        tile.tileContent = TileContent.Lighthouse;
        UpdateTileSprite(tile);

        // then reveal all the applicable tiles
        Vector3Int tileCoords = GetCoordsByTile(tile);

        foreach (MinesweeperTile revealTile in GetLighthouseRevealTiles(lighthouseType, tileCoords.x, tileCoords.y))
        {
            RevealTile(revealTile, false);
        }
    }

    private List<MinesweeperTile> GetLighthouseRevealTiles(LightHouseType lighthouseType, int lhXCoord, int lhYCoord)
    {
        List<(int, int)> offsets = LightHouse.lightHouseShapeCoordinates[lighthouseType];
        List<MinesweeperTile> tilesRevealedByLighthouse = new();

        for (int i = 0; i < offsets.Count; i++)
        {
            // to do: check if I got the y and x correct
            int tileXCoord = lhXCoord + offsets[i].Item1;
            int tileYCoord = lhYCoord + offsets[i].Item2;
            Vector3Int tilePosition = new Vector3Int(tileXCoord, tileYCoord, 0);

            // ensure tile coordinates are inside board
            if (tilemap.HasTile(tilePosition))
            {
                tilesRevealedByLighthouse.Add((MinesweeperTile)tilemap.GetTile(tilePosition));
            }
        }

        return tilesRevealedByLighthouse;
    }

    private void UpdateTileSprite(MinesweeperTile tile)
    {
        Debug.Log("Danger level is " + tile.dangerLevel);

        if (tile.tileContent == TileContent.Shore)
            return;

        if (!tile.isRevealed)
        {
            tile.m_AnimatedSprites = Object.Instantiate<MinesweeperTile>(boardTileHolder.GetBlackTile()).m_AnimatedSprites;
            tilemap.RefreshTile(GetCoordsByTile(tile));
            return;
        }

       
        if (tile.dangerLevel == 1)
        {
            tile.m_AnimatedSprites = boardTileHolder.GetWaterMediumTile().m_AnimatedSprites;
            //tilemap.SetTile(tile);
        }
        if (tile.dangerLevel == 2 || tile.dangerLevel == 3)
            tile.m_AnimatedSprites = boardTileHolder.GetWaterDarkerTile().m_AnimatedSprites;
        if (tile.dangerLevel >= 4)
            tile.m_AnimatedSprites = boardTileHolder.GetWaterDarkestTile().m_AnimatedSprites;

        if (tile.tileContent == TileContent.Shark)
            tile.m_AnimatedSprites = boardTileHolder.GetShark().m_AnimatedSprites;

        if (tile.tileContent == TileContent.Boat)
            tile.m_AnimatedSprites = boardTileHolder.GetBoat().m_AnimatedSprites;

        if (tile.tileContent == TileContent.Lighthouse)
            tile.m_AnimatedSprites = boardTileHolder.GetLightHouse().m_AnimatedSprites;

        tilemap.RefreshTile(GetCoordsByTile(tile));
    }
}
