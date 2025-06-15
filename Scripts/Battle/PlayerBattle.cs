using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerBattle : BattleUnit
{
    [HideInInspector]
    public List<Skill> SkillList =new List<Skill>();
    [HideInInspector]
    public PlayerStats PlayerStat =null;
    [HideInInspector]
    public Skill Run =null;
    public GameObject[] Effect; //스킬에 이펙트 id가 저장되고 나중에 resource로 불러드림
    public Transform EffectPos;
    [HideInInspector]
    public string EffectName;
    public override void BattleDataInit(int id=-1, PlayerStats playerStats = null)
    {
        _id = id;
        base.BattleDataInit();
        StatBind(playerStats);
        data.BaseLevel = PlayerStat.BaseLevel;
        data.BaseHp = PlayerStat.CurrentHp;
        data.BaseAr = PlayerStat.PhysicalArmor;
        data.BaseRes = PlayerStat.Resistance;
        data.BaseAvd = PlayerStat.Avoid;
        data.BaseSpd = (int)(PlayerStat.Quickness *100);
        data.BaseAtt = PlayerStat.MaxDamage;
        data.BaseCr = (int)PlayerStat.CriticalChance;
        CurHp = data.Hp;
    }
   
    void StatBind(PlayerStats playerStats)
    {
        if (PlayerStat == null)
        {
            PlayerStat = playerStats;
        }
        if (Run == null)
        {
            Run = Managers.Data.GetUsedSkillInfo(0);
        }
        SkillList.Clear();
        if (!Managers.Battle.IsCave)
        {
            SkillList.Add(Run);
        }
        foreach (var skillid in PlayerStat.BaseWeapon.Skills)
        {
            SkillList.Add(Managers.Data.GetUsedSkillInfo(skillid));
        }
        if (!Icon)
        {
            Icon = PlayerStat.PlayerPortrait;
        }
        /* 23/05/29 무기 타입 코드 수정 */
        //CharacterEventListener.SetRunTimeAnimator(Managers.Resource.LoadResource<RuntimeAnimatorController>(Enum.GetValues(typeof(DataManager.HumanoidAnim)).GetValue((int)PlayerStat.BaseWeapon.Weapontype).ToString()));
        CharacterEventListener.SetRunTimeAnimator(Managers.Resource.LoadResource<RuntimeAnimatorController>(ResourceManager.ResourcePath.None,
            "Humanoid__" + PlayerStat.BaseWeapon.WeaponHandlegrip + "_" + PlayerStat.BaseWeapon.Weapontype, true));
    }

    public void CreateEffect()
    {
        EffectName = OnSkill.Effect;
        if(EffectName != "")
        {
            GameObject effect = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.Effect, EffectName);
            Instantiate(effect, EffectPos.position, EffectPos.rotation);
        }
        
    }

    public override void PutToken(TokenType tokenType, int Count)
    {
        base.PutToken(tokenType, Count);
        Managers.UIManager.GetPlayerHUD(PlayerStat).UpdateTokenUI(tokenType, Count);
    }
    public override void RealTokenActive(Token token)
    {
        base.RealTokenActive(token);
        Managers.UIManager.GetPlayerHUD(PlayerStat).UpdateTokenUI(token.tokenType, token.Count);
    }


    #region 사운드 함수
    public override void AttackSound()
    {
        if (OnSkill.HaveSound)
        {
            Managers.Sound.PlaySFX("SKills/"+OnSkill.Id, _audioSource);
        }
        else
        {
            Managers.Sound.PlaySFX("Player/"+_id + "/"+_id+"_Attack", _audioSource);
        }
    }
    public override void TakeDamageSound()
    {
        Managers.Sound.PlaySFX("Player/TakeDamage", _audioSource);
    }
    public override void TakeDamageHeavySound()
    {
        Managers.Sound.PlaySFX("Player/TakeDamageHeavy", _audioSource);
    }
    public override void TauntSound()
    {
        Managers.Sound.PlaySFX("Player/Taunt", _audioSource,3f);
    }
    public override void AvoidSound()
    {
        Managers.Sound.PlaySFX("Avoid", _audioSource,2f);
    }
    public override void DeadSound()
    {
        Managers.Sound.PlaySFX("Player/Dead", _audioSource);
    }
    public override void ReviveSound()
    {

    }
    #endregion
}
