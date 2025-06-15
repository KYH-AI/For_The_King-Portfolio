using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveQuest : Quest
{
    public MoveQuest(int id, QuestType type, Town startTown, string npc, string title, bool canStart, int[] beforeQuest, string[] talk, Reward[] reward, string goal = null) : base(id, type, startTown, npc, title, canStart, beforeQuest, talk, reward, goal)
    {
    }
}
