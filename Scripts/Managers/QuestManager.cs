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
            {1,new ListenQuest(1,QuestType.Main,Town.Town1,"마을 이장","시작",true,new int[]{},new string []{
                "카람바에 온 걸 환영하네, 젊은 용병. 작은 마을이지만 신입용병에게 필요한 물품 정도는 취급하고 있지.",
                "무기를 원한다면 무기 상인, 몸을 보호해줄 보호구를 원한다면 방어구 상인. 자네의 의뢰를 더 쉽게 만들어줄 특수한 아이템들은 특수상인에게 가보게.",
                "아마 자네 마음에 들거야! 우리 마을 장인들 솜씨가 괜찮거든. 껄껄! 마을 주위의 짐승들을 조심하게. 카오스에 물들어서 일반 짐승들과는 달라."
            }, new Reward[]{new Reward(24)},"<b><color=#FBB060>마을 이장</color></b>과 대화를 나누세요")},
            {2,new KillQuest(2,QuestType.Sub,Town.Town1,"마을 이장","상행로 확보",false,new int[]{1},new string []{
                "흠. 혹시 바쁘지 않다면 내 의뢰를 수행할 생각이 있나? 보수는 보장하지.",
                "좋아. 알다시피 우리 마을의 주민 대부분은 아르케온에다 자기들이 생산한 생산품들을 교역하는 것으로 먹고살지.",
                "사실 몇 년동안 사람이 다니는 길을 만들어놓았기 때문에 먹이사슬 경쟁에 밀려난 들짐승들을 제외하면 무역로는 안전했단 말이지.",
                "그런데 요즘들어 무슨일인지 모르게 몬스터들의 습격을 받는 일이 굉장히 잦아졌어. 이유는 우리도 몰라.",
                "어차피 자네는 우리 마을에서 벗어나 대도시를 향해 갈 테지. 괜찮다면 가는 길에 보이는 몬스터들을 좀 처치해줬으면 좋겠어. 부탁하네!"
            },new Reward[]{new Reward(80)}, "<b><color=#FBB060>주변 몬스터</color></b>를 처치하고 상행로를 확보하세요")},
            {3,new MoveQuest(3,QuestType.Main,Town.Town2,"영주","진작 말하지",false,new int[]{1,2},new string []{
                "엥! 스카이하이트로 가는 길이라고?! 왜!","player",
                "최상급 용병이 제 목표입니다." ,"npc",
                "확실히 자네같은 용병이 최상급 용병이 안된다면 누가 될까 싶기는 허이! 허허허. 진작 말하지!",
                "마침 스카이하이트로 이 목걸이를 보내야 하거든. 카오스의 방어를 위해선 이 목걸이의 힘이 주기적으로 필요해서 말이지. 아르케온님의 유지라네. 인류를 위해 사용하라는.. ",
                "대단하지 않나? 어떠한 인간도 이 목걸이를 다른 곳으로 이용할 생각을 안해. 그렇게 하면 안된다는 법도 없는데 말이야.",
                "뭐, 잡설은 여기까지. 그동안 우리 도시의 의뢰를 위해.. 흠흠. 나의 개인적인 부탁도 있었지만, 좋은게 좋은 것 아니겠나. 아무튼 이 목걸이를 스카이하이트에 가져다 주게!","player",
                "(굳이 영주랑 척을 지고 싶진 않다. 언제 다시 만날지 모르니. 그냥 의뢰를 받아야 겠다.)",
                "...잘 알겠습니다, 영주님."
            }, new Reward[]{new Reward(24)},"<b><color=#FBB060>스카이하이트</color></b>로 이동하세요")},
            {4,new KillQuest(2,QuestType.Sub,Town.Town2,"표지판","무기상인의 부탁",false,new int[]{2},new string []{
               "-무기 제작에 필요한 재료를 위해 의뢰 합니다.-"
            },new Reward[]{new Reward(80)}, "<b><color=#FBB060>주변 몬스터</color></b>를 무기를 획득하세요")},
            {5,new KillQuest(2,QuestType.Sub,Town.Town2,"영주","저 놈 잡아라!",false,new int[]{2},new string []{
               "이, 이봐! 거기 용병! 호..호호..호옥, 혹시 의뢰를 받을 생각없나?! 응?!",
               "그래그래, 용병이면 의뢰를 받아야지!! 돈 벌어야 할거 아냐? 싫어도 해!","player",
               "(도시의 영주와 좋은 관계를 쌓으면 앞으로 내 일정에 도움이 된다. 아무리 그래도 그렇지..)","npc",
               "해, 해 줘! 응애! 빼애애애액!","player",
               "(...이야기나 들어보자.)",
               "정확한 의뢰 목적과 보수를 말씀 해주십시오.","npc",
               "흠흠. 내 잠시 체통을..험험! 다른게 아니야! 내가 가지고 있던 보석함을 도둑놈의 새끼들이 훔쳐갔단 말이야! 빨리 찾아줘!",
               "아마 지금쯤이면 도시 밖으로 도망쳤을 거야! 서둘러! 빨리! 빨리빨리!!"
            },new Reward[]{new Reward(32)}, "<b><color=#FBB060>주변 몬스터</color></b>를 처치하세요")},
        };
        _town1 = new List<int>() { 1, 2 };
        _town2 = new List<int>() { 3,4,5 };
    }
    
    /// <summary>
    /// 퀘스트 수락
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
    /// npc와 대화
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
            talk= quest.Talk[_talkIndex++] + "\r\n\n<b>퀘스트 목표</b>\r\n" + quest.Goal;
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
    /// 완료된 퀘스트 삭제
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
    /// 클리어체크함수
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
    /// 보상 수여 함수
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
