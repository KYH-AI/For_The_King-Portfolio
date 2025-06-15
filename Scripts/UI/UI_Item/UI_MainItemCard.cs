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

    // ������ ��� (�׳� ��ȭ ǥ��)
    private TextMeshProUGUI _itemStockText;

    // ȸ�� ����� �ڷ�ƾ
    private Coroutine _rotationItemCoroutine;

    // 3D ������ ���� ĳ�� (static : ������ ���� ��� ���⿡ ���� 3D �������� ��� �ʱ�ȭ)
    public static readonly  Dictionary<string, GameObject> ItemObjectDictionary = new Dictionary<string, GameObject>();

    // ���������� ����� 3D ������ �̸�
    private string lastItem3dName = string.Empty;
    
    // 3D ������ ���� ��ġ���� ([0] = ��ǥ, [1] = ȸ��)
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
        // ���� �������� ����� ��� ��� ������ ǥ��
        if (Item.Itemtype is Item.ItemType.GoldCoin)
        {
            _itemStockText.text = "x"+Item.ItemStock;
            _itemStockText.gameObject.SetActive(true);
        }

        SetItemRendererTexture(isRewardItem);
    }

    /// <summary>
    /// ������ 3D ������Ʈ ������ 
    /// </summary>
    private void SetItemRendererTexture(bool isRewardItem)
    {
        Vector3[] itemRendererOffSetPos = null;

        // ���������� ����� ������ ��Ȱ��ȭ
        if (!string.IsNullOrEmpty(lastItem3dName))
        {
            if (ItemObjectDictionary.TryGetValue(lastItem3dName, out GameObject lastItemObject) && lastItemObject.activeSelf)
            {
                lastItemObject.SetActive(false);
            }
        }
        
        // ������ ������ ����� ������ ��� ������ǥ ����
        if (Item.Itemtype is Item.ItemType.Weapon)
        {
            Weapon weapon = (Weapon)Item;
            itemRendererOffSetPos = _WEAPON_ITEM_RENDERER_POSITION[weapon.Weapontype];
        }
        
        // ����� 3D ������ ������Ʈ�� ���°�� ���λ��� �� ����
        if (!ItemObjectDictionary.ContainsKey(Item.ItemName))
        {
            
            ItemObjectDictionary.Add(Item.ItemName, 
                            Instantiate(Managers.Resource.LoadResource<GameObject>(Managers.Resource.GetItemPrefabPath(Item.Itemtype),
                            Item.ItemID + "_" + Managers.Data.GetItemName(Item.Itemtype, Item.ItemID)),
                                     Managers.UIManager.UIItemObjectRendererTexturePos));
        }

        GameObject item3D = ItemObjectDictionary[lastItem3dName = Item.ItemName];

        // ������ ���⿡ ��ǥ ������ �ʿ��� ��� ����
        if (itemRendererOffSetPos != null)
        {
            item3D.transform.localPosition = itemRendererOffSetPos[0];
            item3D.transform.localEulerAngles = itemRendererOffSetPos[1];
        }
        
         item3D.SetActive(true);
        
        // ���� �������� ��� 360�� ȸ������ �ʿ� (������ ȸ������ ����)
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
    /// ���� ������ 360�� ȸ������ 
    /// </summary>
    /// <param name="item3D">ȸ����ų ������</param>
    /// <returns></returns>
    private IEnumerator RotationItemPrefab(Transform item3D)
    {
        float elapsedTime = 0f;
        var initialRotation = item3D.transform.eulerAngles;

        // ȸ�� ���� �ð�
        while (elapsedTime < 0.75f)
        {
            // ȸ�� �ӵ��� ����Ͽ� ������Ʈ�� ȸ��
            float normalizedTime = elapsedTime / 0.75f;
            float newYRotation = initialRotation.y + (360f * normalizedTime);
            item3D.transform.eulerAngles = new Vector3(initialRotation.x, newYRotation, initialRotation.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// ������ ī�尡 ��Ȱ��ȭ �� ��� �������� ǥ�õ� 3D ������ ������Ʈ�� ��Ȱ��ȭ
    /// </summary>
    private void OnDisable()
    {
        // ������ ������ ī�尡 ����� ��� ��� ���� �ؽ��嵵 ��Ȱ��ȭ
        if (Item.Itemtype is Item.ItemType.GoldCoin) _itemStockText.gameObject.SetActive(false);
            
        
        ItemObjectDictionary[Item.ItemName].SetActive(false);
    }
}
