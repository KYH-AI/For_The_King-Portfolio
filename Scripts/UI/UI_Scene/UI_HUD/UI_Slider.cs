using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : UI_Base
{
    // <주 목표>
    /*  1. 슬라이더에 대한 연출을 통일한다. (부드럽게 슬라이더 조절 그리고 슬라이더와 관련된 텍스트도 동일하게 연출)
     *  2. 슬라이더 컴포넌트가 붙어 있는 체력바, 경험치바는 해당 클래스를 이용해 슬라이더를 조절한다.
     *  
     */
 
    private enum SliderBar
    {
        MainSliderBar,     // 최종 슬라이더
        ChangeSliderBar   // 연출용 슬라이더
    }

    // 목적 값
    private float _destinationValue;
    // 시작 값
    private float _startValue;
    // 현재 값
    private float _currentValue;
    // 코루틴 작동 확인
    private bool _isCoroutineRunning = false;
    

    /// <summary>
    /// 각 Hud 클래스에서 초기화 호출함
    /// </summary>
    public override void Init()
    {
        Bind<Slider>(typeof(SliderBar));
     //   print($"{this.gameObject.name} 에서 : {Get<Slider>((int)SliderBar.MainSliderBar).gameObject} 와 {Get<Slider>((int)SliderBar.ChangeSliderBar).gameObject} 준비됨");
    }

    /// <summary>
    /// 슬라이더 게이지 변경 함수
    /// </summary>
    /// <param name="value">변경되어야 하는 슬라이더 값 (0f ~ 1f)</param>
    /// <param name="optionalText">슬라이더와 같이 연출하고 싶은 텍스트 ( ex : 체력 Text )</param>
    /// <param name="finishedValue">현재 변경된 값 (즉 목적지)</param>
    /// <param name="maxValue">해당 값에 대한 최대치 (혹시라도 보정 할려고)</param>
    public void SetSliderValue(float value, TextMeshProUGUI optionalText = null, int finishedValue = 0, int maxValue = 0)
    {
        _isCoroutineRunning = false;
        
        // Slider 값이 이미 0인 경우 또는 요구하는 슬라이더 값이 0인 경우 불필요한 연출을 하지 않고 바로 0으로 만든다.  (즉 사망처리)
        if (value == 0f ||  Get<Slider>((int)SliderBar.MainSliderBar).value == 0f)
        {
            Get<Slider>((int)SliderBar.MainSliderBar).value = value;
            Get<Slider>((int)SliderBar.ChangeSliderBar).gameObject.SetActive(false);
            Get<Slider>((int)SliderBar.ChangeSliderBar).fillRect.gameObject.SetActive(false);

            if ((object)optionalText != null) optionalText.text = finishedValue.ToString();

            return;
        }

        _destinationValue = value;
        // Hud 부모 오브젝트가 비활성화 상태인 경우 
        if (!gameObject.activeInHierarchy)
        {
            Get<Slider>((int)SliderBar.MainSliderBar).value = _destinationValue;
            Get<Slider>((int)SliderBar.ChangeSliderBar).value = _destinationValue;
        }
        else if (_isCoroutineRunning == false) // 슬라이드 코루틴이 작동하지 않는다면
        {
            // 현재 값을 슬라이더와 시작 값으로 적용
            _startValue = (_currentValue = Get<Slider>((int)SliderBar.MainSliderBar).value);
            // 연출용 슬라이더 오브젝트 활성화
            Get<Slider>((int)SliderBar.ChangeSliderBar).gameObject.SetActive(true);
            Get<Slider>((int)SliderBar.ChangeSliderBar).fillRect.gameObject.SetActive(true);
            // 연출 코루틴 시작
            StartCoroutine(ChangeSliderBar(optionalText, finishedValue, maxValue));
        }
    }

    private IEnumerator ChangeSliderBar(TextMeshProUGUI optionalText, int finishedValue, int maxValue)
    {
        _isCoroutineRunning = true;
        float downRate = 1f;//0.01f;
        float upRate = 0.5f; // 0.0004f;
        while (_currentValue != _destinationValue)
        {

            // 슬라이더 값이 증가될 경우
            if (_destinationValue > _currentValue)
            {
                // 슬라이더 값이 증가될 경우 연출 슬라이더가 먼저 목적값에 도달했으며 메인 슬라이더가 천천히 올라온다.
                _currentValue = Mathf.Clamp(_currentValue + Time.deltaTime * upRate, 0f, _destinationValue);
                Get<Slider>((int)SliderBar.MainSliderBar).value = _currentValue;
                Get<Slider>((int)SliderBar.ChangeSliderBar).value = _destinationValue;
                int textValue = RoundToInt((float)maxValue * _currentValue);
                SetOptionalText(optionalText, textValue);
                if (!_isCoroutineRunning)
                {
                    break;
                }

                yield return null;
            }
            // 슬라이더 값이 감소될 경우
            else
            {
                // 슬라이더 값이 감소될 경우메인 슬라이더가가 먼저 목적값에 도달했으며 연출 슬라이더가 천천히 내려간다.
                _currentValue = Mathf.Clamp(_currentValue - Time.deltaTime * downRate, _destinationValue, _startValue);
                Get<Slider>((int)SliderBar.MainSliderBar).value = _currentValue;
                Get<Slider>((int)SliderBar.ChangeSliderBar).value = _startValue;
                int textValue = RoundToInt((float)maxValue * _currentValue);
                SetOptionalText(optionalText, textValue);
                if (!_isCoroutineRunning)
                {
                    break;
                }

                yield return null;
            }
        }
        // 최종 값을 강제로 할당
        SetOptionalText(optionalText, finishedValue);
        // 연출 코루틴과 슬라이드 오브젝트를 비활성화
        _isCoroutineRunning = false;
        Get<Slider>((int)SliderBar.ChangeSliderBar).gameObject.SetActive(false);
        Get<Slider>((int)SliderBar.ChangeSliderBar).fillRect.gameObject.SetActive(false);
    }

    /// <summary>
    /// 슬라이드 값과 텍스트도 동일한 값으로 교체
    /// </summary>
    /// <param name="optionalText">연출이 필요한 텍스트</param>
    /// <param name="textValue">변경된 값</param>
    private static void SetOptionalText(TextMeshProUGUI optionalText, int textValue)
    {
        // 연출 텍스트가 필요없는 경우 중지
        if (!optionalText) return;

        optionalText.text = textValue.ToString();
    }
    
    /// <summary>
    /// 주어진 float 값에 가장 가까운 정수 값을 반환
    /// </summary>
    /// <param name="value">정수 값으로 변환 시킬 값</param>
    /// <returns> float 값을 반올림하여 가장 가까운 정수 값</returns>
    private static int RoundToInt(float value)
    {
        if (value > 0f)
        {
            return (int)(value + 0.5f);
        }
        return (int)(value - 0.5f);
    }
}
