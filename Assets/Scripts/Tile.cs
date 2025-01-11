using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isRevealed;
    public int dangerLevel;
    public TileContent tileContent;
    [SerializeField] Sprite currentSprite;
    [SerializeField] SpriteRenderer currentRenderer;
    [SerializeField] Animation tileAnimation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
