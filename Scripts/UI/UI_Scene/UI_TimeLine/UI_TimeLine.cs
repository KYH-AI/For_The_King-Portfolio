using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_TimeLine : UI_Base
{

    /* << OnClickedTurnOverButton 주 목표 >>
     *
     *  1. 턴 종료 버튼의 추상화 메서드이 필요함 (각 기능마다 턴 종류 버튼 기능이 다름)
     *      1-1. 월드 맵 턴 종료 버튼 기능
     *      1-2. 전투 턴 종료 버튼 기능
     *      1-3. 광산 턴 종료 버튼 기능
     */

    /// <summary>
    /// 턴 종료 버튼 이벤트 추상화 메서드
    /// </summary>
    /// <param name="player">턴 종료 버튼을 누른 플레이어 정보</param>
    public abstract void OnClickedTurnOverButton(WorldMapPlayerCharacter player);
}
