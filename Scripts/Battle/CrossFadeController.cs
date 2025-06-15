using UnityEngine;

public class CrossFadeController : MonoBehaviour
{
    [SerializeField] private float _crossFadeTime = 1.0f; // Cross-fade�� �Ͼ�� �ð� (��)
    private Camera _newCamera; // Cross-fade �� ���ο� ī�޶�
    private Camera _currentCamera; // Cross-fade ���� ��� ���� ī�޶�
    private Animator _animator; // Animator ������Ʈ
    private bool _isCrossFading; // Cross-fade ������ ����

    private void Start()
    {
        // ���� ��� ���� ī�޶� ã��
        _currentCamera = Camera.main;

        // Animator ������Ʈ�� ã��
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Cross-fade ���̸�
        if (_isCrossFading)
        {
            // ����ġ�� ������ ��ȭ��Ŵ
            _animator.SetFloat("CrossFadeWeight", _animator.GetFloat("CrossFadeWeight") + Time.deltaTime / _crossFadeTime);

            // Cross-fade�� ������
            if (_animator.GetFloat("CrossFadeWeight") >= 1.0f)
            {
                // ���ο� ī�޶� Ȱ��ȭ��Ŵ
                _newCamera.gameObject.SetActive(true);

                // ���� ��� ���� ī�޶� ��Ȱ��ȭ��Ŵ
                _currentCamera.gameObject.SetActive(false);

                // Cross-fade ���� ���¸� ������
                _isCrossFading = false;
            }
        }
    }

    public void CrossFade(Camera NewCamera)
    {
        _newCamera = NewCamera;
        // Cross-fade ���� �ƴϸ�
        if (!_isCrossFading)
        {
            // Cross-fade ���� ���·� ������
            _isCrossFading = true;

            // ���ο� ī�޶��� ��ġ�� ������ ���� ��� ���� ī�޶�� ��ġ��Ŵ
            _newCamera.transform.position = _currentCamera.transform.position;
            _newCamera.transform.rotation = _currentCamera.transform.rotation;

            // ���ο� ī�޶� ��Ȱ��ȭ��Ŵ
            _newCamera.gameObject.SetActive(false);

            // Cross-fade�� �ʿ��� ����ġ �ʱⰪ�� ������
            _animator.SetFloat("CrossFadeWeight", 0.0f);
        }
    }
}
