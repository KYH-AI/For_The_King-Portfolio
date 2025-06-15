using System;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
public class BattleUtil
{
    public static int CryChance(PlayerBattle unit)
    {
        return unit.data.Cr;
    }

    public static int FindStat(PlayerBattle unit)
    {
        /* 23/05/29 무기 스텟 관련 코드 수정 */
        int stat = 0;
        switch (unit.PlayerStat.BaseWeapon.WeaponBasestat)
        {
            case Weapon.WeaponBaseStat.Strength:
                stat = (int)(unit.PlayerStat.Strength * 100);
                break;
            case Weapon.WeaponBaseStat.Intelligence:
                stat = (int)(unit.PlayerStat.Intelligence * 100);
                break;
            case Weapon.WeaponBaseStat.Awareness:
                stat = (int)(unit.PlayerStat.Awareness * 100);
                break;
        }
        return stat;
    }

    

    public static Tuple<int, int, bool> SetBattleData(int FailSlot, int YelSlot, int SlotCount, int Damage, bool isPlayer)
    {
        int finalDamage = Damage;
        float i = (FailSlot / (float)SlotCount);
        int acc;
        if (i == 0&& isPlayer)
        {
            acc = 200;
        }
        else if (i==1)
        {
            acc = 0;
        }
        else
        {
            finalDamage = (int)(finalDamage * (1f - i));
            acc = 100;
        }
        if (YelSlot == SlotCount)
        {
            return Tuple.Create(finalDamage, acc, true); ;
        }
        else { return Tuple.Create(finalDamage, acc, false); }
    }


    public static int SetDamage(int attackDamage, BattleData tar, int damageType, int ACC, bool isCry)
    {
        int FinalDmage = attackDamage;
        if (isCry)
        {
            FinalDmage = (int)(FinalDmage * 1.5);
        }
        else if (!StatRoll.RandomFunc(ACC - tar.Avd))
        {
            FinalDmage = 0;
        }


        if (damageType == 0)
        {
            FinalDmage -= tar.Ar;
        }
        else
        {
            FinalDmage -= tar.Res;
        }
        return Math.Max(FinalDmage, 0);
    }





}
