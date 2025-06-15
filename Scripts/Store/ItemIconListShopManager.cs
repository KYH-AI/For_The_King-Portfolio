using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconListShopManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int itemId { get; set; }
    public int price { get; set; }
    
    private Transform _highLight;
    private Transform _description;
    public Transform _CostTransform;
    public Text cost;
    
    private void Start()
    {
        _highLight = transform.Find("HighLight");
        _description = transform.Find("Description_Root");
        _CostTransform = gameObject.transform.Find("ItemDisplayCost");
        cost = _CostTransform.GetComponent<Text>();
        
    }

    private void Update()
    {
        cost.text = price.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _highLight.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highLight.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _description.gameObject.SetActive(true);
    }

    public void Bind()
    {
        
    }
}
