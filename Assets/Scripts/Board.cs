using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public int rows;
    public int columns;
    public Difficulty difficulty;
    private NumItemsByDifficulty numItems;

    // fields below used to create TileMap
    private Tilemap tilemap;
    private TilesHolder boardTileHolder;
    private Vector3Int origin;

    public void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        boardTileHolder = GetComponent<TilesHolder>();
    }

    public void Start()
    {
        // origin point for tilemap
        origin = tilemap.origin;
        tilemap.ClearAllTiles();

        GenerateEmptyBoard();

        // populate board given items (calculated by difficulty)
        numItems = new NumItemsByDifficulty(difficulty, rows * columns);
        PopulateBoardItems();

        // update danger levels
        UpdateBoardDangerLevels();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetTileOnClick();
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
        Debug.Log(GetCoordsByTile(clickedTile));
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
                    newTile = boardTileHolder.GetShoreTile();
                    newTile.Initialize(true, 0, TileContent.Shore);
                }
                else
                {
                    newTile = boardTileHolder.GetWaterShadeTile();
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

        while (numPopulatedItems < numItems)
        {
            // pick a random tile
            int randomRow = Random.Range(0, rows);
            int randomColumn = Random.Range(0, columns);
            MinesweeperTile randomTile = GetTileByCoords(randomRow, randomColumn); 

            // check if the tile is already occupied
            if (randomTile.tileContent == TileContent.Empty)
            {
                randomTile.tileContent = item;
                numPopulatedItems++;
            }
        }
    }

    private void UpdateBoardDangerLevels()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // to do: check if I switched x and y haha
                MinesweeperTile tile = GetTileByCoords(i, j);
                int dangerLevel = GetTileDangerLevel(i, j);
                tile.dangerLevel = dangerLevel;
                UpdateTileSprite();
            }
        }
    }

    private int GetTileDangerLevel(int tileRow, int tileCol)
    {
        int tileDangerLevel = 0;

        List<MinesweeperTile> tileNeighbours = GetTileNeighbours(tileRow, tileCol);

        foreach (MinesweeperTile neighbouringTile in tileNeighbours)
        {
            if (neighbouringTile.tileContent == TileContent.Shark)
            {
                tileDangerLevel++;
            }
        }

        return tileDangerLevel;
    }

    private List<MinesweeperTile> GetTileNeighbours(int ogRow, int ogCol)
    {
        Vector3Int ogPosition = new Vector3Int(ogCol, ogRow, 0);

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

    private MinesweeperTile GetTileByCoords(int tileRow, int tileCol)
    {
        if (tileRow < 0 || tileRow >= rows || tileCol < 0 || tileCol >= columns)
        {
            throw new ArgumentOutOfRangeException($"Tile coordinates ({tileRow}, {tileCol}) are out of bounds.");
        }

        Vector3Int tilePosition = new(tileCol, tileRow, 0);
        MinesweeperTile tileAtPosition = tilemap.GetTile(tilePosition) as MinesweeperTile;
        return tileAtPosition;

    }


    // returns Vector3Int(-1, -1, -1) if no Tile at coordinate
    private Vector3Int GetCoordsByTile(MinesweeperTile targetTile)
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
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
        // UpdateTileSprite();

        // then reveal all the applicable tiles
        Vector3Int tileCoords = GetCoordsByTile(tile);

        foreach (MinesweeperTile revealTile in GetLighthouseRevealTiles(lighthouseType, tileCoords.x, tileCoords.y))
        {
            RevealTile(revealTile, false);
        }
    }

    private List<MinesweeperTile> GetLighthouseRevealTiles(LightHouseType lighthouseType, int lhRow, int lhCol)
    {
        List<(int, int)> offsets = LightHouse.lightHouseShapeCoordinates[lighthouseType];
        List<MinesweeperTile> tilesRevealedByLighthouse = new();

        for (int i = 0; i < offsets.Count; i++)
        {
            // to do: check if I got the y and x correct
            int tileRow = lhRow + offsets[i].Item1;
            int tileCol = lhCol + offsets[i].Item2;
            Vector3Int tilePosition = new Vector3Int(tileCol, tileRow, 0);

            // ensure tile coordinates are inside board
            if (tilemap.HasTile(tilePosition))
            {
                tilesRevealedByLighthouse.Add(GetTileByCoords(tileRow, tileCol));
            }
        }

        return tilesRevealedByLighthouse;
    }

    private void UpdateTileSprite()
    {
        // to do: Mitch is implementing!
    }
}
