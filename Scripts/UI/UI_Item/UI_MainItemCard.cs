using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Object = System.Object;

public class UI_MainItemCard : UI_ItemCard
{

    // 아이템 재고 (그냥 재화 표시)
    private TextMeshProUGUI _itemStockText;

    // 회전 연출용 코루틴
    private Coroutine _rotationItemCoroutine;

    // 3D 아이템 연출 캐싱 (static : 상점을 닫을 경우 연출에 사용된 3D 아이템을 모두 초기화)
    public static readonly  Dictionary<string, GameObject> ItemObjectDictionary = new Dictionary<string, GameObject>();

    // 마지막으로 연출된 3D 아이템 이름
    private string lastItem3dName = string.Empty;
    
    // 3D 아이템 연출 위치보정 ([0] = 좌표, [1] = 회전)
    private readonly Dictionary<Weapon.WeaponType, Vector3[]> _WEAPON_ITEM_RENDERER_POSITION = new Dictionary<Weapon.WeaponType, Vector3[]>()
    {
        { Weapon.WeaponType.WeaponSword, new Vector3[] {new Vector3(-0.24f,0.43f,0), new Vector3(31.6f,39.4f,327.4f)} },
        { Weapon.WeaponType.WeaponBow, new Vector3[] {new Vector3(0.01f,0.8f,0), new Vector3(31.6f,39.4f,327.5f)} },
        { Weapon.WeaponType.WeaponStaff, new Vector3[] {new Vector3(-0.06f,0.65f,0), new Vector3(31.6f,39.4f,327.4f)} },
    };
    
    public override void Init()
    {
        _itemStockText = ObjectUtil.FindChild<TextMeshProUGUI>(gameObject, "ItemStockText", true);
        ItemObjectDictionary.Clear();
        base.Init();
    }

    protected override void SetItemInfo(bool isRewardItem)
    {
        // 현재 아이템이 골드인 경우 골드 개수를 표시
        if (Item.Itemtype is Item.ItemType.GoldCoin)
        {
            _itemStockText.text = "x"+Item.ItemStock;
            _itemStockText.gameObject.SetActive(true);
        }

        SetItemRendererTexture(isRewardItem);
    }

    /// <summary>
    /// 아이템 3D 오브젝트 렌더링 
    /// </summary>
    private void SetItemRendererTexture(bool isRewardItem)
    {
        Vector3[] itemRendererOffSetPos = null;

        // 마지막으로 연출된 아이템 비활성화
        if (!string.IsNullOrEmpty(lastItem3dName))
        {
            if (ItemObjectDictionary.TryGetValue(lastItem3dName, out GameObject lastItemObject) && lastItemObject.activeSelf)
            {
                lastItemObject.SetActive(false);
            }
        }
        
        // 아이템 랜더링 대상이 무기인 경우 연출좌표 보정
        if (Item.Itemtype is Item.ItemType.Weapon)
        {
            Weapon weapon = (Weapon)Item;
            itemRendererOffSetPos = _WEAPON_ITEM_RENDERER_POSITION[weapon.Weapontype];
        }
        
        // 저장된 3D 아이템 오브젝트가 없는경우 새로생성 후 저장
        if (!ItemObjectDictionary.ContainsKey(Item.ItemName))
        {
            
            ItemObjectDictionary.Add(Item.ItemName, 
                            Instantiate(Managers.Resource.LoadResource<GameObject>(Managers.Resource.GetItemPrefabPath(Item.Itemtype),
                            Item.ItemID + "_" + Managers.Data.GetItemName(Item.Itemtype, Item.ItemID)),
                                     Managers.UIManager.UIItemObjectRendererTexturePos));
        }

        GameObject item3D = ItemObjectDictionary[lastItem3dName = Item.ItemName];

        // 아이템 연출에 좌표 보정이 필요한 경우 적용
        if (itemRendererOffSetPos != null)
        {
            item3D.transform.localPosition = itemRendererOffSetPos[0];
            item3D.transform.localEulerAngles = itemRendererOffSetPos[1];
        }
        
         item3D.SetActive(true);
        
        // 보상 아이템인 경우 360도 회전연출 필요 (코인은 회전연출 제외)
        if (isRewardItem && Item.Itemtype != Item.ItemType.GoldCoin)
        {
            if (_rotationItemCoroutine != null)
            {
                StopCoroutine(_rotationItemCoroutine);
            }
            _rotationItemCoroutine = StartCoroutine(RotationItemPrefab(item3D.transform));
        }
    }

    /// <summary>
    /// 보상 아이템 360도 회전연출 
    /// </summary>
    /// <param name="item3D">회전시킬 아이템</param>
    /// <returns></returns>
    private IEnumerator RotationItemPrefab(Transform item3D)
    {
        float elapsedTime = 0f;
        var initialRotation = item3D.transform.eulerAngles;

        // 회전 종료 시간
        while (elapsedTime < 0.75f)
        {
            // 회전 속도에 기반하여 오브젝트를 회전
            float normalizedTime = elapsedTime / 0.75f;
            float newYRotation = initialRotation.y + (360f * normalizedTime);
            item3D.transform.eulerAngles = new Vector3(initialRotation.x, newYRotation, initialRotation.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 아이템 카드가 비활성화 될 경우 마지막에 표시된 3D 아이템 오브젝트도 비활성화
    /// </summary>
    private void OnDisable()
    {
        // 마지막 아이템 카드가 골드인 경우 골드 전용 텍스드도 비활성화
        if (Item.Itemtype is Item.ItemType.GoldCoin) _itemStockText.gameObject.SetActive(false);
            
        
        ItemObjectDictionary[Item.ItemName].SetActive(false);
    }
}
