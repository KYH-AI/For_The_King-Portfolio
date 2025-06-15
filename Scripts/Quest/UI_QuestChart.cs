using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_QuestChart : UI_Base
{
    enum AcceptQuestTexts
    {   
        AcceptQuest1,
        AcceptQuest2,
        AcceptQuest3,
        AcceptQuest4,
    }
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(AcceptQuestTexts));
        for (int i = 0; i < 4; i++)
        {
            Get<TextMeshProUGUI>(i).gameObject.SetActive(false);
        }
    }

    public TextMeshProUGUI GetQuestChart(int i)
    {
        return Get<TextMeshProUGUI>(i);
    }


}
