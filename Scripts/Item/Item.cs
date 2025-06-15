using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class Item : ICloneable
{
    /*
     *  아이템 틀
     */

    public enum ItemType
    {
        NONE =-1,
        Weapon = 0,
        Armor = 1,
        Consumable = 2,
        Accessory = 3,
        QuestItem = 4,
        GoldCoin = 5,
    }

    public enum Grade
    {
        //일반(0) 고급(1) 희귀(2) 영웅(3) 전설(4) 신화(5) 퀘스트(6)
        NONE = -1,
        COMMON = 0,
        ADVANCED = 1,
        RARE = 2,
        HERO = 3,
        LEGEND = 4,
        MYTH = 5,
        QUEST = 6,
    }

    public ItemType Itemtype;

    public Grade ItemGrade { get; private set; }

    public int ItemID { get; private set; }

    public string ItemName { get; private set; }

    public int ItemStock;

    private int _buyPrice;

    public int BuyPrice
    {
        get => _buyPrice;
        private set
        {
            if (value < 0)
                return;
            _buyPrice = value;
        }
    }

    private int _sellPrice;

    public int SellPrice
    {
        get => _sellPrice;
        set
        {
            if (value < 0)
                return;
            _sellPrice = value;
        }
    }

    public string ItemDescription { get; private set; }

    public string ItemCreatedAt { get; private set; }

    public string ItemUpdatedAt { get; private set; }


    // 골드코인을 제외한 모든 아이템 생성자
    public Item(int itemID, string itemName, int itemGrade, int buyPrice, int sellPrice, string itemDescription, string itemCreatedAt, string itemUpdatedAt)
    {
        this.ItemID = itemID;
        this.ItemName = itemName;
        this.ItemGrade = (Grade)itemGrade;
        this.BuyPrice = buyPrice;
        this.SellPrice = sellPrice;
        this.ItemDescription = itemDescription;
        this.ItemCreatedAt = itemCreatedAt;
        this.ItemUpdatedAt = itemUpdatedAt;
        this.ItemStock = 1;
    }

    // 골드코인 전용 아이템 생성자
    protected Item(int itemID, string itemName, int itemGrade, int goldCoinStock, string itemDescription)
    {
        this.ItemID = itemID;
        this.ItemName = itemName;
        this.ItemGrade = (Grade)itemGrade;
        this.ItemStock = goldCoinStock;
        this.ItemDescription = itemDescription;
    }

    public virtual object Clone()
    {
        return MemberwiseClone();
    }
}
    
