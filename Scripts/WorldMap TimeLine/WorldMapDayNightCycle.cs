using System;
using System.Collections;
using System.Collections.Generic;

public class WorldMapDayNightCycle
{
    /*  << WorldMapTimeLine 주 목표>>
     *  1. 낮과 밤 시간을 계산하는 배열 구현 (0 = 아침, 1 = 밤)
     *  2. 현재 첫째 밤 또는  첫째 낮을 구별해야함 (낮과 밤에 출현하는 적 종류가 다름)
     */

    // 밤과 낮을 구별하는 배열
    private  readonly int[] _dayNightArray = new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 };
    
    // 하루의 총 길이 (낮 4 + 밤 4)
    private const int _MAX_CYCLE_LENGTH = 8;
    
    // 현재 시간 배열위치
    private int _currentIndex = 0;
    
    // 현재 밤, 낮 확인
    private int _currentTime;
    
    // 소요된 D-Day
    public int DaysElapsed { get; private set; } = 0;

    
    /// <summary>
    /// 다음 시간으로 설정 (isFirstDay = 첫날 낮 확인, isFirstNight = 첫날 밤 확인)
    /// </summary>
    public void AdvanceTime(out bool isFirstDay, out bool isFirstNight)
    {
        isFirstDay = false;
        isFirstNight = false;
        
        _currentIndex = (_currentIndex + 1) % _MAX_CYCLE_LENGTH;
        _currentTime = _dayNightArray[_currentIndex];
        
        if (_currentIndex == 0)          // 첫날 아침
        {
            // D-Day 1일 증가
            DaysElapsed++;
            isFirstDay = true;
            isFirstNight = false;
        }
        else if (_currentIndex == 4)     // 첫날 밤
        {
            isFirstDay = false;
            isFirstNight = true;
        }
    }

    
    /// <summary>
    /// 현재 시간을 확인
    /// </summary>
    /// <returns>true : 낮, false 밤</returns>
    private bool IsDay()
    {
        return _currentTime == 0;
    }
}
