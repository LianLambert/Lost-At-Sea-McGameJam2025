using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isRevealed;

    public int dangerLevel
    {
        get
        {
            return dangerLevel;
        }

        set
        {
            dangerLevel = value;
            ChangeSprite();
        }
    }

    public TileContent tileContent
    {
        get
        {
            return tileContent;
        }
        set
        {
            tileContent = value;
            ChangeSprite();
        }
    }

    public Sprite dangerLevelSprite;
    public Sprite tileContentSprite;
    public SpriteRenderer dangerLevelRenderer;
    public SpriteRenderer tileContentRenderer;

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

    // If isClicked true, then player clicked on the tile, else tile was revealed by lighthouse.
    public void RevealTile(bool isClicked)
    {
        isRevealed = true;
        ChangeSprite();
        switch (tileContent) {
            case TileContent.Shark:
                SharkBite();
                break;
            case TileContent.TreasureSmall:
                GameManager.coins += 10;
                treasureObtained();
                break;
            case TileContent.TreasureMedium:
                GameManager.coins += 20;
                treasureObtained();
                break;
            case TileContent.TreasureLarge:
                GameManager.coins += 50;
                treasureObtained();
                break;
        }
    }

    private void treasureObtained()
    {
        
    }

    // Player loses a life from revealing a shark
    private void SharkBite()
    {
        GameManager.lifePoints--;
    }

    private void ChangeSprite()
    {
        ChangeSpriteWater();
    }

    private void ChangeSpriteWater()
    {
        if (isRevealed)
        {
            // Clear water
            if (dangerLevel == 0)
            {
                dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_0");
            }
            // 1-2 Sharks
            else if (dangerLevel > 0 && dangerLevel < 3) 
            {
                dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_1");
            }
            // 3-4 sharkss
            else if (dangerLevel > 2 && dangerLevel < 5)
            {
                dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_2");
            }
            // 5+ sharks
            else
            {
                dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_3");
            }
        }
        else
        {
            dangerLevelRenderer.sprite = Resources.Load<Sprite>("Environnement/Prefab/shade_4");
        }
    }

}
