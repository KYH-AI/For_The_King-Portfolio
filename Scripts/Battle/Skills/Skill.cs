using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill 
{
    #region ���������
    private int _id;
    private string _name;
    private int _type;//(0:���� 1:����)
    private int _target;//(0:���� �� 1: ��� �� 2:������ �� �翷 50�۵����� 3:���� �Ʊ� 4.��� �Ʊ� 5:�ڽſ���
    private int _rolls;//���԰���
    private int _cry;//ġ��Ÿ ����
    private int _acc;//(���߷� ����) 
    private string _info;
    private string _effect;
    private int _enemyEffect;
    public Sprite Icon;
    public TokenType TokenType;
    public int TokenCount;
    private bool _haveSound;
    public enum Stats
    {
        None = -1,
        Strength,
        Intelligent,
        Aware,
        Speed,
    }
    public enum Targets
    {
        None = -1,
        SingleTarget,
        TargetGroup,
        Splash,
        TargetFriendly,
        TargetParty,
        TargetSelf,
    }
   
    #endregion

    public string Name => _name;
    public int Id => _id;
    public int Type => _type;
    public int Target => _target;
    public int Rolls => _rolls;
    public int Cry => _cry;
    public int Acc => _acc;
    public string Info => _info;
    public string Effect => _effect;
    public int EnemyEffect => _enemyEffect;
    public bool HaveSound => _haveSound;


    #region ������
    public Skill(int id, string name, int acc, int type, int target, int rolls, int cry, string info,bool sound, string effect="",int tokenType = -1, int tokenCount = -1,int EnemyEffect=-1)
    {
        _id = id;
        _name = name;
        _acc = acc;
        _type = type;
        _target = target;
        _rolls = rolls;
        _cry = cry;
        _info = info;
        _effect = effect;
        _enemyEffect = EnemyEffect;
        TokenType = (TokenType)tokenType;
        TokenCount = tokenCount;
        _haveSound = sound;
    }
    #endregion

}
