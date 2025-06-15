using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEventListener : StateMachineBehaviour
{
    // UI 출력용 콜백 이벤트 함수
    private static event Action _uiPopUp;
    
    public static event Action UiPopUp
    {
        add
        {
            // 마지막에 있던 이벤트 제거
            if (_uiPopUp != null)
                _uiPopUp = null;

            // 새로운 이벤트 등록
            _uiPopUp += value;
        }
        remove
        {
            _uiPopUp -= value;
        }
    }
    
    // 카메라 Blur 효과 작동함수 이벤트
    private static event Action<bool, float> _cameraBlurEffect;
    
    public static event Action<bool, float> CameraBlurEffect
    {
        add
        {
            // 마지막에 있던 이벤트 제거
            if (_cameraBlurEffect != null)
                _cameraBlurEffect = null;

            // 새로운 이벤트 등록
            _cameraBlurEffect += value;
        }
        remove
        {
            _cameraBlurEffect -= value;
        }
    }
    
    
    // 카메라 Blur 효과 증가 or 감소
    public static bool IsBlurEffectIncrease { get; set; }
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _cameraBlurEffect?.Invoke(IsBlurEffectIncrease, stateInfo.length);
    }
    
    // 해당 카메라 연출이 종료될 경우 UI 함수를 출력함
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _uiPopUp?.Invoke();
    }
    
}
