using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ICONTYPE
{
    NONE = -1,
    CLEAR,
    YEL,
    FAIL,
    INACT,
}
public class UI_Battle : UI_PopUp
{
    public List<UI_EnemyInfo> uI_EnemyInfos =new();
    private List<Sprite> Clear;
    private List<Sprite> Yel;
    private List<Sprite> Fail;
    private List<Sprite> InActive;
    AudioSource _audioSource;
    

    public Dictionary<ICONTYPE, List<Sprite>> IconDic;
    public enum Texts
    {
        SkillName, //스킬 이름
        SkillDiscription, //스킬 부가설명
        DamageNum, //데미지
        DamageType,//데미지 타입(물리 데미지, 마법 데미지)
        SlotChanceNum, //슬롯당 정확도
        SKillRange, //공격 범위
    }

    public enum Images
    {
        Slot_1,
        Slot_2,
        Slot_3,
        Slot_4,
        Slot_5,
        Slot_6,
        EnemySlot1,
        EnemySlot2,
        EnemySlot3, 
        EnemySlot4,
        EnemySlot5,
        EnemySlot6,
        Fail_1,//12
        Fail_2,
        Fail_3,
        Fail_4,
        Fail_5,
        Fail_6,
        EnemyFail_1,//18
        EnemyFail_2,
        EnemyFail_3,
        EnemyFail_4,
        EnemyFail_5,
        EnemyFail_6,
        UI_FocusUse,
    } //슬롯

    public enum Buttons
    {
        SKillBtn_1, // 스킬1 버튼
        SKillBtn_2, // 스킬2 버튼
        SKillBtn_3, // 스킬3 버튼
        SKillBtn_4, // 스킬4 버튼
        SKillBtn_5, // 스킬5 버튼
    }
    public enum GameObjects
    {
        Damage, //데미지 숫자 및 타입
        SkillBtns,
        Discription,
        EnemyInfo_1,
        EnemyInfo_2,
        EnemyInfo_3,

    }
    public void Bind()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        for (int i = 0; i < 3; i++)
        {
            //uI_EnemyInfos.Add(GetGameObject(i+3).GetComponent<UI_EnemyInfo>());
            uI_EnemyInfos[i].Bind();
        }
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        IconList();
    }
    void IconList()
    {
        Clear = new List<Sprite>()
    {
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Strength_Clear"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Intelligence_Clear"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Awareness_Clear"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Speed_Clear"),
    };
    
        Yel = new List<Sprite>()
    {
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Strength_Yel"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Intelligence_Yel"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Awareness_Yel"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Speed_Yel"),
    };
    
        Fail = new List<Sprite>()
    {
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld,"uiSlotBlank"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld,"uiHitMiss"),
    };
    
        InActive = new List<Sprite>()
    {
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Strength_Inact"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Intelligence_Inact"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Awareness_Inact"),
        Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.StatIconBattle,"Speed_Inact"),
    };
        IconDic = new Dictionary<ICONTYPE, List<Sprite>>()
        {
            {ICONTYPE.CLEAR, Clear},
            {ICONTYPE.YEL, Yel},
            {ICONTYPE.INACT, InActive},
            {ICONTYPE.FAIL, Fail},

        };
    }
    public void SetEnemyInfos()
    {
        for(int i = 0; i < Managers.Battle.EnemyList.Count; i++)
        {
            uI_EnemyInfos[i].SetInfo();
        }
        for (int i = 3 + Managers.Battle.EnemyList.Count; i< 6; i++)
        {
            GetGameObject(i).SetActive(false);
        } 
    }
    public void InitEnemyInfos()
    {
        for (int i = 3; i < 6; i++)
        {
            GetGameObject(i).SetActive(true);
        }
    }
    public Image GetImage(int i)
    {
        return Get<Image>(i);
    }
    public TextMeshProUGUI GetText(int i)
    {
        return Get<TextMeshProUGUI>(i);
    }
    public Button GetButton(int i)
    {
        return Get<Button>(i);
    }
    public GameObject GetGameObject(int i)
    {
        return Get<GameObject>(i);
    }
    /*public void PlaySound(string clip, float volume = 1f)
    {
        AudioClip clip
        _audioSource.PlayOneShot(clip, volume);
    }*/
}
