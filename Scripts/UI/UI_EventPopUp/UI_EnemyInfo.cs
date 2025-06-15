using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyInfo : UI_PopUp
{
    //[HideInInspector]
    public EnemyBattle enemy;
    int TokenIndex =0;
    public enum Images
    {
        portrait_img,//적 초상화
        Armor,//적 아머 이미지
        Registance,//적 마저 이미지
        Token1,
        Token2,
        Token3,
        Token4,
        Token5,
        Token6,
        Token7,
        Token8,
        Status1,
        Status2,
        Status3,
        Status4,
        Status5,
        Status6,
        Status7,
        Status8,
    }
    public enum Texts
    {
        EnemyName,//적 이름
        HpNum,//적 체력
        Level_num,//적 레벨
        Ar,//적 아머
        Res, //적 마저
        TokenCount1,
        TokenCount2,
        TokenCount3,
        TokenCount4,
        TokenCount5,
        TokenCount6,
        TokenCount7,
        TokenCount8,
    }
    public enum Sliders
    {
        EnemyHpSlider//적 체력바
    }

    public void Bind()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Slider>(typeof(Sliders));
    }

    public void SetInfo()
    {
        Get<TextMeshProUGUI>((int)Texts.EnemyName).text = enemy.EnemyStat.EnemyName;
        Get<TextMeshProUGUI>((int)Texts.HpNum).text = enemy.data.Hp + "";
        Get<TextMeshProUGUI>((int)Texts.Level_num).text = enemy.EnemyStat.EnemyLevel + "";
        Get<TextMeshProUGUI>((int)Texts.Ar).text = enemy.data.Ar + "";
        Get<TextMeshProUGUI>((int)Texts.Res).text = enemy.data.Res+ "";
        Get<Image>((int)Images.portrait_img).sprite = enemy.Icon;
        if (enemy.data.Ar == 0)
        {
            Get<Image>((int)Images.Armor).gameObject.SetActive(false);
        }
        if (enemy.data.Res == 0)
        {
            Get<Image>((int)Images.Registance).gameObject.SetActive(false);
        }
        Get<Slider>((int)Sliders.EnemyHpSlider).value = enemy.CurHp / (float)enemy.data.Hp;
        for (int i = 0; i < 8; i++)
        {
            Get<Image>(3 + i).gameObject.SetActive(false);
        }
    }

    public void SetHpSlder(BattleUnit enemy)
    {
        Get<TextMeshProUGUI>((int)Texts.HpNum).text = enemy.CurHp + "";
        Get<Slider>((int)Sliders.EnemyHpSlider).value = enemy.CurHp / (float)enemy.data.Hp;
    }

    public void SetToken(Dictionary<ActiveTime, List<Token>> _tokenDic)
    {
        for(int i=0; i<8; i++)
        {
            Get<Image>(3+i).gameObject.SetActive(false);
        }
        foreach(List<Token> tokens in _tokenDic.Values)
        {
            foreach (Token token in tokens)
            {
                Get<Image>(3 + TokenIndex).gameObject.SetActive(true);
                Get<Image>(3 + TokenIndex).sprite = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.Token, token.tokenType.ToString());
                Get<TextMeshProUGUI>(5 + TokenIndex++).text = token.Count.ToString();
            }
        }
        TokenIndex=0;
    }



}
