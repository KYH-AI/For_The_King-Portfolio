using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEventListener : StateMachineBehaviour
{
    // UI ��¿� �ݹ� �̺�Ʈ �Լ�
    private static event Action _uiPopUp;
    
    public static event Action UiPopUp
    {
        add
        {
            // �������� �ִ� �̺�Ʈ ����
            if (_uiPopUp != null)
                _uiPopUp = null;

            // ���ο� �̺�Ʈ ���
            _uiPopUp += value;
        }
        remove
        {
            _uiPopUp -= value;
        }
    }
    
    // ī�޶� Blur ȿ�� �۵��Լ� �̺�Ʈ
    private static event Action<bool, float> _cameraBlurEffect;
    
    public static event Action<bool, float> CameraBlurEffect
    {
        add
        {
            // �������� �ִ� �̺�Ʈ ����
            if (_cameraBlurEffect != null)
                _cameraBlurEffect = null;

            // ���ο� �̺�Ʈ ���
            _cameraBlurEffect += value;
        }
        remove
        {
            _cameraBlurEffect -= value;
        }
    }
    
    
    // ī�޶� Blur ȿ�� ���� or ����
    public static bool IsBlurEffectIncrease { get; set; }
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _cameraBlurEffect?.Invoke(IsBlurEffectIncrease, stateInfo.length);
    }
    
    // �ش� ī�޶� ������ ����� ��� UI �Լ��� �����
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _uiPopUp?.Invoke();
    }
    
}
