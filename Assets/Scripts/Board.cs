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
using TMPro;

public class Board : MonoBehaviour
{
    // board properties
    public int rows;
    public int columns;
    public Difficulty difficulty => GameManager.difficulty;
    private NumItemsByDifficulty numItems;
    private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

    // tilemap fields
    private Tilemap tilemap;
    public Tilemap shadowTileMap;
    public Tilemap BelowTileMap;
    public Tilemap SharkMarkerTileMap;
    public Tilemap DangerNumbersTileMap;

    private TilesHolder boardTileHolder;
    private Vector3Int origin => tilemap.origin;

    // updating values
    public void SetPropogate(bool val)
    {
        propagateTiles = val;
    }

    private bool propagateTiles = false;
    public GameObject DangerNumberText;
    public GameObject SharkMarkerX;
    public GameObject BoatPrefab;
    public GameObject TreasurePrefab;
    private Dictionary<TileContent, int> treasureValues = new Dictionary<TileContent, int>
    {
        { TileContent.TreasureSmall, 10 },
        { TileContent.TreasureMedium, 20 },
        { TileContent.TreasureLarge, 50 },
    };

    public void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        boardTileHolder = GetComponent<TilesHolder>();
    }

    public void Start()
    {
        tilemap.ClearAllTiles();
        ConfigureBoardAndCamera();

        GenerateEmptyBoard();

        // populate board given items (calculated by difficulty)
        numItems = new NumItemsByDifficulty(difficulty, rows * columns);
        PopulateBoardItems();

        // update danger levels
        UpdateBoardDangerLevels();

        GameObject.FindGameObjectWithTag("HeartText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLives.ToString();
    }

    private void Update()
    {
        if (GameManager.gameIsOver)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            GetTileOnClick();
        }
        else if (Input.GetMouseButtonUp(0)) // Detect when the mouse button is released
        {
            HandleDropAction();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }

        //TODO REMOVE THIS
        else if (Input.GetKeyDown(KeyCode.P)) // Detect when the mouse button is released
        {
            GameManager.numLightHouses++;
            GameObject.FindGameObjectWithTag("NumLightHouses").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLightHouses.ToString();

        }
    }

    private void HandleDropAction()
    {
        if (DragAndDropLightHouse.isDragging)
        {
            // Get the position of the mouse in world space
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f; // Ensure the z-coordinate is 0 for 2D

            // Convert the world position to a cell position
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            MinesweeperTile droppedTile = tilemap.GetTile(cellPosition) as MinesweeperTile;

            Destroy(FindObjectOfType<DragAndDropLightHouse>().currentPrefab);
            DragAndDropLightHouse.isDragging = false;

            if (droppedTile != null)
            {
                GameManager.numLightHouses--;
                GameObject.FindGameObjectWithTag("NumLightHouses").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLightHouses.ToString();

                PlaceLighthouse(droppedTile, DragAndDropLightHouse.draggingLightHouseType);
            }
        }
    }

    private void HandleRightClick()
    {
        // find the position of the cell
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Ensure the z-coordinate is 0 for 2D
        Vector3Int cellPosition = SharkMarkerTileMap.WorldToCell(worldPosition);
        Vector3 cellWorldPosition = SharkMarkerTileMap.CellToWorld(cellPosition);

        // Get the tile at the clicked cell position
        MinesweeperTile tile = tilemap.GetTile(cellPosition) as MinesweeperTile;

        if (tile != null && !tile.isRevealed)
        {
            if (tile.sharkMarker)
            {
                Destroy(tile.sharkMarker);
                tile.sharkMarker = null;
            }
            else
            {
                // create new text at correct location
                GameObject sharkMarkerObject = Instantiate(SharkMarkerX, cellWorldPosition, Quaternion.identity);
                sharkMarkerObject.transform.position += new Vector3(0.55f, 0.45f, 0);
                tile.sharkMarker = sharkMarkerObject;
            }
        }
    }

    private void AdjustCamera(Vector3 newPosition, float newSize)
    {
        var camera = Camera.main;
        if (camera != null)
        {
            // Update camera position and size
            camera.transform.position = newPosition;
            camera.orthographicSize = newSize;
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void ConfigureBoardAndCamera()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                rows = 10;
                columns = 14;
                AdjustCamera(new Vector3(8.85f, 5.02f, -10), 4.98f);
                break;

            case Difficulty.Medium:
                rows = 20;
                columns = 28;
                AdjustCamera(new Vector3(17.75f, 10f ,-10), 9.97f);
                break;

            case Difficulty.Hard:
                rows = 30;
                columns = 42;
                AdjustCamera(new Vector3(26.5f, 15, -10), 14.9f);
                break;

            default:
                Debug.LogError("Unknown difficulty!");
                break;
        }

        Debug.Log($"Board configured for {difficulty} difficulty: Rows={rows}, Columns={columns}");
    }

    // TODO make lighthouse number not go down when not over tile

    private void GetTileOnClick()
    {
        // Get the position of the mouse in world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Ensure the z-coordinate is 0 for 2D

        // Convert the world position to a cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // Get the tile at the clicked cell position
        MinesweeperTile clickedTile = tilemap.GetTile(cellPosition) as MinesweeperTile;

        if (clickedTile == null)
        {
            return;
        }

        if (clickedTile.sharkMarker)
        {
            Destroy(clickedTile.sharkMarker);
            clickedTile.sharkMarker = null;

        }
        else if (!clickedTile.isRevealed)
        {
            RevealTile(clickedTile, true);
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
        PopulateTileContents(TileContent.Boat, GameManager.numBoatGoal);
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
                int dangerLevel = GetTileDangerLevelByCoord(x, y);
                tile.dangerLevel = dangerLevel;
                UpdateTileSprite(tile);
            }
        }
    }

    private int GetTileDangerLevelByCoord(int tileXCoord, int tileYCoord)
    {
        int tileDangerLevel = 0;

        List<MinesweeperTile> tileNeighbours = GetTileNeighboursByCoord(tileXCoord, tileYCoord);

        foreach (MinesweeperTile neighbouringTile in tileNeighbours)
        {
            if (neighbouringTile.tileContent == TileContent.Shark)
            {
                tileDangerLevel++;
            }
        }

        return tileDangerLevel;
    }

    private List<MinesweeperTile> GetTileNeighboursByCoord(int tileXCoord, int tileYCoord)
    {
        Vector3Int ogPosition = new Vector3Int(tileXCoord, tileYCoord, 0);

        List<Vector3Int> offsets = new()
        {
            new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0), // top row
            new Vector3Int( -1, 0, 0),                           new Vector3Int( 1, 0, 0), // middle row
            new Vector3Int( -1, -1, 0), new Vector3Int( 0, -1, 0), new Vector3Int( 1, -1, 0)  // bottom row
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
        if (tile.isRevealed)
        {
            return;
        }

        // visibly reveal Tile
        tile.isRevealed = true;

        // Tile click sound effect
        if (wasClicked)
        {
            Camera.main.GetComponent<AudioManager>().PlaySound("tile-click");
        }

        // update tile sprite
        UpdateTileSprite(tile);

        // call functions based on if shark, treasure or boat revealed
        switch (tile.tileContent)
        {
            case TileContent.Empty:

               if (propagateTiles && tile.dangerLevel == 0)
                {
                    Vector3Int tileCoords = GetCoordsByTile(tile);

                    foreach (MinesweeperTile neighbourTile in GetTileNeighboursByCoord(tileCoords.x, tileCoords.y))
                    {
                        if (!neighbourTile.isRevealed && neighbourTile.tileContent != TileContent.Boat)
                        {
                            StartCoroutine(CallRevealTileAfterDelay(neighbourTile, false));
                        }
                    }
                }
                break;
            case TileContent.Boat:
                OnBoatOrTreasureRevealed(tile, BoatPrefab);
                Camera.main.GetComponent<AudioManager>().PlaySound("BoatReveal - FirstBoat");
                GameManager.numBoatsCollected++;
                break;
            case TileContent.Shark:
                if (wasClicked)
                {
                    // to do: add animation
                    Camera.main.GetComponent<AudioManager>().PlaySound("SharkReveal - Bad");
                    GameManager.numLives -= 1;
                    GameObject.FindGameObjectWithTag("HeartText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLives.ToString();

                }
                else
                {
                    Camera.main.GetComponent<AudioManager>().PlaySound("SharkReveal - Lighthouse");
                }
                break;
            case TileContent.TreasureSmall:
            case TileContent.TreasureMedium:
            case TileContent.TreasureLarge:
                OnBoatOrTreasureRevealed(tile, TreasurePrefab);
                Camera.main.GetComponent<AudioManager>().PlaySound("TreasureReveal");


                if (propagateTiles && tile.dangerLevel == 0)
                {
                    Vector3Int tileCoords = GetCoordsByTile(tile);

                    foreach (MinesweeperTile neighbourTile in GetTileNeighboursByCoord(tileCoords.x, tileCoords.y))
                    {
                        if (!neighbourTile.isRevealed && neighbourTile.tileContent != TileContent.Boat)
                        {
                            StartCoroutine(CallRevealTileAfterDelay(neighbourTile, false));
                        }
                    }
                }
                GameManager.numCoins += treasureValues[tile.tileContent];
                GameObject.FindGameObjectWithTag("NumCoinsText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numCoins.ToString();
                // to do: add animation
                tile.tileContent = TileContent.Empty;
                StartCoroutine(CallUpdateTileSpriteAfterDelay(tile));
                break;
            // if Lighthouse or Shore, revealing it doesn't do anything
            default:
                break;
        }
    }

    public void PlaceLighthouse(MinesweeperTile tile, LightHouseType lighthouseType)
    {
        if (tile.tileContent == TileContent.Shore)
        {
            return;
        }

        // first update tile content (prevents shark penalty)
        Vector3Int lighthouseTileCoords = GetCoordsByTile(tile);

        TileContent oldTileContent = tile.tileContent;
        tile.dangerLevel = 0;
        tile.tileContent = TileContent.Lighthouse;


        switch (oldTileContent)
        {
            case TileContent.Shark:
                // if there was a shark, update neighbouring tile danger levels
                List<MinesweeperTile> neighbours = GetTileNeighboursByCoord(lighthouseTileCoords.x, lighthouseTileCoords.y);
                foreach (MinesweeperTile neighbourTile in neighbours)
                {
                    Vector3Int neighbourTileCoords = GetCoordsByTile(neighbourTile);
                    neighbourTile.dangerLevel = GetTileDangerLevelByCoord(neighbourTileCoords.x, neighbourTileCoords.y);
                    UpdateTileSprite(neighbourTile);
                }
                break;
            case TileContent.Boat:
                Debug.Log("Lighthouse placed ON boat");

                if (TryMoveBoatToHiddenAndEmptySquare(tile))
                    break;

                GameManager.numBoatsCollected++;
                OnBoatOrTreasureRevealed(tile, BoatPrefab);
                break;
            case TileContent.TreasureSmall:
            case TileContent.TreasureMedium:
            case TileContent.TreasureLarge:
                OnBoatOrTreasureRevealed(tile, TreasurePrefab);
                GameManager.numCoins += treasureValues[oldTileContent];
                break;
        }

        Camera.main.GetComponent<AudioManager>().PlaySound("lighthouse-placing");
        // reveal the tile and update the sprite
        RevealTile(tile, false);

        // then reveal all other applicable tiles
        foreach (MinesweeperTile revealTile in GetLighthouseRevealTiles(lighthouseType, lighthouseTileCoords.x, lighthouseTileCoords.y))
        {
            RevealTile(revealTile, false);
        }
    }

    private bool TryMoveBoatToHiddenAndEmptySquare(MinesweeperTile OldTile)
    {

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var currentTile = tilemap.GetTile(new Vector3Int(i,j,0)) as MinesweeperTile;

                if (currentTile == null)
                    continue;

                if (!currentTile.isRevealed && currentTile.tileContent == TileContent.Empty)
                {
                    currentTile.tileContent = TileContent.Boat;
                    OldTile.tileContent = TileContent.Lighthouse;
                    Debug.Log("BOAT MOVED!");
                    tilemap.RefreshTile(GetCoordsByTile(currentTile));
                    tilemap.RefreshTile(GetCoordsByTile(OldTile));

                    tilemap.SetTile(new Vector3Int(i, j, 0), currentTile);
                    shadowTileMap.SetTile(new Vector3Int(i, j, 0), Instantiate<MinesweeperTile>(boardTileHolder.GetBoat()));
                    return true;
                }
            }
        }

        Debug.Log("SORRY FAILED TO MOVE BOAT");

        return false;
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
        if (tile.tileContent == TileContent.Shore)
            return;

        if (!tile.isRevealed)
        {
            tile.m_AnimatedSprites = Object.Instantiate<MinesweeperTile>(boardTileHolder.GetBlackTile()).m_AnimatedSprites;
            tilemap.RefreshTile(GetCoordsByTile(tile));
            return;
        }

        if (tile.dangerLevel == 0)
        {
            tile.m_AnimatedSprites = boardTileHolder.GetWaterShadeTile().m_AnimatedSprites;
        }
        if (tile.dangerLevel == 1)
        {
            tile.m_AnimatedSprites = boardTileHolder.GetWaterMediumTile().m_AnimatedSprites;
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

        if (tile.tileContent == TileContent.TreasureSmall || tile.tileContent == TileContent.TreasureMedium || tile.tileContent == TileContent.TreasureLarge)
            tile.m_AnimatedSprites = boardTileHolder.GetCoin().m_AnimatedSprites;

        UpdateSpriteLayers(tile);
        UpdateTileDangerNumber(tile);
        Destroy(tile.sharkMarker);
        tilemap.RefreshTile(GetCoordsByTile(tile));
    }

    // used for treasure disappearing after reveal
    private IEnumerator CallUpdateTileSpriteAfterDelay(MinesweeperTile tile)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(1.25f);

        // Call the method
        UpdateTileSprite(tile);
    }

    // used to make "Fast mode" slower ahahahah
    private IEnumerator CallRevealTileAfterDelay(MinesweeperTile tile, bool wasClicked)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(0.15f);

        // Call the method
        RevealTile(tile, wasClicked);
    }

    private void UpdateSpriteLayers(MinesweeperTile tile)
    {
        var item = tile.tileContent;
        var position = GetCoordsByTile(tile);

        if (item == TileContent.Shark)
        {
            tile.m_MaxSpeed = 8;
            tile.m_MinSpeed = 8;
            var shadowTile = Instantiate<MinesweeperTile>(boardTileHolder.GetSharkShadow());
            shadowTile.m_MaxSpeed = 6;
            shadowTile.m_MinSpeed = 6;
            shadowTileMap.SetTile(position, shadowTile);
            BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterDarkestTile()));
        }
        else if (item == TileContent.Boat)
        {
            tile.m_MaxSpeed = 6;
            tile.m_MinSpeed = 6;
            shadowTileMap.SetTile(position, null);
            var danger = GetTileDangerLevelByCoord(position.x, position.y);
            if (danger == 0)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterShadeTile()));
            if (danger == 1 || danger == 2)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterMediumTile()));
            if (danger == 3)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterDarkerTile()));
            if (danger >= 4)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterDarkestTile()));
            // TODO Make sure boat water levels reflect danger levels if we agree it looks nice
        }else if (item == TileContent.Lighthouse)
        {
            BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterShadeTile()));
        }else if (item == TileContent.TreasureSmall || item == TileContent.TreasureMedium || item == TileContent.TreasureLarge)
        {
            shadowTileMap.SetTile(position, null);

            var danger = GetTileDangerLevelByCoord(position.x, position.y);
            if (danger == 0)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterShadeTile()));
            if (danger == 1 || danger == 2)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterMediumTile()));
            if (danger == 3)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterDarkerTile()));
            if (danger >= 4)
                BelowTileMap.SetTile(position, Instantiate<MinesweeperTile>(boardTileHolder.GetWaterDarkestTile()));
        }
        else
        {
            tile.m_MaxSpeed = 2;
            tile.m_MinSpeed = 2;
            shadowTileMap.SetTile(position, null);
            BelowTileMap.SetTile(position, null);
        }

    }

    private void UpdateTileDangerNumber(MinesweeperTile tile)
    {
        // get danger level and position
        int dangerNumber = tile.dangerLevel;

        if (tile.tileContent == TileContent.Shark || tile.tileContent == TileContent.Boat || tile.tileContent == TileContent.Lighthouse || dangerNumber == 0)
        {
            return;
        }

        Vector3Int position = GetCoordsByTile(tile);
        Vector3 worldPosition = DangerNumbersTileMap.CellToWorld(position);

        // destroy any preexisting text on the tile
        if (tile.dangerNumberText != null)
        {
            Destroy(tile.dangerNumberText);
        }

        // create new text at correct location
        GameObject dangerNumberObject = Instantiate(DangerNumberText, worldPosition, Quaternion.identity);
        dangerNumberObject.transform.SetParent(DangerNumbersTileMap.transform);
        dangerNumberObject.transform.position += new Vector3(0.75f, 0.30f, 0);

        // set correct text
        TextMeshPro textMeshPro = dangerNumberObject.GetComponent<TextMeshPro>();

        if (textMeshPro != null)
        {
            // Set the text to the danger level
            textMeshPro.text = dangerNumber.ToString();
        }

        // link the text to the tile
        tile.dangerNumberText = dangerNumberObject;
    }

    private void OnBoatOrTreasureRevealed(MinesweeperTile tile, GameObject itemPrefab)
    {
        // to do: uncomment if we want this
        //Vector3Int cellPosition = GetCoordsByTile(tile);
        //Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
        //Instantiate(itemPrefab, worldPosition, Quaternion.identity);
    }
}