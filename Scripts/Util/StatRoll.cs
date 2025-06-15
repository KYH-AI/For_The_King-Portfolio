using System.Collections.Generic;
using UnityEngine;

public class StatRoll 
{
    public static int Alpha = 5;
    public static Sprite GetSlotIcon(ICONTYPE iCONTYPE,int statType )
    {
        return Managers.Resource.IconDic[iCONTYPE][statType];
    }
    

    public static bool RandomFunc(int percent)
    {
        int result = UnityEngine.Random.Range(1, 101);
        if (result <= percent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool RandomFunc(float percent)
    {
        int i = (int)(percent * 100);
        int result = UnityEngine.Random.Range(1, 101);
        if (result <= i)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    class SuccessCal
    {
        public float persent;
        public float persentSum;
        public int SucessCount;
        public SuccessCal(float persent, float persentSum, int SucessCount)
        {
            this.persent = persent;
            this.persentSum = persentSum;
            this.SucessCount = SucessCount;
        }
    }
    public static List<float> SuccessSlot(PlayerStats player, PlayerStats.StatType slot, int totalCount)
    {
        int i;
        List<float> list = new List<float>();
        for (i = 0; i <= totalCount; i++)
        {
            list.Add(0);
        }
        float percent = player.GetStatValue(slot);
        
        List<SuccessCal> before;
        List<SuccessCal> after = new List<SuccessCal>();
        after.Add(new SuccessCal(percent, percent, 0));
        after.Add(new SuccessCal((percent + (float)Alpha / 100), percent, 1));
        for (i = 1; i < totalCount; i++)
        {
            before = after;
            after = new List<SuccessCal>();
            for (int j = 0; j < (int)UnityEngine.Mathf.Pow(2, i + 1); j++)
            {
                if (j % 2 == 0)
                {
                    after.Add(new SuccessCal(before[j / 2].persent,
                        before[j / 2].persentSum * (1 - before[j / 2].persent),
                        before[j / 2].SucessCount));
                }
                else
                {
                    after.Add(new SuccessCal(Mathf.Min(before[j / 2].persent + (float)Alpha / 100, 1f),
                        before[j / 2].persentSum * before[j / 2].persent,
                        before[j / 2].SucessCount + 1));
                }
            }

        }
        for (i = 0; i < after.Count; i++)
        {
            list[after[i].SucessCount] += (after[i].persentSum * 100);
        }
        return list;
    }
}
