using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Reward 
{
    public Item.ItemType Type;//���� Ÿ��
    int _coinCount;//���� ����
    public int Id;//������ ���̵�
    public bool IsCoin;

    public Reward(int coinCount)
    {
        IsCoin = true;
        _coinCount = coinCount;
        Type = Item.ItemType.GoldCoin;
        Id = 0;
    }
    public Reward(Item.ItemType rewardType, int id)
    {
        Type = rewardType;
        IsCoin = false;
        Id = id;
    }
    public  Item GiveItem()
    {
        if(Type == Item.ItemType.GoldCoin)
        {
            return Managers.Data.GetItemInfo(Type, _coinCount);
        }
        return Managers.Data.GetItemInfo(Type, Id);
    }

}
