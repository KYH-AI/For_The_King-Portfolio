using System;
using UnityEngine;
using UnityEngine.Events;

public class HexEventFxHandler : MonoBehaviour
{
    // Hex �̺�Ʈ ���� ��ƼŬ �ý��� ���� ���� Ŭ����
    
    public event UnityAction EventAction;
    

    private void OnParticleSystemStopped()
    {
        EventAction?.Invoke();
    }
}
