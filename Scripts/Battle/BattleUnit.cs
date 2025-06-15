using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BattleData
{
    #region 베이스 스탯
    public int BaseLevel;
    public int BaseHp;
    public int BaseAr;
    public int BaseRes;
    public int BaseAvd;
    public int BaseAtt;
    public int BaseSpd;
    public int BaseAcc;
    public int BaseCr;
    #endregion

    #region 최종 스탯
    public int Level => BaseLevel;
    public int Hp => (int)((float)BaseHp * Hpalpa);
    public int Ar => (int)((float)BaseAr * Aralpa);
    public int Res => (int)((float)BaseRes * Resalpa);
    public int Avd => (int)((float)BaseAvd * Avdalpa);
    public int Att => (int)((float)BaseAtt * Attalpa);
    public int Spd => (int)((float)BaseSpd * Spdalpa);
    public int Acc => (int)((float)BaseAcc * Accalpa);
    public int Cr => (int)((float)BaseCr * Cralpa);
    #endregion

    #region 보정치
    public float Hpalpa;
    public float Aralpa;
    public float Resalpa;
    public float Avdalpa;
    public float Attalpa;
    public float Spdalpa;
    public float Accalpa;
    public float Cralpa;
    #endregion

    public BattleData()
    {
          BaseLevel =1;
          BaseHp =1;
          BaseAr = 1;
          BaseRes = 1;
          BaseAvd = 1;
          BaseAtt = 1;
          BaseSpd = 1;
          BaseAcc = 1;
          BaseCr = 1;

          Hpalpa = 1;
          Aralpa = 1;
          Resalpa = 1;
          Avdalpa = 1;
          Attalpa = 1;
          Spdalpa = 1;
          Accalpa =1;
          Cralpa =1;
}


}
public struct slotCount
{
    public int Clear;
    public int Yel;
    public int Fail;
}
public class BattleUnit : MonoBehaviour
{
    [HideInInspector]
    public bool IsProgress = false;

    [HideInInspector]
    public bool IsAttack = false;

    [HideInInspector]
    public bool IsRun = false;

    [HideInInspector]
    public BattleUnit Target;

    [HideInInspector]
    public AudioClip audioClip;

    protected AudioSource _audioSource;

    [HideInInspector]
    public Vector3 nowpos;

    public List<GameObject> FXList;

    [HideInInspector]
    public int CurHp;

    [HideInInspector]
    public Sprite Icon;

    protected int _id;

    [HideInInspector]
    public int EnemyIndex;

    public slotCount SlotData;
    public CharacterEventListener CharacterEventListener { get; private set; }

    public BattleData data = new();

    protected Dictionary<ActiveTime, List<Token>> _tokenLists;

    [HideInInspector]
    public Skill OnSkill;


    #region 토큰함수
    public virtual void PutToken(TokenType tokenType, int Count)
    {
        Token token = GetToken(tokenType, Count);
        if (_tokenLists[token.activeTime].Any<Token>(x =>x.tokenType == token.tokenType))
        {
            if (token.CanOverlap)
            {
                Token temp = _tokenLists[token.activeTime].Find(x => x.tokenType == token.tokenType);
                int count = temp.Count + token.Count;
                temp.Count = Math.Min(count, 3);
            }
            else
            {
                return;
            }
            
        }
        else
        {
            _tokenLists[token.activeTime].Add(token);
        }
        
        token.PutToken();
        if (token.activeTime == ActiveTime.Attack)
        {
            token.Active();
        }
    }
    Token GetToken(TokenType tokenType, int Count)
    {
        switch (tokenType)
        {
            case TokenType.Bleeding:
                return new BleedingToken(this, Count);
            case TokenType.Weakening:
                return new WeakeningToken(this, Count);
            case TokenType.Poison:
                return new PoisonToken(this, Count);
            case TokenType.Stun:
                return new StunToken(this, Count);
                default: return null;
              
        }
    }
    public void RemoveToken(Token token)
    {
        token.Remove();
        _tokenLists[token.activeTime].Remove(token);
    }
    
