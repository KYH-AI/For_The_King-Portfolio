using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EventUnitSlot : UI_Base
{
    private static readonly string UNKNOW_PORTRAITS = "EncounterUnknown";
    
    private enum Roots
    {
        EventObjectLevelBackGround,
        EventObjectPortraitRoot,
    }

    private enum LevelText
    {
        EventObjectLevelText
    }
    
    private enum Portrait
    {
        UI_ObjectPortrait
    }

   public override void Init()
   {
       Bind<GameObject>(typeof(Roots));
       Bind<TextMeshProUGUI>(typeof(LevelText));
       Bind<UI_ObjectPortrait>(typeof(Portrait));
       
       Get<UI_ObjectPortrait>(0).Init();
   }

   public void SetEventObjectInformation(bool disableLevel, Sprite portrait = null, int level = -1)
   {
       if (!portrait)
           portrait = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.PortraitEnemy,
               UNKNOW_PORTRAITS);
       
       Get<GameObject>((int)Roots.EventObjectLevelBackGround).gameObject.SetActive(!disableLevel);
     //  GameObject levelBackground = Get<GameObject>((int)Roots.EventObjectLevelBackGround);
     //  levelBackground.SetActive(!disableLevel);
       
       Get<UI_ObjectPortrait>((int)Portrait.UI_ObjectPortrait).SetPortraitTexture(portrait);
       Get<TextMeshProUGUI>((int)LevelText.EventObjectLevelText).text = level.ToString();
   }
}
