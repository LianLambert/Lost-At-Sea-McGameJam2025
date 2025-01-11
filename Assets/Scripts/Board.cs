using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int rows;
    public int columns;
    public Difficulty difficulty;
    public GameObject[,] board;
    public GameObject blankTile;
    private NumItemsByDifficulty numItems;

    public void Start()
    {
        board = GenerateEmptyBoard();

        // populate board given items (calculated by difficulty)
        numItems = new NumItemsByDifficulty(difficulty, rows * columns);
        PopulateBoardItems();

        // update danger levels
        UpdateBoardDangerLevels();

    }

    // adds shore tiles and empty water tiles
    private GameObject[,] GenerateEmptyBoard()
    {
        GameObject[,] emptyBoard = new GameObject[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newTileObject = Instantiate(blankTile);
                Tile tileScript = newTileObject.GetComponent<Tile>();

                // borders should be visible shore tiles
                if (i == 0 || j == 0 || i == rows - 1 || j == columns - 1)
                {
                    tileScript.Initialize(true, 0, TileContent.Shore);
                }
                else
                {
                    tileScript.Initialize(false, 0, TileContent.Empty);
                }

                emptyBoard[i, j] = newTileObject;
            }
        }

        return emptyBoard;
    }

    private void PopulateBoardItems()
    {
        PopulateTileContent(TileContent.Boat, numItems.boats);
        PopulateTileContent(TileContent.Shark, numItems.sharks);
        PopulateTileContent(TileContent.TreasureSmall, numItems.treasureSmall);
        PopulateTileContent(TileContent.TreasureMedium, numItems.treasureMedium);
        PopulateTileContent(TileContent.TreasureLarge, numItems.treasureLarge);
    }

    private void PopulateTileContent(TileContent item, int numItems)
    {
        int numPopulatedItems = 0;

        while (numPopulatedItems < numItems)
        {
            // pick a random tile
            int randomRow = Random.Range(0, rows);
            int randomColumn = Random.Range(0, columns);
            Tile randomTileScript = board[randomRow, randomColumn].GetComponent<Tile>();

            // check if the tile is already occupied
            if (randomTileScript.tileContent == TileContent.Empty)
            {
                randomTileScript.tileContent = item;
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
                UpdateTileDangerLevel(i, j);
            }
        }
    }

    private void UpdateTileDangerLevel(int tileRow, int tileCol)
    {
        int tileDangerLevel = 0;

        List<GameObject> tileNeighbours = GetTileNeighbours(tileRow, tileCol);

        foreach (GameObject neighbouringTile in tileNeighbours)
        {
            if (neighbouringTile.GetComponent<Tile>().tileContent == TileContent.Shark)
            {
                tileDangerLevel++;
            }
        }

        board[tileRow, tileCol].GetComponent<Tile>().dangerLevel = tileDangerLevel;
    }

    private List<GameObject> GetTileNeighbours(int ogRow, int ogCol)
    {
        List<(int, int)> offsets = new List<(int, int)>
        {
            (-1, -1), (-1, 0), (-1, 1), // top row
            ( 0, -1),         ( 0, 1),  // middle row
            ( 1, -1), ( 1, 0), ( 1, 1)  // bottom row
        };

        List<GameObject> neighbours = new List<GameObject>();

        foreach (var offset in offsets)
        {
            int newRow = ogRow + offset.Item1;
            int newCol = ogCol + offset.Item2;

            // ensure new position is inside board
            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < columns)
            {
                neighbours.Add(board[newRow, newCol]);
            }
        }

        return neighbours;
    }

    private Tile GetTileScriptByCoords(int tileRow, int tileCol)
    {
        if (tileRow < 0 || tileRow >= rows || tileCol < 0 || tileCol >= columns)
        {
            throw new ArgumentOutOfRangeException($"Tile coordinates ({tileRow}, {tileCol}) are out of bounds.");
        }
        return board[tileRow, tileCol].GetComponent<Tile>();
    }

    private (int row, int col) GetTileCoordsByScript(Tile tileScript)
    {
        // look for GameObject in board that matches tileScript
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Tile currentTileScript = board[i, j].GetComponent<Tile>();
                if (currentTileScript == tileScript)
                {
                    return (i, j);
                }
            }
        }

        // if script not found
        throw new InvalidOperationException("Tile script not found in board.");
    }

    public void RevealTile(GameObject tileGameObject, bool wasClicked)
    {
        Tile tileScript = tileGameObject.GetComponent<Tile>();

        if (!tileScript.isRevealed)
        {
            // visibly reveal Tile
            tileScript.RevealTile(wasClicked);

            if (tileScript.dangerLevel == 0)
            {
                (int tileRow, int tileCol) = GetTileCoordsByScript(tileScript);

                foreach (GameObject neighbourGameObject in GetTileNeighbours(tileRow, tileCol))
                {
                    RevealTile(neighbourGameObject, false);
                }
            }
        }
    }

    public void PlaceLighthouse(Tile tile, LightHouseType lighthouseType)
    {
        // first update tile content (prevents shark penalty)
        tile.tileContent = TileContent.Lighthouse;

        // then reveal all the applicable tiles
        (int lhRow, int lhCol) = GetTileCoordsByScript(tile);
        foreach (Tile revealTile in GetLighthouseRevealTiles(lighthouseType, lhRow, lhCol)) {
            revealTile.RevealTile(false);
        }
    }

    private List<Tile> GetLighthouseRevealTiles(LightHouseType lighthouseType, int lhRow, int lhCol)
    {
        List<(int, int)> offsets = LightHouse.lightHouseShapeCoordinates[lighthouseType];
        List<Tile> tilesRevealedByLighthouse = new List<Tile>();

        for (int i = 0; i < offsets.Count; i++)
        {
            int tileRow = lhRow + offsets[i].Item1;
            int tileCol = lhCol + offsets[i].Item2;

            // ensure tile coordinates are inside board
            if (tileRow >= 0 && tileRow < rows && tileCol >= 0 && tileCol < columns)
            {
                tilesRevealedByLighthouse.Add(GetTileScriptByCoords(tileRow, tileCol));
            }
        }

        return tilesRevealedByLighthouse;
    }
}
