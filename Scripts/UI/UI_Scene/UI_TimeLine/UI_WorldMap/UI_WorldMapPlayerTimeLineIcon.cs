using System;
using UnityEngine;
using UnityEngine.UI;
public class UI_WorldMapPlayerTimeLineIcon : UI_Base
{
    // 플레이어 초상화 슬롯 캐싱
    private Image _playerIconSlot;

    // 플레이이 초상화 오브젝트 위치
    private enum PlayerImageSlot
    {
        PlayerIconSlot
    }


    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 이미지 컴포넌트 초기화 (1회 호출)
    /// </summary>
    public override void Init()
    {
        Bind<Image>(typeof(PlayerImageSlot));
        _playerIconSlot = Get<Image>((int)PlayerImageSlot.PlayerIconSlot);
        _playerIconSlot.color = Color.gray;
    }

    /// <summary>
    /// 플레이어 초상화 설정 
    /// </summary>
    /// <param name="playerSprite">설정할 플레이어 초상화 이미지</param>
    public void PlayerIconInit(Sprite playerSprite)
    {
        _playerIconSlot.sprite = playerSprite;
    }
    

    /// <summary>
    /// 플레이어 초상화 배경화면 설정 (플레이어 턴인 경우 흰색 아니면 회색)
    /// </summary>
    public Color SetPlayerIconBackGroundColor
    {
        set => _playerIconSlot.color = value;
    }
}
