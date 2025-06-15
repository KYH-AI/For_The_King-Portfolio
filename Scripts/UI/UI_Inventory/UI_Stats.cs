using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Stats : UI_Base, I_Inventory
{
    /*  1. Ŭ���� �̸�
     * 
     *  2. Ŭ���� ���� ���
     * 
     *  3. ���������� ���� ���� ���
     * 
     *  4. ���� ���� ���ݷ�
     *    4-1. �⺻ ������ (���� �⺻ ������ + �÷��̾� �⺻ ������) (����, ���� ���� ����)
     *    4-2. ũ��Ƽ�� Ȯ��
     *    4-3. �߰� ���� ������
     *    4-4. �߰� ���� ������
     * 
     *  5. ��Ÿ �ɷ�ġ
     *   5-1. ���� ����
     *   5-2. XP
     *   5-3. XP �߰� ȹ�淮
     *   5-4. ���� ��ȭ
     *   5-5. ��ȭ �߰� ȹ�淮
     *   5-6. ���� �̵� �Ÿ� (�����)
     * 
     *  6. ���� ���� ���� (��� ���� ����)
     *   6-1. �⺻ ���� ���� (�÷��̾� �⺻ ���� ����)
     *   6-2. �⺻ ���� ���� (�÷��̾� �⺻ ���� ����)
     *   6-3. ���� ȸ�Ƿ� 
     *   6-4. �߰� ���� ����  (�� ���� ���� + �߰� ���� ����)
     *   6-5. �߰� ���� ����  (�� ���� ���� + �߰� ���� ����)
     *
     *  7. ���� ü�� / �ִ� ü��
     *   7-1. �⺻ �ִ� ü�� (�÷��̾� �⺻ �ִ� ü�� (baseHp + ���� �� �ִ�ü��)
     *   7-2. �߰� �ִ� ü�� (�߰� �ִ� ü��)
     *   7-3. ���� ���߷� ����Ʈ / �ִ� ���߷� ����Ʈ
     *   7-4. �⺻ �ִ� ���߷� ����Ʈ
     *   7-5. �߰� ���߷� ����Ʈ
     */


    public enum Texts
    {
        ClassNameText,
        
        MaxDamageText,
        BaseAttackDamageText,
        CritChanceText, // 0%
        ModifyPhysicalDamageText,
        ModifyMagicDamageText,
        
        MaxPhysicalArmorText, // 120
        MaxResistanceText,    //   / 120
        BasePhysicalArmorText,
        BaseResistanceText,
        MaxEvadeText,
        ModifyPhysicalArmorText,
        ModifyResistanceText,
        
        CurrentLevelText,
        CurrentXpText, // 0 / 150
        XpMultiText,   // 0%
        CurrentGoldText,
        GoldMultiText,  // 0%
        CurrentMaxMovementText,
        ModifyMovementText,
        
        MaxHpText,   // 0 / 150
        BaseMaxHpText,
        HpRegenText,
        MaxFocusText,  // 0 / 150
        ModifyMaxHpText
    }

    private enum TextRoot
    {
        ClassAbilitiesPanel,
        EquipAbilitiesPanel
    }
    
    public override void Init()
    {
        Bind<Transform>(typeof(TextRoot));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void InventoryUiUpdate(PlayerStats requestPlayer)
    {
        var textDisplay = requestPlayer.GetInventoryStatUiDisplay();

        for (int i = 0; i < textDisplay.Length; i++)
        {
            Get<TextMeshProUGUI>(i).text = textDisplay[i].Text;
            Get<TextMeshProUGUI>(i).color = textDisplay[i].Color;
        }
        
        // ���� UI Ȱ��ȭ
        this.gameObject.SetActive(true);
    }
}
