using System;

public class Consumable : Item
{
    #region 변수선언부

    public enum StatType
    {
        //체력, 등등
        None = -1,
        
        Health,
    }
    public StatType Stattype { get; private set; }
    
    public int StatResponse { get; private set; }


    #endregion

    #region 생성자
    public Consumable(int itemID, string itemName, int itemGrade, int buyPrice, int sellPrice, string itemDescription, StatType statType, int statResponse, string itemCreatedAt, string itemUpdatedAt)
        : base(itemID, itemName, itemGrade, buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        /*
         * 소모품 생성자 (골드도 포함) (feat. 강진민)
         */
        
        Stattype = statType;
        StatResponse = statResponse;

        // 골드코인 판단 유무
        this.Itemtype = buyPrice == -1 ? ItemType.GoldCoin : ItemType.Consumable;
    }

    #endregion
}

