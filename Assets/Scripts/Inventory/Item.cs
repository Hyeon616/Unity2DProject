using System;
using UnityEngine;


[Serializable]
public class Item
{

    public enum ItemType
    {
        //무기
        Sword,
        Pickaxe,
        Axe,

        //포션
        HealthPotion,
        ManaPotion,

        //돈
        Coin,

        //약
        Medkit,

        //블럭
        Grass,
        Dirt,
        Mars,
        Tech,
        Stone,
        Log,
        Mushroom,
        Leaf,

        // 자원
        Coal,
        Iron,
        Gold,
        Diamond,

    }

    public ItemType itemType;
    public int amount;

    // 아이템 추가
    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Sword:
                return ItemAssets.Instance.swordSprite;
            case ItemType.Pickaxe:
                return ItemAssets.Instance.PickaxeSprite;
            case ItemType.Axe:
                return ItemAssets.Instance.AxeSprite;
            case ItemType.HealthPotion:
                return ItemAssets.Instance.healthPotionSprite;
            case ItemType.ManaPotion:
                return ItemAssets.Instance.manaPotionSprite;
            case ItemType.Coin:
                return ItemAssets.Instance.CoinSprite;
            case ItemType.Medkit:
                return ItemAssets.Instance.medkitSprite;
            case ItemType.Grass:
                return ItemAssets.Instance.GrassSprite;
            case ItemType.Dirt:
                return ItemAssets.Instance.DirtSprite;
            case ItemType.Mars:
                return ItemAssets.Instance.MarsSprite;
            case ItemType.Tech:
                return ItemAssets.Instance.TechSprite;
            case ItemType.Stone:
                return ItemAssets.Instance.StoneSprite;
            case ItemType.Log:
                return ItemAssets.Instance.LogSprite;
            case ItemType.Mushroom:
                return ItemAssets.Instance.MushroomSprite;
            case ItemType.Leaf:
                return ItemAssets.Instance.LeafSprite;
            case ItemType.Coal:
                return ItemAssets.Instance.CoalSprite;
            case ItemType.Iron:
                return ItemAssets.Instance.IronSprite;
            case ItemType.Gold:
                return ItemAssets.Instance.GoldSprite;
            case ItemType.Diamond:
                return ItemAssets.Instance.DiamondSprite;
           
        }

    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.HealthPotion:
            case ItemType.ManaPotion:
            case ItemType.Coin:
            case ItemType.Medkit:
            case ItemType.Grass:
            case ItemType.Dirt:
            case ItemType.Mars:
            case ItemType.Tech:
            case ItemType.Stone:
            case ItemType.Log:
            case ItemType.Mushroom:
            case ItemType.Leaf:
            case ItemType.Coal:
            case ItemType.Iron:
            case ItemType.Gold:
            case ItemType.Diamond:
                return true;
            case ItemType.Sword:
            case ItemType.Pickaxe:
            case ItemType.Axe:
                return false;
                
            
        }

    }


}
