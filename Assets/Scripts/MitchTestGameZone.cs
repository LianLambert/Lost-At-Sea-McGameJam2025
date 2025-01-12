using UnityEngine;
using UnityEngine.Tilemaps;

public class GameZone : MonoBehaviour
{
    private Tilemap _gameZoneTilemap;
    private TilesHolder _tilesHolder;
    public int size;

    Vector3Int origin;

    private void Awake()
    {
        _gameZoneTilemap = GetComponent<Tilemap>();
        _tilesHolder = GetComponent<TilesHolder>();
    }

    private void Start()
    {
        var columns = size;
        var rows = size;

        // Origin point for the tilemap
        origin = _gameZoneTilemap.origin;

        // Clear any existing tiles
        _gameZoneTilemap.ClearAllTiles();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // Determine which tile to use based on the grid position
                int tileIndex = (x + y) % 3; // Cycles through 0, 1, 2

                Tile tile = null;
                var r = Random.Range(0, 5);


                switch (r)
                {
                    case 0:
                        tile = _tilesHolder.GetWaterShadeTile();
                        tile.isRevealed = true;
                        break;
                    case 1:
                        tile = _tilesHolder.GetWaterMediumTile();
                        tile.isRevealed = true;
                        break;
                    case 2:
                        tile = _tilesHolder.GetWaterDarkerTile();
                        tile.isRevealed = true;
                        break;
                    case 3:
                        tile = _tilesHolder.GetWaterDarkestTile();
                        tile.isRevealed = true;
                        break;
                    case 4:
                        tile = _tilesHolder.GetBlackTile();
                        tile.isRevealed = false;
                        break;
                }

                // Set the tile on the tilemap
                _gameZoneTilemap.SetTile(new Vector3Int(origin.x + x, origin.y + y, origin.z), tile);
            }
        }

        // Adjust camera to fit the tilemap
        AdjustCamera(columns, rows);

        // Compress the tilemap bounds
        _gameZoneTilemap.CompressBounds();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse button click
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
        Vector3Int cellPosition = _gameZoneTilemap.WorldToCell(worldPosition);

        // Get the tile at the clicked cell position
        Tile clickedTile = _gameZoneTilemap.GetTile(cellPosition) as Tile;

        if (clickedTile != null)
        {
            Debug.Log($"Clicked Tile: {clickedTile.name} at {cellPosition}");
        }
        else
        {
            Debug.Log("No tile at clicked position.");
        }

        if (!clickedTile.isRevealed)
        {
            Debug.Log("Revealing tile");
            var newTile = _tilesHolder.GetWaterShadeTile();
            _gameZoneTilemap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, cellPosition.z), newTile);
            //_gameZoneTilemap.RefreshAllTiles();
        }
    }
}
