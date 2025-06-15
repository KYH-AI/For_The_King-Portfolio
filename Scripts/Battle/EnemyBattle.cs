using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattle : BattleUnit
{
    [HideInInspector]
    public EnemyStat EnemyStat;
    [HideInInspector]
    public List<Skill> SkillList = new();
    UI_EnemyInfo _uI_EnemyInfos;
    public override void BattleDataInit(int id = -1, PlayerStats playerStats = null)
    {
        _uI_EnemyInfos = Managers.Battle.UI_Battle.uI_EnemyInfos[EnemyIndex];
        _id = id;
        base.BattleDataInit();
        StatBind(id);
        data.BaseLevel = EnemyStat.EnemyLevel;
        data.BaseHp = EnemyStat.EnemyHealth;
        data.BaseAr = EnemyStat.EnemyArmor;
        data.BaseRes = EnemyStat.enemyRes;
        data.BaseAvd = EnemyStat.EnemyAvoid;
        data.BaseAtt = EnemyStat.EnemyAttck;
        data.BaseSpd = EnemyStat.EnemySpeed;
        data.BaseAcc = EnemyStat.EnemyAcc;
        data.BaseCr = EnemyStat.EemyCr;
        CurHp = data.Hp;
        
    }
    void StatBind(int id)
    {
        EnemyStat = Managers.Data.GetEnemyStatsInfo(id);
        if (SkillList.Count < 1)
        {
            foreach (var skillid in EnemyStat.Skills)
            {
                SkillList.Add(Managers.Data.GetUsedSkillInfo(skillid));
            }
        }
        Icon = EnemyStat.EnemyIcon;
    }
    public override void PutToken(TokenType tokenType, int Count)
    {
        base.PutToken(tokenType, Count);
        _uI_EnemyInfos.SetToken(_tokenLists);
    }

    public override void ActiveToken(ActiveTime activeTime)
    {
        base.ActiveToken(activeTime);
        _uI_EnemyInfos.SetToken(_tokenLists);
    }
    #region 사운드 함수
    public override void AttackSound()
    {
        Managers.Sound.PlaySFX("Enemy/"+ _id +"/"+_id+"_Attack", _audioSource);
    }
    public override void TakeDamageSound()
    {
        Managers.Sound.PlaySFX("Enemy/" + _id + "/" + _id + "_TakeDamage", _audioSource);
    }
    public override void TakeDamageHeavySound()
    {
        Managers.Sound.PlaySFX("Enemy/" + _id + "/" + _id + "_TakeDamage", _audioSource);
    }
    public override void TauntSound()
    {
        Managers.Sound.PlaySFX("Enemy/" + _id + "/" + _id + "_Taunt", _audioSource);
    }
    public override void AvoidSound()
    {
        Managers.Sound.PlaySFX("Avoid", _audioSource,2f);
    }
    public override void DeadSound()
    {
        Managers.Sound.PlaySFX("Enemy/" + _id + "/" + _id + "_Dead", _audioSource);
    }
    public override void ReviveSound()
    {

    }
    #endregion
}