    public virtual void RealTokenActive(Token token)
    {
        token.Active();
    }
    public virtual void ActiveToken(ActiveTime activeTime)
    {
        if(_tokenLists[activeTime].Count!= 0)
        {
            foreach (Token token in _tokenLists[activeTime])
            {
                RealTokenActive(token);
            }
            for(int i = _tokenLists[activeTime].Count-1; i>=0; i--)
            {
                if (_tokenLists[activeTime][i].Count == 0)
                {
                    RemoveToken(_tokenLists[activeTime][i]);
                }
            }
           
        }
    }
    public void CountDownToken(ActiveTime activeTime)
    {
        if (_tokenLists[activeTime].Count != 0)
        {
            foreach (Token token in _tokenLists[activeTime])
            {
                token.Count--;

            }
            for (int i = _tokenLists[activeTime].Count - 1; i >= 0; i--)
            {
                if (_tokenLists[activeTime][i].Count == 0)
                {
                    RemoveToken(_tokenLists[activeTime][i]);
                }
            }

        }
    }
    #endregion

    /// <summary>
    /// 유닛의 데이터 초기화
    /// </summary>
    /// <param name="id">해당 유닛의 아이디</param>
    public virtual void BattleDataInit(int id = -1, PlayerStats playerStats = null)
    {
        _tokenLists = new Dictionary<ActiveTime, List<Token>>()
        {
            { ActiveTime.Attack,new List<Token>()},
            { ActiveTime.TakeDamage,new List<Token>()},
            { ActiveTime.Immediately,new List<Token>()},
            { ActiveTime.EnemyTurn,new List<Token>()},
            { ActiveTime.TurnStart,new List<Token>()},
            { ActiveTime.MyTurnStart,new List<Token>()},
        };
        nowpos = this.transform.position;
        _audioSource = this.GetComponent<AudioSource>();
        if (this.GetComponentInChildren<Animator>()&& CharacterEventListener==null)
        {
            CharacterEventListener = new CharacterEventListener(this.GetComponentInChildren<Animator>());
        }
        else
        {
            CharacterEventListener = new CharacterEventListener(this.GetComponent<Animator>());
        }
        Invoke("Taunt", 1f);

        FXList[7].GetComponent<ParticleSystem>().Stop();
    }

