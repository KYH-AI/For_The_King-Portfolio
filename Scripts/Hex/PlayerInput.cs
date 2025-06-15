using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    //public UnityEvent<Vector3> PointerClick;

    public UnityEvent<Vector3, bool> PointerHexMousePos;

    private bool _isMouseClick = false;
        
    private Vector3 _lastPosition;
    
    private Vector3 _currentPosition;
    
    private void Update()
    {
        DetectMouseClick();
    }
    
    private void DetectMouseClick()
    {
        if (Managers.Camera.nowState != CameraManager.NowState.WORLD) return;

        
        
        // 현재 플레이어 차례이면서 UI_EventPopUp 창이 활성화 되지 않은 경우만 월드맵 상호작용 가능함
        if ((bool)UnitManager.Instacne.SelectedWorldMapPlayerCharacter && !UnitManager.Instacne.SelectedWorldMapPlayerCharacter.IsPlayerInEvent)
        {
            _currentPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // UI 요소 위에 있다면 클릭 이벤트를 무시합니다.
                    Debug.Log("마우스 클릭을 UI 요소 위에서 무시합니다.");
                    return;
                }
                
                _isMouseClick = true;
            }

            if (_lastPosition == _currentPosition && !_isMouseClick) return;
            
            PointerHexMousePos?.Invoke(_currentPosition, _isMouseClick);
            _lastPosition = _currentPosition;
            _isMouseClick = false;
        }
        
    }
    
}
