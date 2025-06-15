using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat 
{
    
    private int _enemyID;
    public Sprite EnemyIcon;
    private int _enemyLevel;
    private string _enemyName;

    private int _enemyType;
    private int _enemyAttakType;

    private int _enemyHealth;
    private int _enemyArmor;
    private int _enemyRes;
    private int _enemyAvoid;
    private int _enemyAttck;
    private int _enemySpeed;
    private int _enemyAcc;
    private int _enemyCr;
    private List<int> _skills;

    public int EnemyID => _enemyID;

    public int EnemyLevel => _enemyLevel;
    public string EnemyName => _enemyName;
    public int EnemyHealth => _enemyHealth;
    public int EnemyArmor => _enemyArmor;
    public int enemyRes => _enemyRes;
    public int EnemyAvoid => _enemyAvoid;
    public int EnemyAttck => _enemyAttck;
    public int EnemySpeed => _enemySpeed;
    public int EnemyAcc => _enemyAcc;
    public int EemyCr => _enemyCr;
    public List<int> Skills => _skills;


    public EnemyStat(int enemyID, string enemyName,int enemyLevel, int enemyType, int enemyAttakType, int enemyHealth, int enemyAttck, int enemySpeed, int enemyArmor, int enemyRes, int enemyAcc, int enemyAvoid,  int enemyCr, int skill1, int skill2 = -1, int skill3 = -1)
    {
        _enemyID = enemyID;
        _enemyLevel = enemyLevel;
        _enemyName = enemyName;
        _enemyType = enemyType;
        _enemyAttakType = enemyAttakType;
        _enemyHealth = enemyHealth;
        _enemyArmor = enemyArmor;
        _enemyRes = enemyRes;
        _enemyAvoid = enemyAvoid;
        _enemyAttck = enemyAttck;
        _enemySpeed = enemySpeed;
        _enemyAcc = enemyAcc;
        _enemyCr = enemyCr;
        int[] temp = { skill1, skill2, skill3 };
        _skills = new List<int>(temp.Length);
        for(int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != -1)
            {
                _skills.Add(temp[i]);
            }
        }
    }
}
