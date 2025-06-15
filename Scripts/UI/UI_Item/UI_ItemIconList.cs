using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class UI_ItemIconList : UI_Base
{
    /*  < ����, �÷��̾� �κ��丮�� ǥ���� ������ UI ������Ʈ >
     * 1. ������ ������ �̹���
     * 2. ������ �̸�
     * 3. ������ HighLight
     * 4. (����, �κ��丮 ���� �ʿ� ) ������ ���
     * 5. (���� : ������ ����)
     */

    
    protected enum Images
    {
        HighLight,
        ItemIcon,
    }

    protected Button ItemIconButton;
    
    // ���� ������ ������
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

        // ������ ������ ����
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
