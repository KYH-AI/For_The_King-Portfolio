using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemMenuButton : UI_PopUp
{
    /*          <������ ��ȣ�ۿ� UI>
        1. �������� ������ Ŭ�� �� ��ư ����
            1. ����
            2. ���� �� ����
            3. �ݱ�

        2. �������� �κ��丮 ������ Ŭ�� �� ��ư ����
            1. ���
            2. ����
            3. �Ǹ� (��� ���� text) ��� ������
            4. �ֱ� �÷��̾� �ʻ�ȭ (���� �÷��̾� �ִ� ��� ��ŭ)
            5. �ݱ�

        3. ���������� �κ��丮 ������ Ŭ�� �� ��ư ����
            1. ���
            2. ����
            3. �Ǹ� (��Ȱ��ȭ)
            4. �ֱ� �÷��̾� �ʻ�ȭ (���� �÷��̾� �ִ� ��� ��ŭ)
            5. �ݱ�
     */

    private enum Buttons
    {
        UseButton,   // ������ ��� ��ư
        UnEquipButton, // ���� ��ü ��ư
        EquipButton, // ������ ���� ��ư
        ShellButton, // �Ǹ� ��ư
        SendButton1, // �÷��̾� 1���� ������ ���� ��ư
        SendButton2, // �÷��̾� 2���� ������ ���� ��ư
        BuyButton,   // ���� ��ư
        BuyAndEquipButton,  // ���� �� ���� ��ư
        CloseButton         // �ݱ� ��ư
    }

    private enum ButtonText
    {
        ShellButtonText
    }


    public enum ItemMenuType
    {
        NONE = -1,
        Default = 0,     // ����
        Shop = 1,       // �÷��̾� ��(���� �̿��)
        Shopping = 2,   // ���� ��
        LoadOut = 3,    // �÷��̾� �ε�ƿ� (��� ����)
    }

    private ItemMenuType _lastType = ItemMenuType.NONE;

    // �÷��̾ ������ ������ ����
    public Item SelectItem { get; private set; }

    // ������ �������� ���
    private Equipment _equipmentItem;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(ButtonText));

        // �ݱ� ��ư �̺�Ʈ
        Get<Button>((int)Buttons.CloseButton).onClick.AddListener(base.ClosedPopUpUI);

        // ��� ��ư �̺�Ʈ
        //Get<Button>((int)Buttons.UseButton).onClick.AddListener();

        // ��� ��ü ��ư �̺�Ʈ
        Get<Button>((int)Buttons.UnEquipButton).onClick.AddListener(() =>
        {
            Managers.Inventory.UnEquipmentItem(Managers.Inventory.RequestPlayer, _equipmentItem.EquipmentSlottype);
            base.ClosedPopUpUI();
        });
        
        // ���� ��ư �̺�Ʈ
        Get<Button>((int)Buttons.BuyButton).onClick.AddListener(() => 
        { 
            Managers.Store.BuyItem(Managers.Store.CurrentStore, Managers.Store.Customer, SelectItem);
            base.ClosedPopUpUI();
        });

        // �Ǹ� ��ư �̺�Ʈ
        Get<Button>((int)Buttons.ShellButton).onClick.AddListener(() =>
        {
            Managers.Store.SellItem(Managers.Store.Customer, SelectItem);
            base.ClosedPopUpUI();
        });
        
         // ��� ���� ��ư �̺�Ʈ
        Get<Button>((int)Buttons.EquipButton).onClick.AddListener( ()=>
        {
            Managers.Inventory.EquipmentItem(Managers.Inventory.RequestPlayer, (Equipment)SelectItem);
            base.ClosedPopUpUI();
        });
        
        // ���� �� ���� ��ư �̺�Ʈ
        Get<Button>((int)Buttons.BuyAndEquipButton).onClick.AddListener(() =>
        {
            Managers.Store.BuyItem(Managers.Store.CurrentStore, Managers.Store.Customer, SelectItem);
            Managers.Inventory.EquipmentItem(Managers.Store.Customer, (Equipment)SelectItem);
            base.ClosedPopUpUI();
        });
    }

    public void ActiveItemMenuUI(PlayerStats requestPlayer, Item item, ItemMenuType itemMenuType)
    {
        // �̹� ������ ��ȣ�ۿ� ��ư UI�� Ȱ��ȭ �� ���� ��Ȱ��ȭ�ϰ� ����
        if(this.gameObject.activeSelf) base.ClosedPopUpUI();
        
        // ������ ������ ���� �Ҵ�
        SelectItem = item;
        
        // �̹� ������ �������� ��츦 ���� Ȯ��
        if (item is Equipment equipment)
        {
            _equipmentItem = equipment;
        }
        
        // ���� ������ �̿����� �÷��̾� != �κ��丮 UI�� ��û�� �÷��̾� = ������ �ǸŹ�ư ��ȣ�ۿ� ��Ȱ��ȭ
        if (itemMenuType is ItemMenuType.Shop && Managers.Store.Customer != requestPlayer) 
            Get<Button>((int)Buttons.ShellButton).interactable = false;
        else if(itemMenuType is ItemMenuType.Shop)
            Get<Button>((int)Buttons.ShellButton).interactable = true;
        
        // ���� ������ ���Ź�ư Ȯ��
        if (itemMenuType is ItemMenuType.Shopping)
        {
            // ���� �÷��̾� ��ȭ�� �������� ���Ű����� ��� ���� ��ư Ȱ��ȭ
            Get<Button>((int)Buttons.BuyAndEquipButton).interactable = Get<Button>((int)Buttons.BuyButton).interactable = Managers.Store.Customer.CurrentGold >= item.BuyPrice;
        }


        // �Ǹ� ���� UI Text ǥ��
        Get<TextMeshProUGUI>((int)ButtonText.ShellButtonText).text = "�Ǹ� (" + SelectItem.SellPrice + ")";
        
        if (itemMenuType == _lastType)
        {
            // �κ��丮 ���¿� �´� ��ư Ȱ��ȭ ���� ��ŵ
            base.Init();
            return;
        }
        
        switch (itemMenuType)
        {
            case ItemMenuType.Default :
                DisableButtons(Buttons.ShellButton, Buttons.BuyButton, Buttons.BuyAndEquipButton, Buttons.UnEquipButton);
                EnableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.SendButton1, Buttons.SendButton2);
                break;
            
            case ItemMenuType.LoadOut :
                DisableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton, 
                                Buttons.SendButton1, Buttons.SendButton2, Buttons.BuyButton, Buttons.BuyAndEquipButton);
                EnableButtons(Buttons.UnEquipButton);
                break;
            
            case ItemMenuType.Shop :
                DisableButtons(Buttons.BuyButton, Buttons.BuyAndEquipButton, Buttons.UnEquipButton);
                EnableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton,  
                               Buttons.SendButton1, Buttons.SendButton2);
                break;
            
            case ItemMenuType.Shopping :
                DisableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton, 
                               Buttons.SendButton1, Buttons.SendButton2, Buttons.UnEquipButton);
                EnableButtons(Buttons.BuyButton, Buttons.BuyAndEquipButton);
                break;
            
        }

        _lastType = itemMenuType;
        
        // ��ư UI Ȱ��ȭ
        base.Init();
      
    }

    // ��ư ��Ȱ��ȭ
    private void DisableButtons(params Buttons[] disableButtons)
    {
        foreach (var button in disableButtons)
        {
            Get<Button>((int)button).gameObject.SetActive(false);
        }
    }

    // ��ư Ȱ��ȭ
    private void EnableButtons(params Buttons[] enableButtons)
    {
        foreach (var button in enableButtons)
        {
            Get<Button>((int)button).gameObject.SetActive(true);
        }
    }
    

}
