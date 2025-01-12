using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CustomTileObject", order = 1)]
public class MinesweeperTile : AnimatedTile
{
    public bool isRevealed;

    private int _dangerLevel;
    public int dangerLevel
    {
        get
        {
            return _dangerLevel;
        }

        set
        {
            _dangerLevel = value;
            //ChangeSpriteWater();
        }
    }

    private TileContent _tileContent;
    public TileContent tileContent
    {
        get
        {
            return _tileContent;
        }
        set
        {
            _tileContent = value;
            //ChangeSpriteTileContent();
        }
    }

    // public SpriteRenderer dangerLevelRenderer;
    //public SpriteRenderer tileContentRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Initialize(bool isRevealed, int dangerLevel, TileContent tileContent)
    {
        this.isRevealed = isRevealed;
        this.dangerLevel = dangerLevel;
        this.tileContent = tileContent;
    }

    //// If isClicked true, then player clicked on the tile, else tile was revealed by lighthouse.
    //public void RevealTile(bool isClicked)
    //{
    //    isRevealed = true;
    //    GameManager.numTilesRevealed++;

    //    ChangeSpriteWater();
    //    ChangeSpriteTileContent();

    //    switch (tileContent) {
    //        case TileContent.Shark:
    //            SharkBite();
    //            break;
    //        case TileContent.TreasureSmall:
    //            GameManager.numCoins += 10;
    //            treasureObtained();
    //            break;
    //        case TileContent.TreasureMedium:
    //            GameManager.numCoins += 20;
    //            treasureObtained();
    //            break;
    //        case TileContent.TreasureLarge:
    //            GameManager.numCoins += 50;
    //            treasureObtained();
    //            break;
    //        case TileContent.Boat:
    //            GameManager.numBoats++;
    //            break;
    //    }
    //}

    //private void treasureObtained()
    //{
        
    //}

    //// Player loses a life from revealing a shark
    //private void SharkBite()
    //{
    //    GameManager.numLives--;
    //}

    //private void ChangeSpriteWater()
    //{
    //    // Clear water
    //    if (dangerLevel == 0)
    //    {
    //        dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_0");
    //    }
    //    // 1-2 Sharks
    //    else if (dangerLevel > 0 && dangerLevel < 3) 
    //    {
    //        dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_1");
    //    }
    //    // 3-4 sharkss
    //    else if (dangerLevel > 2 && dangerLevel < 5)
    //    {
    //        dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_2");
    //    }
    //    // 5+ sharks
    //    else
    //    {
    //        dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_3");
    //    }
    //}

    //private void ChangeSpriteTileContent()
    //{
    //    if (isRevealed)
    //    {

    //    }
    //    else
    //    {
    //        tileContentRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_4");
    //    }
    //}

}
