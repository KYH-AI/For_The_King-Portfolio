using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerEquippedItemCard : UI_ItemCard
{
    
    // 현재 착용중인 아이템 카드 위치 선정
    public enum EquippedTarget
    {
        EquipShopTarget, // 상점 이용 시
        EquipInventoryTarget // 인벤토리 이용 시
    }

    // 인베토리 좌표
    private readonly Vector2 _inventoryTargetPosition = new Vector2(-427f, 18f);
    
    // 상점 좌표
    private readonly Vector2 _shopTargetPosition = new Vector2(427f,18f);

    /// <summary>
    /// 장비 착용에 대한 아이템 카드 UI 위치 선정
    /// </summary>
    /// <param name="equippedTarget">왼쪽, 오른쪽 구별</param>
    public void SetPlayerEquippedItemCardPosition(EquippedTarget equippedTarget)
    {
        var targetPosition = equippedTarget is EquippedTarget.EquipInventoryTarget ? _inventoryTargetPosition : _shopTargetPosition;
        transform.localPosition = targetPosition;
    }

    protected override void SetItemInfo(bool isRewardItem)
    {
        
    }
}
