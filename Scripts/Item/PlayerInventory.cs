using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{

    public PlayerStats Owner { get; private set; }

    
    #region �÷��̾� ������ ���� ��Ʈ

    private Dictionary<Equipment.EquipmentSlotType, Equipment> _playerEquipmentSlots = new Dictionary<Equipment.EquipmentSlotType, Equipment>();

    #endregion

    
    #region �÷��̾� ������ �κ��丮

    // ������ Ÿ�� �������� ������ ����Ʈ�� �̿��ؼ� ����
    // (������ ID�� DB���� int���� �ƴ� string���� ����� �� ������)
    private Dictionary<Item.ItemType, Dictionary<string, Item>> _playerItemDictionary;

    #endregion

    
    // ���� ��� �ʱ�ȭ �Լ�
    public PlayerInventory(PlayerStats owner, Weapon weapon = null, Armor shield = null, 
                            Armor armor = null, Armor helmet = null, Armor boots = null, 
                            Accessory amulet = null, Accessory ring = null)
    {
        // ������ �ʱ�ȭ
        this.Owner = owner;
        
        /* 1. ����
        * 2. ����
        * 3. ��
        * 4. ���
        * 5. �Ź�
        * 6. ����
        * 7. �����
        */
        

        /* �κ��丮 ��ųʸ� �ʱ�ȭ */
        _playerItemDictionary = new Dictionary<Item.ItemType, Dictionary<string, Item>>
        {
            [Item.ItemType.Weapon] = new Dictionary<string, Item>(),
            [Item.ItemType.Armor] = new Dictionary<string, Item>(),
            [Item.ItemType.Accessory] = new Dictionary<string, Item>(),
            [Item.ItemType.Consumable] = new Dictionary<string, Item>(),
            [Item.ItemType.QuestItem] = new Dictionary<string, Item>()
        };
        


        /* ��� ���� �ʱ�ȭ */
        _playerEquipmentSlots = new Dictionary<Equipment.EquipmentSlotType, Equipment>()
        {
            [Equipment.EquipmentSlotType.PrimaryWeapon] = weapon,
            [Equipment.EquipmentSlotType.Shield] = shield,
            [Equipment.EquipmentSlotType.Clothes] = armor,
            [Equipment.EquipmentSlotType.Helmet] = helmet,
            [Equipment.EquipmentSlotType.Boots] = boots,
            [Equipment.EquipmentSlotType.Ring] = ring,
            [Equipment.EquipmentSlotType.Amulet] = amulet,
        };
    }
    
    #region �κ��丮 Ȯ�� �Լ�

    
    /// <summary>
    /// �÷��̾� �κ��丮�� ������ Ÿ������ Ȯ��
    /// </summary>
    /// <param name="itemType">ã�����ϴ� ������ Ÿ��</param>
    /// <returns>�ش� ������ Ÿ���� ���ִ� ��ųʸ�</returns>
    public Dictionary<string, Item> FilterItemsByType(Item.ItemType itemType)
    {
        if (_playerItemDictionary.ContainsKey(itemType))
        {
            return _playerItemDictionary[itemType];
        }

        return new Dictionary<string, Item>();
    }

    /// <summary>
    /// �÷��̾� �κ��丮�� ���͸� ���� ��� ������ Ȯ��
    /// </summary>
    /// <returns>��� �������� ���ִ� ����Ʈ ��ȯ</returns>
    public List<Item> NoneFilterItemsByType()
    {
        List<Item> allItems = new List<Item>();

        foreach (var itemDictionary in _playerItemDictionary.Values)
        {
            allItems.AddRange(itemDictionary.Values);
        }

        return allItems;
    }

    /// <summary>
    /// �÷��̾� �κ��丮�� Ư�� ������ ���翩�� Ȯ��
    /// </summary>
    /// <param name="item">ã�����ϴ� ������</param>
    /// <returns>������ ���� ����</returns>
    public bool ContainsInventoryItemCheck(Item item)
    {
        // 3���� ��츦 ����ؾ���
        
        /* 1. �ش� ������ Ÿ���� ��ųʸ��� �ִ°ܿ�
         * 2. �ش� ��ųʸ��� ã�����ϴ� �������� �����ϴ���?
         * 3. �ش� �������� ��� 1�� �̻�����?
         */
        string itemID = item.ItemID.ToString();
        
        return _playerItemDictionary.ContainsKey(item.Itemtype) &&
               _playerItemDictionary[item.Itemtype].ContainsKey(itemID) && 
               _playerItemDictionary[item.Itemtype][itemID].ItemStock > 0;
    }

    
    /// <summary>
    /// �÷��̾ �����ϰ� �ִ� ������ ���� ������ ��ȯ
    /// </summary>
    /// <param name="itemSlotType">��ü�ϰ��ڴ� ������ ����(����) Ÿ��</param>
    /// <returns>���� �������� ������ ��ȯ</returns>
    public Equipment FilterEquipmentSlotByItem(Equipment.EquipmentSlotType itemSlotType)
    {
        if (_playerEquipmentSlots.ContainsKey(itemSlotType))
        {
            // ������ü�� ������ ȹ�� �� ��ȯ
            Equipment unEquipmentItem = _playerEquipmentSlots[itemSlotType];
            return unEquipmentItem;
        }
        Debug.LogError($"{itemSlotType} ������ ������ ã�� �� �����ϴ�!");
        return null;
    }

    /// <summary>
    /// �÷��̾� ������ ���� ��ü
    /// </summary>
    /// <param name="unEquipmentItem">���� ��ü�� ������</param>
    public void UnEquipmentItemSlot(Equipment unEquipmentItem)
    {
        if (_playerEquipmentSlots.ContainsValue(unEquipmentItem))
        {
            // �ش� ������ null������ �Ҵ�
            _playerEquipmentSlots[unEquipmentItem.EquipmentSlottype] = null;

            return;
        }
        
        Debug.LogError($"{unEquipmentItem.ItemName}�� �������� �÷��̾ �������� �ʰ� �ֽ��ϴ�!");
    }
    
    /// <summary>
    /// �÷��̾� ��� ���Կ� ���� �������
    /// </summary>
    /// <param name="item">������ ���</param>
    public void SetEquipmentSlotItem(Equipment item)
    {
        _playerEquipmentSlots[item.EquipmentSlottype] = item;
    }
    
    /// <summary>
    /// �÷��̾� ��� ���Կ� ���� ��� �������� Ȯ��
    /// </summary>
    /// <param name="itemSlotType">���� ������ ���</param>
    /// <returns>�ش� ��� ���Կ� ��� ����Ȯ��</returns>
    public bool ContainsEquipmentItemCheck(Equipment itemSlotType)
    {
        Equipment equipment = _playerEquipmentSlots[itemSlotType.EquipmentSlottype];
        // �ش� ��� ���Կ� ��� ������ true, ������ false ��ȯ
        return equipment != null;
    }
    
    #endregion

}
