using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using DG.Tweening;
using System;

public class UI_RewardButton : UI_Base
{
    PlayerStats _owner;
    Item _item;
    UI_CaveTImeLineGird _uIcave =null;
    Vector3 PanelorignalSize =Vector3.zero;
    private enum Buttons
    {
        ShareButton,
        UseButton,
        EquipButton,
        collectButton,
        PassButton,
        DisarmButton,
        MoveButton,
        ForesightButton,
    }
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.ShareButton).gameObject.BindEvent(data => { ShareButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.UseButton).gameObject.BindEvent(data => { UseButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.EquipButton).gameObject.BindEvent(data => { EquipButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.collectButton).gameObject.BindEvent(data => { collectButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.PassButton).gameObject.BindEvent(data => { PassButtonEvent(); }, MouseUIEvent.Click);

        Get<Button>((int)Buttons.DisarmButton).gameObject.BindEvent(data => { DisarmButtonClick(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.DisarmButton).gameObject.BindEvent(data => { DisarmButtonEnter(); }, MouseUIEvent.Enter);
        Get<Button>((int)Buttons.DisarmButton).gameObject.BindEvent(data => { DisarmButtonExit(); }, MouseUIEvent.Exit);

        Get<Button>((int)Buttons.MoveButton).gameObject.BindEvent(data => { MoveButtonEvent(); }, MouseUIEvent.Click);

        Get<Button>((int)Buttons.ForesightButton).gameObject.BindEvent(data => { ForesightButtonClick(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.ForesightButton).gameObject.BindEvent(data => { ForesightButtonEnter(); }, MouseUIEvent.Enter);
        Get<Button>((int)Buttons.ForesightButton).gameObject.BindEvent(data => { ForesightButtonExit(); }, MouseUIEvent.Exit);


    }

    public void SetOwner(PlayerStats playerStats)
    {
        _owner = playerStats;
    }
    private void ShareButtonEvent()
    {
        int count = Managers.Instance.GetPlayerCount;
        for (int j = 0; j < count; j++)
        {
            Managers.Instance.GetPlayer(j).PlayerStats.UpdateGold(_item.ItemStock / count);
        }
        Managers.Battle.ShowReward(); 
    }
    private void UseButtonEvent()
    {
        Managers.Battle.ShowReward();
    }
    private void EquipButtonEvent()
    {
        _owner.UpdateItem(_item);
        Managers.Inventory.AddInventoryItem(_owner, _item);
        Managers.Battle.ShowReward();
    }
    private void collectButtonEvent()
    {
        if (_item.Itemtype == Item.ItemType.GoldCoin)
        {
            _owner.UpdateGold(_item.ItemStock);
        }
        else
        {
            _owner.UpdateItem(_item);
            Managers.Inventory.AddInventoryItem(_owner, _item);
        }
        Managers.Battle.ShowReward();
    }
    private void PassButtonEvent()
    {
        Managers.Battle.ShowReward();
    }

    public void SetRewardBtn(Item item)
    {
        _item = item;
        for(int i =(int)Buttons.DisarmButton; i<=(int)Buttons.ForesightButton; i++)
        {
            Get<Button>(i).gameObject.SetActive(false);
        }
        Get<Button>((int)Buttons.PassButton).gameObject.SetActive(true);
        switch (item.Itemtype)
        {
            case Item.ItemType.GoldCoin:
                Get<Button>((int)Buttons.ShareButton).gameObject.SetActive(true);
                Get<Button>((int)Buttons.UseButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.EquipButton).gameObject.SetActive(false);
                break;
            case Item.ItemType.QuestItem:
                Get<Button>((int)Buttons.ShareButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.UseButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.EquipButton).gameObject.SetActive(false);
                break;
            case Item.ItemType.Consumable:
                Get<Button>((int)Buttons.ShareButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.UseButton).gameObject.SetActive(true);
                Get<Button>((int)Buttons.EquipButton).gameObject.SetActive(false);
                break;
           default:
                Get<Button>((int)Buttons.ShareButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.UseButton).gameObject.SetActive(false);
                Get<Button>((int)Buttons.EquipButton).gameObject.SetActive(true);
                break;

        }
    }

    public void SetTrapBtn()
    {
        for(int i=0; i<=(int) Buttons.PassButton; i++)
        {
            Get<Button>(i).gameObject.SetActive(false);
        }
        Get<Button>((int)Buttons.DisarmButton).gameObject.SetActive(true);
        Get<Button>((int)Buttons.MoveButton).gameObject.SetActive(false);
        Get<Button>((int)Buttons.ForesightButton).gameObject.SetActive(false);
        if (_uIcave == null)
        {
            _uIcave = Managers.UIManager.UICave;
        }
        if(PanelorignalSize == Vector3.zero)
        {
            PanelorignalSize = _uIcave.GetGameObject(0).transform.localScale;
        }
       
    }

    private void DisarmButtonClick()
    {
        if(Managers.Cave.NowEvent ==CaveManager.CaveEvent.TRAP)
            _uIcave.CaveIcons.SetRandomSlot(_owner,CaveIcon.StageType.TRAP1);
        if (Managers.Cave.NowEvent == CaveManager.CaveEvent.TRAP2)
            _uIcave.CaveIcons.SetRandomSlot(_owner, CaveIcon.StageType.TRAP2);
        List<PlayerStats> list = Managers.Cave.PlayerStatsList;
        for (int i = 0; i < list.Count; i++)
        {
            Managers.UIManager.GetPlayerHUD(list[i]).VoteBtnOff();
        }
        _uIcave.GetGameObject(0).transform.DOScale(Vector3.zero, 0.2f).From(PanelorignalSize)
             .OnComplete(() =>
             {
                 _uIcave.GetGameObject(0).SetActive(false);
                 _uIcave.GetGameObject(0).transform.localScale = PanelorignalSize;
             });
    }
    private void DisarmButtonEnter()
    {
        List<float> list = StatRoll.SuccessSlot(_owner,Managers.Cave.NowCave.UsingSlot, _uIcave.SlotCount);
        _uIcave.SetTrapMessages(list);
        _uIcave.GetGameObject(0).SetActive(true);
        _uIcave.GetGameObject(0).transform.DOScale(PanelorignalSize, 0.2f).From(Vector3.zero);
    }
    private void DisarmButtonExit()
    {
        _uIcave.GetGameObject(0).transform.DOScale(Vector3.zero, 0.2f).From(PanelorignalSize);

    }

    public void SetClearBtn()
    {
        for (int i = 0; i <= (int)Buttons.DisarmButton; i++)
        {
            Get<Button>(i).gameObject.SetActive(false);
        }
        Get<Button>((int)Buttons.MoveButton).gameObject.SetActive(true);
        Get<Button>((int)Buttons.ForesightButton).gameObject.SetActive(true);
        if (_uIcave == null)
        {
            _uIcave = Managers.UIManager.UICave;
        }
        if (PanelorignalSize == Vector3.zero)
        {
            PanelorignalSize = _uIcave.GetGameObject(0).transform.localScale;
        }

    }

    public void SetOnlyMoveBtn()
    {
        for (int i = 0; i <= (int)Buttons.DisarmButton; i++)
        {
            Get<Button>(i).gameObject.SetActive(false);
        }
        Get<Button>((int)Buttons.MoveButton).gameObject.SetActive(true);
        Get<Button>((int)Buttons.ForesightButton).gameObject.SetActive(false);

    }

    private void MoveButtonEvent()
    {
        List<PlayerStats> list = Managers.Cave.PlayerStatsList;
        for (int i = 0; i < list.Count; i++)
        {
            Managers.UIManager.GetPlayerHUD(list[i]).VoteBtnOff();
        }
        _uIcave.ShowChooseBtns(true);
        _uIcave.GetGameObject(0).SetActive(false);
        _uIcave.GetButton(0).gameObject.SetActive(true);
        _uIcave.GetButton(2).gameObject.SetActive(true);
    }

    private void ForesightButtonClick()
    {
        List<PlayerStats> list = Managers.Cave.PlayerStatsList;
        for (int i = 0; i < list.Count; i++)
        {
            Managers.UIManager.GetPlayerHUD(list[i]).VoteBtnOff();
        }
        _uIcave.ShowChooseBtns(false);
        _uIcave.GetGameObject(0).SetActive(false);
        _uIcave.GetGameObject(0).transform.DOScale(Vector3.zero, 0.2f).From(PanelorignalSize)
             .OnComplete(() =>
             {
                 _uIcave.GetGameObject(0).SetActive(false);
                 _uIcave.GetGameObject(0).transform.localScale = PanelorignalSize;
             });

        _uIcave.ForeSightPlayer = _owner;
        _uIcave.GetButton(0).gameObject.SetActive(!_uIcave.IsLeftForesight);
        _uIcave.GetButton(2).gameObject.SetActive(!_uIcave.IsRightForesight);
    }
    private void ForesightButtonEnter()
    {
        List<float> list = StatRoll.SuccessSlot(_owner, Managers.Cave.NowCave.UsingSlot, _uIcave.SlotCount);
        _uIcave.SetForeSightMessages(list);
        _uIcave.GetGameObject(0).SetActive(true);
        _uIcave.GetGameObject(0).transform.DOScale(PanelorignalSize, 0.2f).From(Vector3.zero);
    }
    private void ForesightButtonExit()
    {
        _uIcave.GetGameObject(0).transform.DOScale(Vector3.zero, 0.2f).From(PanelorignalSize);
    }


}
