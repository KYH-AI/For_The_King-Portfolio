using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Main,
    Sub,
}
public enum Town
{
    Town1,
    Town2,
    Town3,
    Town4,
    Town5,
}


public class Quest 
{
    protected int _id;
    protected QuestType _type;
    protected Reward[] _reward;
    protected string _title;
    protected Town _startTown;
    protected Town _completeTown;
    protected bool _canStart = false;
    protected string[] _talk;
    protected string _goal;
    protected string _npc;
    public int TalkIndex = 0;
    public List<int> BeforeQuest;
    protected int _xp;

    public int ID => _id;
    public QuestType Type => _type;
    public string Title => _title;
    public Town StartTown => _startTown;
    public Town CompleteTown => _completeTown;
    public bool CanStart => _canStart;
    public string[] Talk => _talk;
    public string Goal => _goal;
    public string NPC => _npc;
    public Reward[] Reward => _reward;
    public Quest(int id, QuestType type, Town startTown, string npc,string title, bool canStart, int[] beforeQuest,string[] talk, Reward[] reward, string goal = null)
    {
        _id = id;
        _startTown = startTown;
        _type = type;
        _title = title;
        _npc = npc; 
        _canStart = canStart;
        _talk = talk;
        _reward = reward;
        _goal = goal;
        BeforeQuest = new List<int>(beforeQuest);
    }

    public void SetTrue(int id)
    {
        if (BeforeQuest.Contains(id))
        {
            BeforeQuest.Remove(id);
        }
        if(BeforeQuest.Count == 0)
        {
            _canStart =true;
        }
    }
}
