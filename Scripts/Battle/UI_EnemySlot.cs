using System.Collections;
using UnityEngine;


public class UI_EnemySlot : UI_PopUp
{
    public UI_Battle UI_Battle;
    int _maxSlot = 6;
    
    slotCount slotData;
    
    public override void Init()
    {
        this.gameObject.SetActive(true);
        SlotDataInit();

    }
    private void SlotDataInit()
    {
        slotData.Clear = 0;
        slotData.Yel = 0;
        slotData.Fail = 0;
    }
    public void SetSlot(int Slot_count, int skillType)
    {
        for (int i = 6; i < _maxSlot+6; i++)
        {
            UI_Battle.GetImage(i).gameObject.SetActive(true);
            UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.INACT][0];
        }
        for (int i = Slot_count; i < _maxSlot; i++)
        {
            UI_Battle.GetImage(i+6).gameObject.SetActive(false);
        }
    }

    public void ClosedPopUp()
    {
        this.gameObject.SetActive(false);
    }
    
    IEnumerator RandomEnemySlot(BattleUnit Enemy, Skill onSkill)
    {
        yield return new WaitForSeconds(1f);
        if (StatRoll.RandomFunc(Enemy.data.Cr+ onSkill.Cry))
        {
            for (int i = 6; i < onSkill.Rolls + 6; i++)
            {
                UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.YEL][0];
                slotData.Yel++;
                Managers.Sound.PlaySFX("Slot_clear");
                yield return new WaitForSeconds(0.4f);
            }
             
        }
        else
        {
            int FinalSlotChance = onSkill.Acc + Enemy.data.Acc;
            for (int i = 6; i < onSkill.Rolls + 6; i++)
            {
                if (StatRoll.RandomFunc(FinalSlotChance))
                {
                    UI_Battle.GetImage(i).sprite = UI_Battle.IconDic[ICONTYPE.CLEAR][0];
                    Managers.Sound.PlaySFX("Slot_clear");
                    slotData.Clear++;
                }
                else
                {
                    UI_Battle.GetImage(i + 12).sprite = UI_Battle.IconDic[ICONTYPE.FAIL][1];
                    Managers.Sound.PlaySFX("slot_fail");
                    slotData.Fail++;
                }

                yield return new WaitForSeconds(0.4f);
            }
        }
        
        Enemy.SlotData = slotData;
        Enemy.IsAttack = true;
        for (int i = 6; i < onSkill.Rolls + 6; i++)
        {
            UI_Battle.GetImage(i + 12).sprite = UI_Battle.IconDic[ICONTYPE.FAIL][0];
        }
        ClosedPopUp();
    }

    public void SetRandomEnemySlot(BattleUnit unit, Skill onSkill)
    {
        
        StartCoroutine(RandomEnemySlot(unit, onSkill));
    }

}
