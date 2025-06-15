using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BattleManager;

public class UI_BattleTimeLineGrid : UI_TimeLine
{
    public UI_Battle UI_Battle;
    public UI_BattlePopUp UI_battlepopup;
    public UI_EnemySlot UI_enemyslot;
    private Sprite _enemyBg;
    private Sprite _playerBg;
    Animation anim;
    List<Vector3> _iconPos = new List<Vector3>();
    Vector3 _wait1Scale;
    public enum Images
    {
        PlayUnit,//0
        Wait1,
        Wait2,
        Wait3,
        Wait4,
        Wait5,
        Wait6,
        Wait7,
        Wait8,
        Wait9,
        Wait10,
        Wait11,
        Wait12,
        Wait13,
        PlayUnitBg,//14
        Wait1Bg,
        Wait2Bg,
        Wait3Bg,
        Wait4Bg,
        Wait5Bg,
        Wait6Bg,
        Wait7Bg,
        Wait8Bg,
        Wait9Bg,
        Wait10Bg,
        Wait11Bg,
        Wait12Bg,
        Wait13Bg,
        Select1,//28
        Select2,
        Select3,
        Select4,
        Select5,
        Select6,
        Select7,
        Select8,
        Select9,
        Select10,
        Select11,
        Select12,
        Select13,
        lootHeaderBG,//41
        lootSubHeaderBG,
    }

    public enum GameObjects
    {
        UI_BattlePopUP,
        StageNameBG
    }
  
    public override void Init()
    {
        anim = GetComponent<Animation>();
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        UI_Battle.Bind();
        for (int i = (int)Images.PlayUnitBg; i <= (int)Images.Wait13Bg; i++)
        {
            _iconPos.Add(Get<Image>(i).rectTransform.anchoredPosition);
            
        }
        _wait1Scale = Get<Image>((int)Images.Wait1Bg).rectTransform.localScale;

        Get<Image>((int)Images.Wait13).gameObject.SetActive(false);
        Get<Image>((int)Images.Wait13Bg).gameObject.SetActive(false);

        SelectFalse();
        gameObject.SetActive(false);
        FlagFalse();
        BgInit();
    }
    void BgInit()
    {
        _enemyBg = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.BattleIcon,"EnemyBg");
        _playerBg = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.BattleIcon, "PlayerBG");
    }
    public void SetIcons(List<UnitOrder> UnitOrders)
    {
        Get<Image>((int)Images.PlayUnit).gameObject.SetActive(false);
        Get<Image>((int)Images.PlayUnitBg).gameObject.SetActive(false);
        Get<Image>((int)Images.Wait13Bg).gameObject.SetActive(true);
        Get<Image>((int)Images.Wait13).gameObject.SetActive(true);
        for (int i = 0; i < 13; i++)
        {
            Get<Image>(i+1).sprite = UnitOrders[i].Unit.Icon;
            if (UnitOrders[i].Unit.CompareTag("Player"))
            {
                Get<Image>(i + 15).sprite = _playerBg;
            }
            else
            {
                Get<Image>(i + 15).sprite = _enemyBg;
            }
        }
        anim.Play();
        
    }

    public void NowplayerTurn()
    {
        Managers.Battle.InplayerTrue();
    }
    public void InitIcons(List<UnitOrder> UnitOrders)
    {
        for (int i = 0; i < 13; i++)
        {
           
            Get<Image>(i).sprite = UnitOrders[i].Unit.Icon;
            if (UnitOrders[i].Unit.CompareTag("Player"))
            {
                Get<Image>(i + 14).sprite = _playerBg;
            }
            else
            {
                Get<Image>(i + 14).sprite = _enemyBg;
            }
        }
    }

    public void SelectFalse()
    {
        for (int i = (int)Images.Select1; i <= (int)Images.Select13; i++)
        {
            Get<Image>(i).gameObject.SetActive(false);
        }
    }
    public void FlagFalse()
    {
        Get<Image>(41).gameObject.SetActive(false);
        Get<Image>(42).gameObject.SetActive(false);
    }
    public void FlagTrue()
    {
        Get<Image>(41).gameObject.SetActive(true);
        Get<Image>(42).gameObject.SetActive(true);
    }
    
    public void CaveUI(bool iscave)
    {
        if (iscave)
        {
            Get<GameObject>((int)GameObjects.StageNameBG).SetActive(true);
        }
        else
        {
            Get<GameObject>((int)GameObjects.StageNameBG).SetActive(false);
        }
    }
    public void SelectTrue(List<UnitOrder> UnitOrders, BattleUnit target, bool isFirst =false)
    {
        if (isFirst)
        {
            for (int i = 1; i < 13; i++)
            {
                if (UnitOrders[i].Unit == target)
                {
                    Get<Image>(i + 27).gameObject.SetActive(true);
                }
                else
                {
                    Get<Image>(i + 27).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 1; i < 13; i++)
            {
                if (UnitOrders[i].Unit == target)
                {
                    Get<Image>(i + 28).gameObject.SetActive(true);
                }
                else
                {
                    Get<Image>(i + 28).gameObject.SetActive(false);
                }
            }
        }
    }

    public void Ending(List<UnitOrder> UnitOrders)
    {
        SelectFalse();
        Get<Image>((int)Images.PlayUnit).gameObject.SetActive(true);
        Get<Image>((int)Images.PlayUnitBg).gameObject.SetActive(true);
        for (int i = 0; i < 14; i++)
        {
            Get<Image>(i + 14).rectTransform.anchoredPosition = _iconPos[i];
        }
        InitIcons(UnitOrders);
        Get<Image>((int)Images.Wait1Bg).rectTransform.localScale = _wait1Scale;
        Get<Image>((int)Images.Wait13Bg).gameObject.SetActive(false);
        Get<Image>((int)Images.Wait13).gameObject.SetActive(false);
        
    }

    public void SetBattlePopUp(bool istrue)
    {
        Get<GameObject>(0).SetActive(istrue);
    }
    public override void OnClickedTurnOverButton(WorldMapPlayerCharacter player)
    {

    }
}
