using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Store_Old : UI_Base
{
    private List<Item> itemList;
    private GameObject itemInfoUI;
    private Text itemNameText;
    private Text itemPriceText;
    private Text itemStockText;
    
    public override void Init()
    {
    }


    private void BindEvent()
    {
        for (var i = 0; i < itemList.Count; i++)
        {
            BindEvent(Get<GameObject>(i), (PointerEventData eventData) => { OnClickItem(i); }, MouseUIEvent.Click);
        }
    }
    
    private void OnClickItem(int index)
    {
        if (itemInfoUI == null)
        {
            itemInfoUI = Instantiate(Resources.Load<GameObject>("UI_ItemInfo"), transform.parent);
            itemNameText = Get<Text>(0);
            itemPriceText = Get<Text>(1);
            itemStockText = Get<Text>(2);
            BindEvent(itemInfoUI, (PointerEventData eventData) => { itemInfoUI.SetActive(false); }, MouseUIEvent.Click);
        }

      //  itemNameText.text = itemList[index].name;
      //  itemPriceText.text = itemList[index].buyPrice.ToString();
        //itemStockText.text = StoreManager.Instance.GetStockValue(index).ToString();

        itemInfoUI.SetActive(true);
    }
}


