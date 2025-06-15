using UnityEngine;

public class CrossFadeController : MonoBehaviour
{
    [SerializeField] private float _crossFadeTime = 1.0f; // Cross-fade가 일어나는 시간 (초)
    private Camera _newCamera; // Cross-fade 후 새로운 카메라
    private Camera _currentCamera; // Cross-fade 이전 사용 중인 카메라
    private Animator _animator; // Animator 컴포넌트
    private bool _isCrossFading; // Cross-fade 중인지 여부

    private void Start()
    {
        // 현재 사용 중인 카메라를 찾음
        _currentCamera = Camera.main;

        // Animator 컴포넌트를 찾음
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Cross-fade 중이면
        if (_isCrossFading)
        {
            // 가중치를 서서히 변화시킴
            _animator.SetFloat("CrossFadeWeight", _animator.GetFloat("CrossFadeWeight") + Time.deltaTime / _crossFadeTime);

            // Cross-fade가 끝나면
            if (_animator.GetFloat("CrossFadeWeight") >= 1.0f)
            {
                // 새로운 카메라를 활성화시킴
                _newCamera.gameObject.SetActive(true);

                // 현재 사용 중인 카메라를 비활성화시킴
                _currentCamera.gameObject.SetActive(false);

                // Cross-fade 중인 상태를 해제함
                _isCrossFading = false;
            }
        }
    }

    public void CrossFade(Camera NewCamera)
    {
        _newCamera = NewCamera;
        // Cross-fade 중이 아니면
        if (!_isCrossFading)
        {
            // Cross-fade 중인 상태로 변경함
            _isCrossFading = true;

            // 새로운 카메라의 위치와 방향을 현재 사용 중인 카메라와 일치시킴
            _newCamera.transform.position = _currentCamera.transform.position;
            _newCamera.transform.rotation = _currentCamera.transform.rotation;

            // 새로운 카메라를 비활성화시킴
            _newCamera.gameObject.SetActive(false);

            // Cross-fade에 필요한 가중치 초기값을 설정함
            _animator.SetFloat("CrossFadeWeight", 0.0f);
        }
    }
}
