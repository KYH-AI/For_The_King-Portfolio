using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_Option : UI_Base
{
    private enum Btn
    {
        OptionBtn,
        CloseButton,
        SoundOptions,
        OutButton,
        YesButton,
        NoButton,
    }
    private enum GamObjects
    {
        OptionMenu,
        SystemDialog,
    }
    public override void Init()
    {
        Bind<Button>(typeof(Btn));
        Bind<GameObject>(typeof(GamObjects));
        Get<Button>((int)Btn.OptionBtn).gameObject.BindEvent(data => { OptionBtn(); }, MouseUIEvent.Click);
        Get<Button>((int)Btn.CloseButton).gameObject.BindEvent(data => { MainMenuBtn(); }, MouseUIEvent.Click);
        Get<Button>((int)Btn.SoundOptions).gameObject.BindEvent(data => { SoundOptionBtn(); }, MouseUIEvent.Click);
        Get<Button>((int)Btn.OutButton).gameObject.BindEvent(data => { OptionCloseBtn(); }, MouseUIEvent.Click);
        Get<Button>((int)Btn.YesButton).gameObject.BindEvent(data => { YesBtn(); }, MouseUIEvent.Click);
        Get<Button>((int)Btn.NoButton).gameObject.BindEvent(data => { NoBtn(); }, MouseUIEvent.Click);
    }

    private void OptionBtn()
    {
        Get<GameObject>((int)GamObjects.OptionMenu).SetActive(true);
    }
    private void MainMenuBtn()
    {
        Get<GameObject>((int)GamObjects.SystemDialog).SetActive(true);
    }
    private void SoundOptionBtn()
    {

    }
    private void OptionCloseBtn()
    {
        Get<GameObject>((int)GamObjects.OptionMenu).SetActive(false);
    }
    private void YesBtn()
    {
        Managers.Camera.FadeOut.FadeOut(2f, new Color(0, 0, 0));
        GameLogic.Instance.SetGameResult(GameLogic.GameResult.Dead);
        GameLogic.Instance.SceneChange(GameLogic.Scene.MainMenu, 2f);
    }
    private void NoBtn()
    {
        Get<GameObject>((int)GamObjects.SystemDialog).SetActive(false);
    }


}
