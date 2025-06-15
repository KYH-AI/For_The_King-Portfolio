using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;

public class UI_InventoryItemIconList : UI_ItemIconList
{
    private enum Texts
    {
        ItemDisplayText 
    }
    
    public override void Init()
    {
        base.Init();
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        ItemIconButton.onClick.AddListener(() => Managers.UIManager.ShowItemMenuButtonUI(Managers.Inventory.RequestPlayer,
            Item, 
            Managers.Inventory.InventoryItemMenuType, 
            transform.position));
    }

    protected sealed override void SetItemInfo()
    {
        SetItemName(Item.ItemName);
        SetItemStock(Item.ItemStock);
    }
    
    protected override void SetItemHighLight()
    {
        bool isShow = Get<Image>((int)Images.HighLight).enabled = !Get<Image>((int)Images.HighLight).enabled;
        Managers.UIManager.ShowMainItemCardUI(Item, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, isShow);
    }

    protected sealed override void SetItemName(string itemName)
    {
        Get<TextMeshProUGUI>((int)Texts.ItemDisplayText).text = itemName;
    }
    
    protected sealed override void SetItemStock(int itemStock)
    {
        Get<TextMeshProUGUI>((int)Texts.ItemDisplayText).text += " (" + itemStock + ")";
    }
}
