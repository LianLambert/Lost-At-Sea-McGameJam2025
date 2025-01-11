using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isRevealed;
    public int dangerLevel;
    public TileContent tileContent;
    [SerializeField] Sprite[] dangerLevelSpriteArray;
    [SerializeField] Sprite tileContentSprite;
    public SpriteRenderer dangerLevelRenderer;
    public SpriteRenderer tileContentRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    public Tile(bool isRevealed, int dangerLevel, TileContent tileContent){
        this.isRevealed = isRevealed;
        this.dangerLevel = dangerLevel;
        this.tileContent = tileContent;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        
    }
}
