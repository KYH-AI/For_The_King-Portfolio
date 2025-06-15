using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningToken : Token
{
    public WeakeningToken(BattleUnit battleUnit, int count) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Weakening;
        CanOverlap = false;
        Count = count;
    }
    public override void Active()
    {
        _battleUnit.data.Attalpa = 0.5f;
    }
    public override void Remove()
    {
        _battleUnit.FXList[6].SetActive(false);
        _battleUnit.data.Attalpa = 1f;
    }
    public override void PutToken()
    {
        _battleUnit.FXList[6].SetActive(true);
    }
}
