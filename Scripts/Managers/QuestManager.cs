using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    private bool _isnpc = true;
    public UI_Quest ui_Quest;
   
    public Dictionary<int, Quest> Quests;
    public List<Quest> ActiveQuest = new List<Quest>();
    List<int> _town1;
    List<int> _town2;
    List<int> _town3;
    List<int> _town4;
    List<int> _town5;
    int _talkIndex = 0;
    public PlayerStats player;
    public void Init()
    {
        ui_Quest = Managers.UIManager.EventPopUp.UI_Quest;
        Quests = new Dictionary<int, Quest>()
        {
            {1,new ListenQuest(1,QuestType.Main,Town.Town1,"���� ����","����",true,new int[]{},new string []{
                "ī���ٿ� �� �� ȯ���ϳ�, ���� �뺴. ���� ���������� ���Կ뺴���� �ʿ��� ��ǰ ������ ����ϰ� ����.",
                "���⸦ ���Ѵٸ� ���� ����, ���� ��ȣ���� ��ȣ���� ���Ѵٸ� �� ����. �ڳ��� �Ƿڸ� �� ���� ������� Ư���� �����۵��� Ư�����ο��� ������.",
                "�Ƹ� �ڳ� ������ ��ž�! �츮 ���� ���ε� �ؾ��� �����ŵ�. ����! ���� ������ ���µ��� �����ϰ�. ī������ ���� �Ϲ� ���µ���� �޶�."
            }, new Reward[]{new Reward(24)},"<b><color=#FBB060>���� ����</color></b>�� ��ȭ�� ��������")},
            {2,new KillQuest(2,QuestType.Sub,Town.Town1,"���� ����","����� Ȯ��",false,new int[]{1},new string []{
                "��. Ȥ�� �ٻ��� �ʴٸ� �� �Ƿڸ� ������ ������ �ֳ�? ������ ��������.",
                "����. �˴ٽ��� �츮 ������ �ֹ� ��κ��� �Ƹ��ɿ¿��� �ڱ���� ������ ����ǰ���� �����ϴ� ������ �԰����.",
                "��� �� �⵿�� ����� �ٴϴ� ���� �������ұ� ������ ���̻罽 ���￡ �з��� �����µ��� �����ϸ� �����δ� �����ߴ� ������.",
                "�׷��� ������ ���������� �𸣰� ���͵��� ������ �޴� ���� ������ �������. ������ �츮�� ����.",
                "������ �ڳ״� �츮 �������� ��� �뵵�ø� ���� �� ����. �����ٸ� ���� �濡 ���̴� ���͵��� �� óġ�������� ���ھ�. ��Ź�ϳ�!"
            },new Reward[]{new Reward(80)}, "<b><color=#FBB060>�ֺ� ����</color></b>�� óġ�ϰ� ����θ� Ȯ���ϼ���")},
            {3,new MoveQuest(3,QuestType.Main,Town.Town2,"����","���� ������",false,new int[]{1,2},new string []{
                "��! ��ī������Ʈ�� ���� ���̶��?! ��!","player",
                "�ֻ�� �뺴�� �� ��ǥ�Դϴ�." ,"npc",
                "Ȯ���� �ڳװ��� �뺴�� �ֻ�� �뺴�� �ȵȴٸ� ���� �ɱ� �ͱ�� ����! ������. ���� ������!",
                "��ħ ��ī������Ʈ�� �� ����̸� ������ �ϰŵ�. ī������ �� ���ؼ� �� ������� ���� �ֱ������� �ʿ��ؼ� ������. �Ƹ��ɿ´��� �������. �η��� ���� ����϶��.. ",
                "������� �ʳ�? ��� �ΰ��� �� ����̸� �ٸ� ������ �̿��� ������ ����. �׷��� �ϸ� �ȵȴٴ� ���� ���µ� ���̾�.",
                "��, �⼳�� �������. �׵��� �츮 ������ �Ƿڸ� ����.. ����. ���� �������� ��Ź�� �־�����, ������ ���� �� �ƴϰڳ�. �ƹ�ư �� ����̸� ��ī������Ʈ�� ������ �ְ�!","player",
                "(���� ���ֶ� ô�� ���� ���� �ʴ�. ���� �ٽ� ������ �𸣴�. �׳� �Ƿڸ� �޾ƾ� �ڴ�.)",
                "...�� �˰ڽ��ϴ�, ���ִ�."
            }, new Reward[]{new Reward(24)},"<b><color=#FBB060>��ī������Ʈ</color></b>�� �̵��ϼ���")},
            {4,new KillQuest(2,QuestType.Sub,Town.Town2,"ǥ����","��������� ��Ź",false,new int[]{2},new string []{
               "-���� ���ۿ� �ʿ��� ��Ḧ ���� �Ƿ� �մϴ�.-"
            },new Reward[]{new Reward(80)}, "<b><color=#FBB060>�ֺ� ����</color></b>�� ���⸦ ȹ���ϼ���")},
            {5,new KillQuest(2,QuestType.Sub,Town.Town2,"����","�� �� ��ƶ�!",false,new int[]{2},new string []{
               "��, �̺�! �ű� �뺴! ȣ..ȣȣ..ȣ��, Ȥ�� �Ƿڸ� ���� ��������?! ��?!",
               "�׷��׷�, �뺴�̸� �Ƿڸ� �޾ƾ���!! �� ����� �Ұ� �Ƴ�? �Ⱦ ��!","player",
               "(������ ���ֿ� ���� ���踦 ������ ������ �� ������ ������ �ȴ�. �ƹ��� �׷��� �׷���..)","npc",
               "��, �� ��! ����! ���־־־�!","player",
               "(...�̾߱⳪ ����.)",
               "��Ȯ�� �Ƿ� ������ ������ ���� ���ֽʽÿ�.","npc",
               "����. �� ��� ü����..����! �ٸ��� �ƴϾ�! ���� ������ �ִ� �������� ���ϳ��� �������� ���İ��� ���̾�! ���� ã����!",
               "�Ƹ� �������̸� ���� ������ �������� �ž�! ���ѷ�! ����! ��������!!"
            },new Reward[]{new Reward(32)}, "<b><color=#FBB060>�ֺ� ����</color></b>�� óġ�ϼ���")},
        };
        _town1 = new List<int>() { 1, 2 };
        _town2 = new List<int>() { 3,4,5 };
    }
    
    /// <summary>
    /// ����Ʈ ����
    /// </summary>
    /// <param name="quest"></param>
    public void AcceptQuest(int id)
    {
        Quest quest = Quests[id];
        ActiveQuest.Add(quest);
        ui_Quest.SetQuestChart();
        if(quest is KillQuest)
        {
            Managers.WorldMapGenerator.SetQuestZone(id, (int)quest.StartTown, true, BattleZoneType.Camp);
        }
        else if(quest is MoveQuest)
        {
            Managers.WorldMapGenerator.SetQuestZone(id, (int)quest.StartTown, false);
        }
       
    }

    /// <summary>
    /// npc�� ��ȭ
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public Tuple<string,bool> playTalk(int id)
    {
        Quest quest = Quests[id];
        
        string talk ="";
        if (_talkIndex == quest.Talk.Length)
        {
            _talkIndex = 0;
            talk = null;
            if(quest is ListenQuest)
            {
                ClearQuest(id);
            }
        }
        else if (_talkIndex == quest.Talk.Length - 1 && quest is not ListenQuest)
        {
            talk= quest.Talk[_talkIndex++] + "\r\n\n<b>����Ʈ ��ǥ</b>\r\n" + quest.Goal;
        }
        else if(quest.Talk[_talkIndex].Equals("player"))
        {
            _talkIndex++;
            talk = quest.Talk[_talkIndex++];
            _isnpc = false;
        }
        else if (quest.Talk[_talkIndex].Equals("npc"))
        {
            _talkIndex++;
            talk = quest.Talk[_talkIndex++];
            _isnpc = true;
        }
        else
        {
            talk = quest.Talk[_talkIndex++];
        }
        return Tuple.Create(talk, _isnpc);

    }

    /// <summary>
    /// �Ϸ�� ����Ʈ ����
    /// </summary>
    /// <param name="quest"></param>
    public void RemoveQuest(int id)
    {
        Quest quest = Quests[id];
        if (ActiveQuest.Contains(quest))
            ActiveQuest.Remove(quest);
        ui_Quest.SetQuestChart();
    }

    /// <summary>
    /// Ŭ����üũ�Լ�
    /// </summary>
    /// <param name="quest"></param>
    public void ClearQuest(int id)
    {
        Quest quest = Quests[id];
        GiveReward(id);
        RemoveQuest(id);
        SetQuestTrue(id);
    }

    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    /// <param name="quest"></param>
    public void GiveReward(int id)
    {
        Quest quest = Quests[id];
        for (int i = 0; i < quest.Reward.Length; i++)
        {
            if (quest.Reward[i].IsCoin)
            {
                int count = Managers.Instance.GetPlayerCount;
                Item coin = quest.Reward[i].GiveItem();
                for(int j =0; j < count; j++)
                {
                    Managers.Instance.GetPlayer(j).PlayerStats.UpdateGold(coin.ItemStock/ count);
                }
            }
            else
            {
                Item item = quest.Reward[i].GiveItem();
                player.UpdateItem(item);
            }

        }
    }

    public void SetQuestTrue(int id)
    {
        for (int index = 1; index < Quests.Count; index++)
        {
            if(id == index) { continue; }
            Quests[index].SetTrue(id);
        }
    }

    public List<int> GetTownQuest(Town town)
    {
        switch (town)
        {
            case Town.Town1:
                return _town1;
            case Town.Town2:
                return _town2;
            case Town.Town3:
                return _town3;
            case Town.Town4:
                return _town4;
            case Town.Town5:
                return _town5;
            default: return null;
        }
    }
    public void DeleteTownQuest(Town town, int id)
    {
        switch (town)
        {
            case Town.Town1:
                 _town1.Remove(id);
                break;
            case Town.Town2:
                 _town2.Remove(id);
                break;
            case Town.Town3:
                 _town3.Remove(id);
                break;
            case Town.Town4:
                 _town4.Remove(id);
                break;
            case Town.Town5:
                 _town5.Remove(id);
                break;
            default: return;
        }
    }

  
}