    /// <summary>
    /// 전투입장 애니메이션 실행
    /// </summary>
    private void Taunt()
    {
        CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Taunt);
    }

    /// <summary>
    /// 최종 데미지 주는 함수
    /// </summary>
    /// <param name="targetunit">타겟유닛</param>
    /// <param name="damage">최종 데미지</param>
    public void TakeDamage(BattleUnit targetunit, int damage, bool isCry, bool isToken =false)
    {
        if (damage == 0)
        {
            Managers.Floating.FloatingText("회피!", targetunit.transform.position);
            targetunit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Avoid);
        }
        else if (isCry)
        {
            
            Managers.Floating.FloatingText("치명타!\n" + damage.ToString(), targetunit.transform.position, true);
            targetunit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.TakeHeavyDamage);
            targetunit.FXList[7].GetComponent<ParticleSystem>().Play();
            if (targetunit != this)
            {
                targetunit.FXList[9].GetComponent<ParticleSystem>().Play();
                Managers.Camera.ShakeCamera(5f, 3f, 0.7f);
            }
            if (isToken != true && OnSkill != null && OnSkill.TokenType != TokenType.None)
            {
                targetunit.PutToken(OnSkill.TokenType, OnSkill.TokenCount);
            }
           
        }
        else
        {
           
            Managers.Floating.FloatingText("-"+damage.ToString(), targetunit.transform.position);
            targetunit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.TakeSmallDamage);
            targetunit.FXList[7].GetComponent<ParticleSystem>().Play();
            if (targetunit != this)
            {
                targetunit.FXList[9].GetComponent<ParticleSystem>().Play();
                Managers.Camera.ShakeCamera(3f, 2f, 0.5f);
            }

            if (isToken != true && OnSkill !=null&& OnSkill.TokenType != TokenType.None)
            {
                targetunit.PutToken(OnSkill.TokenType, OnSkill.TokenCount);
            }
        } 
        
        StartCoroutine(Damaged(targetunit, damage));

    }
    IEnumerator Damaged(BattleUnit targetunit, int damage)
    {
        if (targetunit.CompareTag("Enemy"))
        {
            UI_EnemyInfo uI_EnemyInfos = Managers.Battle.UI_Battle.uI_EnemyInfos[targetunit.EnemyIndex];
            for (int i = 0; i < damage; i++)
            {
                targetunit.CurHp -= 1;
                /*if (targetunit.CompareTag("Enemy"))
                {
                    uI_EnemyInfos.SetHpSlder(targetunit);
                }*/
                uI_EnemyInfos.SetHpSlder(targetunit);
                if (targetunit.CurHp <= 0)
                {
                    targetunit.CurHp = 0;
                    Managers.Battle.Die(targetunit);
                    break;
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            targetunit.CurHp -= damage;
            targetunit.GetComponent<PlayerBattle>().PlayerStat.UpdateCurrentHealth(targetunit.CurHp);
            targetunit.FXList[9].GetComponent<ParticleSystem>().Play();
            if (targetunit.CurHp <= 0)
            {
                targetunit.CurHp = 0;
                Managers.Battle.Die(targetunit);
            }


        }

    }

    /// <summary>
    /// 오브젝트 파괴하는 함수
    /// </summary>
    public void Destroyed()
    {
        if (this.CompareTag("Enemy"))
        {
            StartCoroutine(DestroyedCo());
        }
    }
    IEnumerator DestroyedCo()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void Takeheal(BattleUnit targetunit,int heal)
    {
        Managers.Floating.FloatingText("+" + heal.ToString(), targetunit.transform.position);
        targetunit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Victory);
        targetunit.FXList[10].GetComponent<ParticleSystem>().Play();
        targetunit.CurHp += heal;
        targetunit.GetComponent<PlayerBattle>().PlayerStat.UpdateCurrentHealth(targetunit.CurHp);
    }

    /// <summary>
    /// 최종 데미지 계산하는 함수
    /// </summary>
    public void Fight()
    {

        Tuple<int, int, bool> result;
        List<BattleUnit> targets;
      
        int finalDamge;
        if (gameObject.CompareTag("Player"))
        {
            result = BattleUtil.SetBattleData(SlotData.Fail, SlotData.Yel, OnSkill.Rolls, data.Att, true);
            targets = new List<BattleUnit>(Managers.Battle.EnemyList);
        }
        else
        {
            result = BattleUtil.SetBattleData(SlotData.Fail, SlotData.Yel, OnSkill.Rolls, data.Att, false);
            targets = new List<BattleUnit>(Managers.Battle.PlayerList);
        }

        BattleData tar = Target.data;
        
        switch (OnSkill.Target)
        {
            case 0:
                finalDamge = BattleUtil.SetDamage(result.Item1, tar, OnSkill.Type, result.Item2, result.Item3);
                if (OnSkill.Target == 0)
                {
                    if (OnSkill.EnemyEffect != -1)
                    {
                        Target.FXList[OnSkill.EnemyEffect].GetComponent<ParticleSystem>().Play();
                    }
                    TakeDamage(Target, finalDamge, result.Item3);  
                }

                break;
            case 1:

                for (int i = 0; i < targets.Count; i++)
                {
                    if (OnSkill.EnemyEffect != -1)
                    {
                        targets[i].FXList[OnSkill.EnemyEffect].GetComponent<ParticleSystem>().Play();
                    }
                    finalDamge = BattleUtil.SetDamage(result.Item1, tar, OnSkill.Type, result.Item2, result.Item3);
                    TakeDamage(targets[i], finalDamge, result.Item3);

                }
                break;
            case 2:
                break;

        }


    }

    #region 사운드 함수
    public virtual void AttackSound()
    {

    }
    public virtual void TakeDamageSound()
    {
        
    }
    public virtual void TakeDamageHeavySound()
    {
        
    }
    public virtual void TauntSound()
    {

    }
    public virtual void AvoidSound()
    {

    }
    public virtual void DeadSound()
    {

    }
    public virtual void ReviveSound()
    {

    }
    #endregion
    
}


