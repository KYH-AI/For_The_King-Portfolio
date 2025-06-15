using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenType
{
    None =-1,
    Buff = 0,
    Firmness,//����
    Avoid,//ȸ��
    Fast, //�ż�
    Reproduction, //���
    Focus,//���߷�
    Insight,//����
    Strength,//������
    Chance,//��ȸ
    Enforce,//��ȭ
    Action, //�ൿ
    PoisonAttack, //�͵� �ٸ���
    SawBladeAttack,//�鳯 ����
    FireAttack, //��ȭ��
    ChainAttack, //�罽 ����  
    BlindAttack, // �𷡻Ѹ���
    HunterAttack, // ǥ�� ����
    WeakenessAttack,//���� ����
    Stealth,//����
    Provocation,//����
    Ready,//�غ��
    DeBuff = 30,
    Stun,//����
    SlowDown,//��ȭ
    Bleeding, //����
    Poison,//�ߵ�
    Burn,//ȭ��
    Freezing, //����
    Blind, //�Ǹ�
    Weakening, //��ȭ
    Vulnerability,//���
    Haziness,//�帴��
    Restraint,//�ൿ����
    Chaos, //ī����
    Exhausted, //��ħ
    Target, //ǥ��
    Unrest,//�Ҿ�
}
public enum ActiveTime
{
    Immediately,
    TurnStart,
    Attack,
    TakeDamage,
    GetOtherToken,
    EnemyTurn,
    MyTurnStart,
}
public class Token
{
    public ActiveTime activeTime;
    public TokenType tokenType;
    public bool CanOverlap;
    public int Count;
    protected BattleUnit _battleUnit;
    
    public Token(BattleUnit battleUnit)
    {
        _battleUnit = battleUnit;
    }
    

    public virtual void Active()
    {
        Count--;
    }
    public virtual void Remove()
    {

    }
    public virtual void PutToken()
    {

    }
}
