using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenType
{
    None =-1,
    Buff = 0,
    Firmness,//굳셈
    Avoid,//회피
    Fast, //신속
    Reproduction, //재생
    Focus,//집중력
    Insight,//명중
    Strength,//강인함
    Chance,//기회
    Enforce,//강화
    Action, //행동
    PoisonAttack, //맹독 바르기
    SawBladeAttack,//톱날 장착
    FireAttack, //인화성
    ChainAttack, //사슬 감기  
    BlindAttack, // 모래뿌리기
    HunterAttack, // 표적 지정
    WeakenessAttack,//약점 노출
    Stealth,//은신
    Provocation,//도발
    Ready,//준비됨
    DeBuff = 30,
    Stun,//기절
    SlowDown,//둔화
    Bleeding, //출혈
    Poison,//중독
    Burn,//화상
    Freezing, //빙결
    Blind, //실명
    Weakening, //약화
    Vulnerability,//취약
    Haziness,//흐릿함
    Restraint,//행동제한
    Chaos, //카오스
    Exhausted, //지침
    Target, //표적
    Unrest,//불안
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
