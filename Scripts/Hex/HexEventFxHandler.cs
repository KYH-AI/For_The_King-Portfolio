using System;
using UnityEngine;
using UnityEngine.Events;

public class HexEventFxHandler : MonoBehaviour
{
    // Hex 이벤트 지역 파티클 시스템 연출 종료 클래스
    
    public event UnityAction EventAction;
    

    private void OnParticleSystemStopped()
    {
        EventAction?.Invoke();
    }
}
