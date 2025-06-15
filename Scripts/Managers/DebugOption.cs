using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugOption : MonoBehaviour
{
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_Dropdown playerDropdown;
    [SerializeField] private TMP_Dropdown itemTypeDropdown;
    [SerializeField] private TMP_Dropdown itemDropdown;

    private int _playerIndex, _itemTypeIndex, _itemIndex;
    
    public void Init()
    {
        playerDropdown.onValueChanged.AddListener(playerIndex => { _playerIndex = playerIndex; });
        itemTypeDropdown.onValueChanged.AddListener(OnChangeItemDropDownValues);
        itemDropdown.onValueChanged.AddListener(itemIndex => { _itemIndex = itemIndex; });
        sendButton.onClick.AddListener(() => SendItemButton(_playerIndex, _itemTypeIndex, _itemIndex));

        playerDropdown.ClearOptions();
        itemTypeDropdown.ClearOptions();
        itemDropdown.ClearOptions();

        List<string> playerList = new List<string>();

        for (int i = 0; i < Managers.Instance.GetPlayerCount; i++)
        {
            playerList.Add(Managers.Instance.GetPlayer(i).PlayerStats.BaseClassName);
        }
        
        playerDropdown.AddOptions(playerList);


        List<string> itemTypeList = new List<string>();
        
        for (int i = 0; i < Enum.GetValues(typeof(Item.ItemType)).Length; i++)
        {
            itemTypeList.Add(((Item.ItemType)i).ToString());
        }
        
        itemTypeDropdown.AddOptions(itemTypeList);

        playerDropdown.value = 0;
        itemTypeDropdown.value = 0;
        itemDropdown.value = 0;

    }
    
    private void OnChangeItemDropDownValues(int itemTypeIndex)
    {
        itemDropdown.ClearOptions();
        
        _itemTypeIndex = itemTypeIndex;
        Item.ItemType selectedType = (Item.ItemType)itemTypeIndex;
        var itemInfo = Managers.Data.ItemInfos(selectedType);

        List<string> itemList = new List<string>();

        foreach (var item in itemInfo.Values)
        {
            itemList.Add(item.ItemName);
        }
        
        itemDropdown.AddOptions(itemList);
    }
    
    private void SendItemButton(int playerIndex, int itemTypeIndex ,int itemIndex)
    {
        var item = Managers.Data.GetItemInfo((Item.ItemType)itemTypeIndex, itemIndex);
        Managers.Inventory.AddInventoryItem(Managers.Instance.GetPlayer(playerIndex).PlayerStats, item);
    }
    
}
