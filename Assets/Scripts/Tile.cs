using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isRevealed
    {
        get
        {
            return isRevealed;
        }
        set
        {
            isRevealed = value;
            ChangeSprite();
        }
    }

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

    public Sprite _dangerLevelSprite;
    public Sprite _tileContentSprite;
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


    private void ChangeSprite()
    {
        throw new NotImplementedException();
    }
}
