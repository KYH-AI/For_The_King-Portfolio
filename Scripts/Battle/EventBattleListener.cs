using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBattleListener : MonoBehaviour
{
    BattleUnit BattleUnit;
    private void Start()
    {
        BattleUnit = GetComponentInParent<BattleUnit>();
    }

    public void Fight()
    {
        BattleUnit.Fight();
    }
    public void Destroyed()
    {
        BattleUnit.Destroyed();
    }
    #region 사운드 함수
    public void AttackSound()
    {
        BattleUnit.AttackSound();
    }
    public void TakeDamageSound()
    {
        BattleUnit.TakeDamageSound();
    }
    public void TakeDamageHeavySound()
    {
        BattleUnit.TakeDamageHeavySound();
    }
    public void TauntSound()
    {
        BattleUnit.TauntSound();
    }
    public void AvoidSound()
    {
        BattleUnit.AttackSound();
    }
    public void DeadSound()
    {
        BattleUnit.DeadSound();
    }
    public void ReviveSound()
    {
        BattleUnit.ReviveSound();
    }
    #endregion
}
