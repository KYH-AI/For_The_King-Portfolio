using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_BattlePopUp : UI_PopUp
{
    int _maxSlot = 6;
    int _maxSkill = 5;
    readonly Vector3 EnterBtn = new Vector3(1.7f, 1.7f, 1.7f);
    readonly Vector3 ExitBtn = new Vector3(1.1f, 1.1f, 1.1f);
    readonly Vector3 Hover = new Vector3(2, 5);
    [HideInInspector]
    public int SlotOrder = 0;
    int _nowSkill;
    [HideInInspector]
    public bool IsBind = false;
    [HideInInspector]
    public bool IsSkillChange = false;
    public UI_Battle UI_Battle;
    [HideInInspector]
    public Skill Clickskill;
    int _statType;
    [HideInInspector]
    public int WDamage;
    [HideInInspector]
    public int useStat;
    int _Cry;
    
    slotCount slotData;
    PlayerBattle _nowplayer;
    public override void Init()
    {
        this.gameObject.SetActive(true);
        SlotDataInit();
        _nowplayer = Managers.Battle.InPlayer;
        useStat = BattleUtil.FindStat(_nowplayer);
        WDamage = _nowplayer.PlayerStat.BaseWeapon.Damage;
        _Cry = BattleUtil.CryChance(_nowplayer);
        _statType = (int)_nowplayer.PlayerStat.BaseWeapon.WeaponBasestat;
        _nowSkill = 0;
    }
    private void SlotDataInit()
    {
        slotData.Clear = 0;
        slotData.Yel = 0;
        slotData.Fail = 0;
    }

    #region 스킬 UI맵핑
    private void SetTexts(string Skill_name, string Skill_discription, 
         int SKill_range, int Slot_chance_num, int Slot_count, int Damage_num ,  int Damage_type)
    {
        UI_Battle.GetText((int)UI_Battle.Texts.SkillName).text = Skill_name;
        UI_Battle.GetText((int)UI_Battle.Texts.SkillDiscription).text = Skill_discription;
        switch (SKill_range)
        {
            case 0:
                UI_Battle.GetText((int)UI_Battle.Texts.SKillRange).text = "단일 대상";
                break;
            case 1:
                UI_Battle.GetText((int)UI_Battle.Texts.SKillRange).text = "광역 대상";
                break;
            case 2:
                UI_Battle.GetText((int)UI_Battle.Texts.SKillRange).text = "자신에게";
                break;
        }
       
        UI_Battle.GetText((int)UI_Battle.Texts.SlotChanceNum).text = Slot_chance_num +"";
        UI_Battle.GetText((int)UI_Battle.Texts.DamageNum).text = Damage_num + "";
        if (Damage_type == 4)
        {
            UI_Battle.GetGameObject((int)UI_Battle.GameObjects.Damage).gameObject.SetActive(false);
        }
        else { UI_Battle.GetGameObject((int)UI_Battle.GameObjects.Damage).gameObject.SetActive(true); }
        if (Damage_type == 0)
        {
            UI_Battle.GetText((int)UI_Battle.Texts.DamageType).text = "물리데미지";
        }
        else if(Damage_type == 1)
        {
            UI_Battle.GetText((int)UI_Battle.Texts.DamageType).text = "마법데미지";
        }
        for (int i = 0; i < _maxSlot; i++)
        {
            UI_Battle.GetImage(i).gameObject.SetActive(true);
        }
        for (int i = Slot_count; i < _maxSlot; i++)
        {
            UI_Battle.GetImage(i).gameObject.SetActive(false);
        }
    }
    public void SetSkills(List<Skill> skill_list)
    {
        for(int i =0; i < skill_list.Count; i++)
        {
            UI_Battle.GetButton(i).image.sprite= skill_list[i].Icon;
        }
        for (int i = skill_list.Count; i < _maxSkill; i++)
        {
            UI_Battle.GetButton(i).gameObject.SetActive(false);
        }
    }

    public void ChangeText(Skill skill)
    {
        int finalacc = skill.Id ==1 ? skill.Acc + (int)(_nowplayer.PlayerStat.Quickness*100): skill.Acc + useStat;
        SetTexts(skill.Name, skill.Info, skill.Target,
            finalacc, skill.Rolls, 
            WDamage, skill.Type);

        for(int i = 0; i < skill.Rolls; i++)
        {
            if (skill.Id == 0)
            {
                UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.INACT][3];
            }
            else
            {
                UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.INACT][_statType];
            }
            
        }
        Clickskill = skill;
    }
    #endregion
    #region 스킬버튼 이벤트 등록
    public void BtnEvent(List<Skill> skills )
    {

        for(int i = 0; i < skills.Count; i++)
        {
            GameObject go = UI_Battle.GetButton(i).gameObject;  
            UI_PointerEventHandler evt = go.GetComponent<UI_PointerEventHandler>();
            HandlerInit(evt);

            evt.OnExitHandler += ((PointerEventData data) => {
                OnExitBtn(go);
            });
           
            switch (i){
                case 0:
                    evt.OnEnterHandler += ((PointerEventData data) => {
                        OnEnterBtn(skills, go, (int)UI_Battle.Buttons.SKillBtn_1);
                    });
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        OnClickBtn(data, skills[(int)UI_Battle.Buttons.SKillBtn_1]);
                    });
                    break;
                case 1:
                    evt.OnEnterHandler += ((PointerEventData data) => {
                        OnEnterBtn(skills, go, (int)UI_Battle.Buttons.SKillBtn_2);
                    });
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        OnClickBtn(data, skills[(int)UI_Battle.Buttons.SKillBtn_2]);
                    });
                    break;
                case 2:
                    evt.OnEnterHandler += ((PointerEventData data) => {
                        OnEnterBtn(skills, go, (int)UI_Battle.Buttons.SKillBtn_3);
                    });
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        OnClickBtn(data, skills[(int)UI_Battle.Buttons.SKillBtn_3]);
                    });
                    break;
                case 3:
                    evt.OnEnterHandler += ((PointerEventData data) => {
                        OnEnterBtn(skills, go, (int)UI_Battle.Buttons.SKillBtn_4);
                    });
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        OnClickBtn(data, skills[(int)UI_Battle.Buttons.SKillBtn_4]);
                    });
                    break;
                case 4:
                    evt.OnEnterHandler += ((PointerEventData data) => {
                        OnEnterBtn(skills, go, (int)UI_Battle.Buttons.SKillBtn_5);
                    });
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        OnClickBtn(data, skills[(int)UI_Battle.Buttons.SKillBtn_5]);
                    });
                    break;
            }
            
        }
       
    }
    public void HandlerInit(UI_PointerEventHandler evt)
    {
        evt.OnClickHandler = null;
        evt.OnEnterHandler = null;
        evt.OnExitHandler = null;
    }
    public void OnClickBtn(PointerEventData data, Skill skill)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            _nowplayer.IsAttack = true;
            UI_Battle.GetGameObject((int)UI_Battle.GameObjects.SkillBtns).gameObject.SetActive(false);
            UI_Battle.GetGameObject((int)UI_Battle.GameObjects.Discription).gameObject.SetActive(false);
            _nowplayer.OnSkill = Clickskill;
            _Cry += Clickskill.Cry;
            _nowplayer.data.BaseAtt =  WDamage;
            UI_Battle.GetImage((int)UI_Battle.Images.UI_FocusUse).gameObject.SetActive(false);
            _nowplayer.PlayerStat.TempUsedFocus = 0;
        }
        if (data.button == PointerEventData.InputButton.Right)
        {
            if (_nowplayer.PlayerStat.CurrentFocus <= 0) return;
            if (SlotOrder < skill.Rolls)
            {
                if (skill.Id ==0)
                {
                    UI_Battle.GetImage(SlotOrder++).sprite = UI_Battle.IconDic[ICONTYPE.YEL][3];
                }
                else
                {
                    UI_Battle.GetImage(SlotOrder++).sprite = UI_Battle.IconDic[ICONTYPE.YEL][_statType];
                }
                slotData.Yel++;
                _nowplayer.PlayerStat.UpdateCurrentFocus(1, false);


            }
            Managers.Sound.PlaySFX("Focus_Use");

        }
       
    }

    public void OnEnterBtn(List<Skill> skills, GameObject go, int i)
    {
        if (_nowSkill != i)
        {
            _nowSkill = i;
            ChangeText(skills[i]);            
            SlotOrder = 0;
            slotData.Yel = 0;
            _nowplayer.PlayerStat.RefundFocus();
        }
        go.transform.localScale = EnterBtn;
        BattleUnit player = _nowplayer;
        if (skills[i].Target == 1)
        {
            player.FXList[1].SetActive(true);
            player.FXList[0].SetActive(false);
        }
        else
        {
            player.FXList[0].SetActive(true);
            player.FXList[1].SetActive(false);
        }
        IsSkillChange = true;
        UI_Battle.GetImage((int)UI_Battle.Images.UI_FocusUse).gameObject.SetActive(true);
        UI_Battle.GetImage((int)UI_Battle.Images.UI_FocusUse).rectTransform.position = go.GetComponent<Image>().rectTransform.position + Hover;

    }
    public void OnExitBtn(GameObject go)
    {
        go.transform.localScale = ExitBtn;
        UI_Battle.GetImage((int)UI_Battle.Images.UI_FocusUse).gameObject.SetActive(false);

    }
    #endregion

    /// <summary>
    /// 팝업창 닫기
    /// </summary>
    public void ClosedPopUp()
    {
        UI_Battle.GetGameObject((int)UI_Battle.GameObjects.Damage).gameObject.SetActive(true);
        
        UI_Battle.GetGameObject((int)UI_Battle.GameObjects.SkillBtns).gameObject.SetActive(true);
        UI_Battle.GetGameObject((int)UI_Battle.GameObjects.Discription).gameObject.SetActive(true);
        for (int i = 0; i < _maxSlot; i++)
        {
            UI_Battle.GetImage(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < _maxSkill; i++)
        {
            UI_Battle.GetButton(i).gameObject.SetActive(true);
        }
        SlotOrder = 0;
        
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 슬롯 계산 
    /// </summary>
    public void SetRandomSlot(BattleUnit unit)
    {
        StartCoroutine(RandomSlot(unit));
    }
    IEnumerator RandomSlot(BattleUnit player)
    {
        int FinalSlotChance;
        if (Clickskill.Id == 0)
        {
            _statType = 3;
            /*
            if (BattleUtil.RandomFunc(_Cry))
            {
                for (int i = SlotOrder; i < Clickskill.Rolls; i++)
                {
                    UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[UI_Battle.ICONTYPE.YEL][_statType];
                    slotData.Yel++;
                    Managers.Sound.PlaySFX("Slot_clear");
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                
            }
            */
            FinalSlotChance = Clickskill.Acc + (int)(_nowplayer.PlayerStat.Quickness * 100);
            for (int i = SlotOrder; i < Clickskill.Rolls; i++)
            {
                if (StatRoll.RandomFunc(FinalSlotChance))
                {
                    if (StatRoll.RandomFunc(_Cry))
                    {
                        UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.YEL][_statType];
                        slotData.Yel++;
                        Managers.Sound.PlaySFX("Slot_clear");
                    }
                    else
                    {
                        UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.CLEAR][_statType];
                        slotData.Clear++;
                        Managers.Sound.PlaySFX("Slot_clear");
                    }

                }
                else
                {
                    UI_Battle.GetImage(i + 12).sprite = UI_Battle.IconDic[ICONTYPE.FAIL][1];
                    slotData.Fail++;
                    Managers.Sound.PlaySFX("slot_fail");
                }
                yield return new WaitForSeconds(0.5f);
            }

            CanRun(slotData.Clear, slotData.Yel, Clickskill.Rolls);
        }
        else
        {
            if (StatRoll.RandomFunc(_Cry))
            {
                for (int i = SlotOrder; i < Clickskill.Rolls; i++)
                {
                    UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.YEL][_statType];
                    slotData.Yel++;
                    Managers.Sound.PlaySFX("Slot_clear");
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                FinalSlotChance = Clickskill.Acc + useStat;
                for (int i = SlotOrder; i < Clickskill.Rolls; i++)
                {
                    if (StatRoll.RandomFunc(FinalSlotChance))
                    {
                        UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.CLEAR][_statType];
                        slotData.Clear++;
                        Managers.Sound.PlaySFX("Slot_clear");
                    }
                    else
                    {
                        UI_Battle.GetImage(i + 12).sprite = UI_Battle.IconDic[ICONTYPE.FAIL][1];
                        slotData.Fail++;
                        Managers.Sound.PlaySFX("slot_fail");
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
            
            _nowplayer.SlotData = slotData;
            player.IsAttack = true;
        }

        for (int i = SlotOrder; i < Clickskill.Rolls; i++)
        {
            UI_Battle.GetImage(i + 12).sprite = UI_Battle.IconDic[ICONTYPE.FAIL][0];
        }
        
        ClosedPopUp();
    }
    /// <summary>
    /// 도망치기
    /// </summary>
    /// <param name="clear"></param>
    /// <param name="yel"></param>
    /// <param name="slotCount"></param>
    private void CanRun(int clear,int yel,int slotCount)
    {
        int i = (int)(clear +yel / (float)slotCount) * 100;
        if (StatRoll.RandomFunc(i))
        {
            Managers.Floating.FloatingText("도망 성공!!", _nowplayer.transform.position);
            Managers.Battle.Ending(true);

        }
        else
        {
            Managers.Floating.FloatingText("도망 실패!!", _nowplayer.transform.position);
            _nowplayer.IsProgress = false;
            _nowplayer.IsRun = true;
            Managers.Battle.TurnOver();
            
        }
    }

}
