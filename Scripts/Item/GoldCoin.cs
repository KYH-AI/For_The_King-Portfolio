public class GoldCoin : Item
{
    public GoldCoin(int itemID, string itemName, int itemGrade, int goldStock, string itemDescription) : base(itemID, itemName, itemGrade, goldStock, itemDescription)
    {
        this.Itemtype = ItemType.GoldCoin;
    }
}
