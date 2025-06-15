using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Quest : UI_PopUp
{
    Sprite PlayerPortrait;
    Sprite NpcPortrait;
    List<int> QuestIds;
    private UI_QuestChart uI_QuestChart;
    GameObject go;
    UI_PointerEventHandler evt;
    int _nowClickList =-1;
    Town town;
    enum QuestTexts
    {
        messageDisplay,
        name,
        LocationName,
        Quest1Title,
        Quest2Title,
        Quest3Title,
        Quest4Title,
        Quest1Reward,  
        Quest2Reward,
        Quest3Reward,
        Quest4Reward,
        Quest1Text,
        Quest2Text,
        Quest3Text,
        Quest4Text,
    }

    enum QuestImages
    {
        Portrait,
    }
    enum QuestGO
    {
        UI_QuestHud,
        MessagePanel,
        UI_Questboard,
        Quest1Parent,
        Quest2Parent,
        Quest3Parent,
        Quest4Parent,
        Quest1Detail,
        Quest2Detail,
        Quest3Detail,
        Quest4Detail,
        NoMoreQuest,
    }
    enum QuestButton
    {
        Quest1,
        Quest2,
        Quest3,
        Quest4,
        Quest1Button,
        Quest2Button,
        Quest3Button,
        Quest4Button,
        closeButton
    }

    public override void Init()
    {
        uI_QuestChart = Managers.UIManager.GetQuestChart();
        uI_QuestChart.Init();
        town = Town.Town1;
        Bind<TextMeshProUGUI>(typeof(QuestTexts));
        Bind<Image>(typeof(QuestImages));
        Bind<GameObject>(typeof(QuestGO));
        Bind<Button>(typeof(QuestButton));
        for(int i = 0; i < 4; i++)
        {
            uI_QuestChart.GetQuestChart(i).gameObject.SetActive(false);
        }
        go = Get<Button>((int)QuestButton.closeButton).gameObject;
        evt = go.GetComponent<UI_PointerEventHandler>();
        evt.OnClickHandler = null;
        evt.OnClickHandler += ((PointerEventData data) =>
        {
            CloseQuestBoard();
        });

    }

    public void NextTalk(int id)
    {
        Tuple<string, bool> tuple = Managers.Quest.playTalk(id);
        string talk = tuple.Item1;
        bool isNpc = tuple.Item2;
        if (talk == null)
        {
            Managers.Quest.DeleteTownQuest(town, id);
            Get<GameObject>((int)QuestGO.UI_QuestHud).gameObject.SetActive(false);
            Get<GameObject>((int)QuestGO.UI_Questboard).gameObject.SetActive(true);
            ShowBoardAfterTalk();
        }
        if (!isNpc)
        {
            Get<Image>((int)QuestImages.Portrait).sprite = PlayerPortrait;
        }
        else
        {
            Get<Image>((int)QuestImages.Portrait).sprite = NpcPortrait;
        }
        Get<TextMeshProUGUI>((int)QuestTexts.messageDisplay).text = talk;
      
    }

    public void Event()
    {
        
        for(int i = 0; i < 4; i++)
        {
            go = Get<Button>(i).gameObject;
            evt = go.GetComponent<UI_PointerEventHandler>();
            evt.OnClickHandler = null;
            switch (i)
            {
                case 0:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickList(0);
                    });
                    break;
                case 1:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickList(1);
                    });
                    break;
                case 2:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickList(2);
                    });
                    break;
                case 3:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickList(3);
                    });
                    break;

            }
           
        }

        for (int i = 0; i < 4; i++)
        {
            go = Get<Button>(i+4).gameObject;
            evt = go.GetComponent<UI_PointerEventHandler>();
            evt.OnClickHandler = null;
            switch (i)
            {
                case 0:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickAccept(0);  
                    });
                    break;
                case 1:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickAccept(1);
                    });
                    break;
                case 2:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickAccept(2);
                    });
                    break;
                case 3:
                    evt.OnClickHandler += ((PointerEventData data) =>
                    {
                        ClickAccept(3);
                    });
                    break;

            }

        }
    }

    public void ShowTalkWindow(int id)
    {
        PlayerPortrait = Managers.Quest.player.PlayerPortrait;
        NpcPortrait = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.PortraitNpc, Managers.Quest.Quests[id].StartTown.ToString());
        Get<GameObject>((int)QuestGO.UI_QuestHud).gameObject.SetActive(true);
        Get<TextMeshProUGUI>((int)QuestTexts.name).text = Managers.Quest.Quests[id].NPC;
        NextTalk(id);
    }

    public void ShowQuestBoard()
    {
        _nowClickList = -1;
        Get<TextMeshProUGUI>((int)QuestTexts.LocationName).text = town.ToString();
        QuestIds = Managers.Quest.GetTownQuest(town);
        ShowQuestList(QuestIds.Count);   
        for (int index = 0; index < 4; index++)
        {
            Get<GameObject>(index + 7).SetActive(false);
        }
        Event();
        base.Init();
    }
    public void ShowBoardAfterTalk()
    {
        _nowClickList = -1;
        Get<TextMeshProUGUI>((int)QuestTexts.LocationName).text = town.ToString();
        QuestIds = Managers.Quest.GetTownQuest(town);
        ShowQuestList(QuestIds.Count);
        for (int index = 0; index < 4; index++)
        {
            Get<GameObject>(index + 7).SetActive(false);
        }
        Event();
    }
    public void CloseQuestBoard()
    {
        base.ClosedPopUpUI();
        Managers.UIManager.ShowMainItemCardUI(null, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget,false, true);
    }

    private void ShowQuestList(int Count)
    {
        if(Count == 0)
        {
            Get<GameObject>((int)QuestGO.Quest1Parent).SetActive(false);
            Get<GameObject>((int)QuestGO.Quest2Parent).SetActive(false);
            Get<GameObject>((int)QuestGO.Quest3Parent).SetActive(false);
            Get<GameObject>((int)QuestGO.Quest4Parent).SetActive(false);
            Get<GameObject>((int)QuestGO.NoMoreQuest).SetActive(true);
        }
        else
        {
            Get<GameObject>((int)QuestGO.NoMoreQuest).SetActive(false);
            for (int i= 0; i < Count; i++)
            {
                Get<GameObject>(i+3).SetActive(true);
                SetTextList(i);
            }
            for (int i = Count; i < 4; i++)
            {
                Get<GameObject>(i + 3).SetActive(false);
            }
        }
    }
    /// <summary>
    /// 퀘스트 목록 텍스트 맵핑
    /// </summary>
    private void SetTextList(int i)
    {
        Quest quest = Managers.Quest.Quests[QuestIds[i]];
        Get<TextMeshProUGUI>(i+3).text = "["+ quest .Type.ToString()+ "] " + quest.ID + "."+ quest.Title;
        if (quest.Reward[0].Type == Item.ItemType.GoldCoin)
        {
            Get<TextMeshProUGUI>(i + 7).text = quest.Reward[0].GiveItem().ItemStock+ "gold";
        }
        else
        {
            Get<Text>(i + 7).text = "아이템";
        }
        Get<TextMeshProUGUI>(i + 11).text = "<b>퀘스트 목표</b>\r\n" + quest.Goal+ "\r\n";
        if (!quest.CanStart)
        {
            Get<Button>(i + 4).gameObject.SetActive(false);
            Get<TextMeshProUGUI>(i + 11).text += "\r\n<b>미완료된 선행퀘스트 </b>\r\n";
            for (int j = 0; j < quest.BeforeQuest.Count; j++)
            {
                Quest beforequest = Managers.Quest.Quests[quest.BeforeQuest[j]];
                if (j == 0)
                {
                    Get<TextMeshProUGUI>(i + 11).text += beforequest.ID + "." + beforequest.Title;
                }
                else
                {
                    Get<TextMeshProUGUI>(i + 11).text += " & " + beforequest.ID + "." + beforequest.Title;
                }

            }
        }
        else
        {
            Get<Button>(i + 4).gameObject.SetActive(true);
            Get<TextMeshProUGUI>(i + 11).text += "\r\n\r\n";
        }
        
    }

    private void ClickList(int i)
    {
        Item item = GetReward(i);
        if (_nowClickList != i)
        {
            _nowClickList = i;
            for(int index =0; index<4; index++)
            {
                Get<GameObject>(index + 7).SetActive(false);
            }
            Get<GameObject>(i+7).SetActive(true);
            Managers.UIManager.ShowMainItemCardUI(item, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, true,true);
        }
        else
        {
            Get<GameObject>(i + 7).SetActive(false);
            _nowClickList = -1;
            Managers.UIManager.ShowMainItemCardUI(item, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, false, true);
        }
    }

    private void ClickAccept(int index)
    {
        if(index >= QuestIds.Count) return;
        Managers.Quest.AcceptQuest(QuestIds[index]);
        go = Get<GameObject>((int)QuestGO.MessagePanel);
        evt = go.GetComponent<UI_PointerEventHandler>();
        evt.OnClickHandler = null;
        evt.OnClickHandler += ((PointerEventData data) =>
        {
            NextTalk(QuestIds[index]);
        });
        Get<GameObject>((int)QuestGO.UI_Questboard).SetActive(false);
        Managers.UIManager.ShowMainItemCardUI(null, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, false, true);
        ShowTalkWindow(QuestIds[index]);
    }

    public void SetQuestChart()
    {
        for (int i = 0; i < 4; i++)
        {
            uI_QuestChart.GetQuestChart(i).gameObject.SetActive(false);
        }
        List<Quest> acceptquests = Managers.Quest.ActiveQuest;
        for(int i = 0; i < acceptquests.Count; i++)
        {
            uI_QuestChart.GetQuestChart(i).gameObject.SetActive(true);
            uI_QuestChart.GetQuestChart(i).text = acceptquests[i].Goal;
            if (acceptquests[i].Reward[0].Type == Item.ItemType.GoldCoin)
            {
                uI_QuestChart.GetQuestChart(i).text += "(" + acceptquests[i].Reward[0].GiveItem().ItemStock + "gold)";
            }
            else
            {
                uI_QuestChart.GetQuestChart(i).text += "(아이템)";
            }
        }
        
    }
    public Item GetReward(int i)
    {
        Quest quest = Managers.Quest.Quests[QuestIds[i]];
        if(quest.Reward[0].Type == Item.ItemType.GoldCoin)
        {
            return quest.Reward[0].GiveItem();
        }
        else{
            return quest.Reward[0].GiveItem();
        }

    }
}
