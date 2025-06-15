using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class UI_ItemIconList : UI_Base
{
    /*  < 상점, 플레이어 인벤토리에 표시할 아이템 UI 오브젝트 >
     * 1. 아이템 아이콘 이미지
     * 2. 아이템 이름
     * 3. 아이템 HighLight
     * 4. (상점, 인벤토리 구별 필요 ) 아이템 재고
     * 5. (상점 : 아이템 가격)
     */

    
    protected enum Images
    {
        HighLight,
        ItemIcon,
    }

    protected Button ItemIconButton;
    
    // 현재 보유한 아이템
    public Item Item { get; private set; }

    public override void Init()
    {
        ItemIconButton = GetComponent<Button>();
        Bind<Image>(typeof(Images));
        this.gameObject.BindEvent(data => { SetItemHighLight(); }, MouseUIEvent.Enter );
        this.gameObject.BindEvent(data => { SetItemHighLight(); }, MouseUIEvent.Exit );
    }

    public void SetItemInfo(Item item)
    {
        this.Item = item;

        // 아이템 아이콘 설정
        Sprite itemIcon = null;
        switch (Item.Itemtype)
        {
            case Item.ItemType.Weapon :
                Weapon weapon = (Weapon)item;
                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    weapon.Weapontype.ToString(), true);
                break;
            
            case Item.ItemType.Armor :
                Armor armor = (Armor)item;
                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    armor.Armortype.ToString(), true);
                break;
            
            case Item.ItemType.Consumable :

                break;
        }
        
        SetItemInfo();
        SetItemIconImage(itemIcon);
    }

    private void SetItemIconImage(Sprite itemIcon)
    {
        Get<Image>((int)Images.ItemIcon).sprite = itemIcon;
    }

    protected abstract void SetItemHighLight();
    
    protected abstract void SetItemInfo();

    protected abstract void SetItemName(string itemName);
    
    protected abstract void SetItemStock(int itemStock);
    
}
