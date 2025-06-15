using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class UI_BattleButtonGrid : UI_EventPopUpButtonGrid
{

    public enum Buttons
    {
        FightButton,
        AmbushButton,
        SneakButton,
        LeaveButton
    }


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.FightButton).gameObject.BindEvent(data => { FightButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.AmbushButton).gameObject.BindEvent(data => { AmbushButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.SneakButton).gameObject.BindEvent(data => { SneakButtonEvent(); }, MouseUIEvent.Click);
        BindEventLeaveButton((int)Buttons.LeaveButton);
    }

    protected override void LeaveButtonEvent()
    {
        this.WorldMapPlayerCharacter.AutoMovingHex(this.WorldMapPlayerCharacter.LastHex);
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }


    private void FightButtonEvent()
    {
        List<PlayerStats> playerObjectList = new List<PlayerStats>(ParentUIPopUp.UI_MainEventPopUp.GetInteractionPlayerList());
        List<int> enemyList = new List<int>(ParentUIPopUp.UI_MainEventPopUp.GetInteractionEventList());
        
        foreach (var VARIABLE in playerObjectList)
        {   
            print($"{VARIABLE} ");
        }
        
        foreach (var VARIABLE in enemyList)
        {
            print($"{VARIABLE} ");
        }

        // ���� ������ ����Ʈ ���̵� Ȯ�� (������ -1)
        Managers.Battle.BattleInit(playerObjectList, enemyList, "BattleMap_1", BattleResult, PrototypeRandomReward(), ParentUIPopUp.UI_MainEventPopUp.GetHexQuestID().questID);
        Managers.Camera.nowState = CameraManager.NowState.BATTLE;
        Managers.UIManager.ChangeTimeLineGrid<UI_BattleTimeLineGrid>();
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }

    private List<Item> PrototypeRandomReward()
    {
        int range = Random.Range(1, 4);
        List<Item> rewardList= new List<Item>(range);
        Item.ItemType itemType;
        int itemID;
        
        for (int i = 0; i < range; i++)
        {
            
            switch (Random.Range(1, 4))
            {
                case 1 : itemType = Item.ItemType.Weapon;
                    itemID = Random.Range(1, 4);
                    break;
                case 2 : itemType = Item.ItemType.Armor; 
                    itemID = Random.Range(0, 5);
                    break;
                case 3 : itemType = Item.ItemType.GoldCoin; 
                    itemID = Random.Range(1, 50);
                    break;
                default: itemType = Item.ItemType.NONE;
                    itemID = -1;
                    break;
            }

            rewardList.Add(Managers.Data.GetItemInfo(itemType, itemID));
        }

        return rewardList;
    }
    
    private void AmbushButtonEvent()
    {
        
    }
    
    private void SneakButtonEvent()
    {
        
    }

    /// <summary>
    /// ��������� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="isRun">�������� �����ƴ��� �Ǵ���</param>
    private void BattleResult(bool isRun)
    {
        // �������� ����ģ ���
        if (isRun)
        {
            // �÷��̾�� LastHex ��ġ�� ������
           // worldMapPlayerCharacter.Retreat();
        }
        else // �������� ������ �ο���
        {
            // �÷��̾�� �������� Hex�� ������
            //ParentUIPopUp.CompleteHexEvent();
            
        }
        
        if(!isRun) ParentUIPopUp.UI_MainEventPopUp.CompleteHexEvent();
        this.WorldMapPlayerCharacter.AutoMovingHex(isRun ? this.WorldMapPlayerCharacter.LastHex : this.WorldMapPlayerCharacter.CurrentHex);
    }
}
