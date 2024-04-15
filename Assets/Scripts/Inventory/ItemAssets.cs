using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
   // singleton

    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;


    // item Sprite Ãß°¡
    public Sprite swordSprite;
    public Sprite PickaxeSprite;
    public Sprite AxeSprite;

    public Sprite CoinSprite;

    public Sprite healthPotionSprite;
    public Sprite manaPotionSprite;
    
    public Sprite medkitSprite;
    
    public Sprite GrassSprite;
    public Sprite DirtSprite;
    public Sprite MarsSprite;
    public Sprite TechSprite;
    public Sprite StoneSprite;
    public Sprite LogSprite;
    public Sprite MushroomSprite;
    public Sprite LeafSprite;

    public Sprite CoalSprite;
    public Sprite IronSprite;
    public Sprite GoldSprite;
    public Sprite DiamondSprite;

   
   
}
