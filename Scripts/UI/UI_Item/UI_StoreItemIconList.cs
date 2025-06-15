using UnityEngine;
using UnityEngine.UI;
using Utils;
using TMPro;

public class UI_StoreItemIconList : UI_ItemIconList
{
    private enum Texts
    {
        ItemDisplayText,
        ItemDisplayStockText,
        ItemDisplayCostText
    }
    
    public override void Init()
    {
        base.Init();
        
        Bind<TextMeshProUGUI>(typeof(Texts)); 
        ItemIconButton.onClick.AddListener(() => Managers.UIManager.ShowItemMenuButtonUI(Managers.Inventory.RequestPlayer, 
            Item, 
            UI_ItemMenuButton.ItemMenuType.Shopping, 
            transform.position));
    }


    protected sealed override void SetItemInfo()
    {
        SetItemName(Item.ItemName);
        SetItemStock(Item.ItemStock);
        SetItemCost(Item.BuyPrice);
    }

    protected override void SetItemHighLight()
    {
        bool isShow = Get<Image>((int)Images.HighLight).enabled = !Get<Image>((int)Images.HighLight).enabled;
        Managers.UIManager.ShowMainItemCardUI(Item, UI_PlayerEquippedItemCard.EquippedTarget.EquipShopTarget, isShow);
    }

    protected sealed override void SetItemName(string itemName)
    {
        Get<TextMeshProUGUI>((int)Texts.ItemDisplayText).text = itemName;
    }

    protected sealed override void SetItemStock(int itemStock)
    {
        if (itemStock <= 0) ItemIconButton.interactable = false;
        
        Get<TextMeshProUGUI>((int)Texts.ItemDisplayStockText).text = itemStock.ToString();
    }

    public void UpdateItemStock(int newItemStock)
    {
        SetItemStock(newItemStock);
    }

    private void SetItemCost(int itemCost)
    {
        Get<TextMeshProUGUI>((int)Texts.ItemDisplayCostText).text = itemCost.ToString();
    }
}
