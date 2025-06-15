using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerEquippedItemCard : UI_ItemCard
{
    
    // ���� �������� ������ ī�� ��ġ ����
    public enum EquippedTarget
    {
        EquipShopTarget, // ���� �̿� ��
        EquipInventoryTarget // �κ��丮 �̿� ��
    }

    // �κ��丮 ��ǥ
    private readonly Vector2 _inventoryTargetPosition = new Vector2(-427f, 18f);
    
    // ���� ��ǥ
    private readonly Vector2 _shopTargetPosition = new Vector2(427f,18f);

    /// <summary>
    /// ��� ���뿡 ���� ������ ī�� UI ��ġ ����
    /// </summary>
    /// <param name="equippedTarget">����, ������ ����</param>
    public void SetPlayerEquippedItemCardPosition(EquippedTarget equippedTarget)
    {
        var targetPosition = equippedTarget is EquippedTarget.EquipInventoryTarget ? _inventoryTargetPosition : _shopTargetPosition;
        transform.localPosition = targetPosition;
    }

    protected override void SetItemInfo(bool isRewardItem)
    {
        
    }
}
