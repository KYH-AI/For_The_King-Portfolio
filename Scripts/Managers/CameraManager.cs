using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Cinemachine;
using Cinemachine.PostFX;

public class CameraManager : MonoBehaviour
{

    
    public enum CameraVolumeType
    {
        Battle,
        Store,
        Cave,
    }

    #region �κ��丮 Dummy ī�޶�

    // �κ��丮 Dummy ���� ���̾�
    public readonly int INVENTORY_DUMMY_LAYER = 9;

    private const int _INVENTORY_DUMMY_CAM_INDEX = 4;

    private Camera[] _inventoryDummyCamera;

    #endregion
    
    #region ����� ī�޶� ��ǥ �� �̵��ӵ�


    public CinemachineVirtualCameraBase WorldVirtualCam;//���忡�� ����ϴ� ����ī�޶�

    public CinemachineVirtualCameraBase Nowcam;//���� ����ϰ� �ִ� ����ī�޶�



    // ī�޶� ���� ��ǥ
    private readonly float[] _WORLD_MAP_CAMERA_OFFSET = {0f, 12f, -10f};
    
    // ī�޶� �ڵ� �̵� �ӵ�
    private readonly float _WORLD_CAMERA_AUTO_MOVE_SPEED = 50f;

    // ī�޶� ���� �̵� �ӵ�
    private readonly float _WORLD_CAMERA_MANUAL_SPEED = 15f;
    
    private readonly string[] _INPUT_KEY_BOARD = new string[2] { "Horizontal", "Vertical" };

    // ī�޶� ���� �߿� ����� W,A,S,D ���� ����ó��
    private bool _worldCameraIsBusy = false;

    #endregion

    #region ���� ī�޶�

    
    [Header("�� ���� ī�޶� ���� ��ġ")]
    [SerializeField] private CinemachineBlendListCamera[] _shopCamerablendList = new CinemachineBlendListCamera[3];
   
    #endregion

    #region ī�޶� ����Ʈ ���μ��� ����

    

    private CinemachineVolumeSettings _battleCameraVolume;//��Ʋī�޶��� ����
    private CinemachineVolumeSettings _storeCameraVolume;//����ī�޶��� ����

    private CinemachineVolumeSettings _nowVolum;//���� ����� ����

    private Coroutine _cameraBlurCoroutine;

    #endregion

    //��Ʋ �������� �� ������
    private float _battleFd = 5f;
    private float _battleLen = 133f;

    public RectTransform transform_cursor;
    private RectTransform transform_PopUp;
    [SerializeField]

    List<CinemachineVirtualCameraBase> BattleCamPos = new List<CinemachineVirtualCameraBase>();
    List<CinemachineVirtualCameraBase> CaveCamPos = new List<CinemachineVirtualCameraBase>();
    CinemachineBlendListCamera _battleBlendList;
    CinemachineBlendListCamera _caveBlendList;
    CinemachineBlendListCamera _caveMidBlendList;
    bool _isCaveFinal = false;
    public bool IsCaveFinal
    {
        get { return _isCaveFinal; }
        set { _isCaveFinal = value; }
    }
    public UI_FadeOut FadeOut; //���� ���Խ� ���̵�ƿ��� �̹���

    private bool _isBatteEnd =false;
    public bool IsBatteEnd
    {
        get { return _isBatteEnd; }
        set { _isBatteEnd = value; }
    }
    public bool DelayAttack;
    public enum NowState
    {
        NONE =-1,
        WORLD,
        BATTLE,
        STORE,
        CAVE,
    }
    public enum NowPlay
    {
        NONE =-1,
        PLAYER,
        ENEMY,
    }
    public NowState nowState;
    public NowPlay nowPlay = NowPlay.NONE;
    public int CaveCamIndex =1;
    int _caveBeforecam;

    Color _caveFog = new Color(46f / 255, 159f / 255, 159f / 255);
    /// <summary>
    /// Init
    /// </summary>
    public void Init()
    {
        FadeOut.gameObject.SetActive(true);
        
        nowState = NowState.WORLD;

        Nowcam = WorldVirtualCam;
        
        // ī�޶� ���� ���� (Blur �����Լ� �̺�Ʈ ���)
        CameraEventListener.CameraBlurEffect += BlurEffect;
    }
    
    void Update()
    {
        switch (nowState)
        {
            case NowState.WORLD:
                if (_worldCameraIsBusy) return;
                WorldMove();
                break;
            case NowState.BATTLE:
                BattleaMove();
                break;
            case NowState.CAVE:
                CaveMove();
                break;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UIManager.ClosePopUpUI();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {

            List<PlayerStats> playerObjectList = new List<PlayerStats>();
            for (int i = 0; i < Managers.Instance.GetPlayerCount; i++)
            {
                playerObjectList.Add(Managers.Instance.GetPlayer(i).PlayerStats);
            }
            List<List<int>> temp = new List<List<int>>();

            Managers.Cave.CaveInit(CaveManager.LengthType.SHORT, playerObjectList, temp, "CaveMap");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine("CaveToWorld");
        }


    }

    IEnumerator CaveToWorld()
    {
        FadeOut.FadeOut(1.5f, new Color(255, 255, 255));
        yield return new WaitForSeconds(2f);
        Managers.Cave.ToWorld();
    }
    /// <summary>
    /// ī�޶� ��ȯ �Լ�
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void ChangeCamera(CinemachineVirtualCameraBase from, CinemachineVirtualCameraBase to)
    {
        from.Priority = 0;
        to.Priority = 1;
        Nowcam = to;
    }

    /// <summary>
    /// ���� ����� ���� ����
    /// </summary>
    /// <param name="cameraVolumeType"></param>
    public void ChangeVolume(CameraVolumeType cameraVolumeType)
    {
        switch (cameraVolumeType)
        {
            case CameraVolumeType.Battle:
                _nowVolum = _battleCameraVolume;
                break;
            case CameraVolumeType.Store:
                _nowVolum = _storeCameraVolume;
                break;
        }
    }
    
    #region �÷����̹���
    
    private void Update_MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        transform_cursor.position = mousePos;
    }
    public void MouseEnterPopUp(Image image)
    {
        image.gameObject.SetActive(true);
        transform_PopUp = image.rectTransform;
        if (transform_PopUp.GetComponent<Graphic>())
            transform_PopUp.GetComponent<Graphic>().raycastTarget = false;
        float w = transform_PopUp.rect.width;
        float h = transform_PopUp.rect.height;
        transform_PopUp.position = transform_cursor.position + (new Vector3(w, h) * 0.5f);
    }

 
    #endregion

    #region ��Ʋ ī�޶�
    public void SetBattleCamera(Transform Parent)
    {
        nowState = NowState.BATTLE;
        BattleCamPos.Clear();
        //�̵��� ���� ����ī�޶� ����
        _battleBlendList =Parent.GetChild(0).GetComponent<CinemachineBlendListCamera>();
        _battleCameraVolume = _battleBlendList.GetComponent<CinemachineVolumeSettings>();
        SetBlur(_battleCameraVolume, _battleLen,_battleFd);
        foreach (Transform child in _battleBlendList.transform)
            BattleCamPos.Add(child.GetComponent<CinemachineVirtualCameraBase>());

        _battleBlendList.m_Instructions[0].m_VirtualCamera = BattleCamPos[0];
        _battleBlendList.m_Instructions[1].m_VirtualCamera = null;
        _battleBlendList.m_Instructions[2].m_VirtualCamera = null;
        FadeOut.FadeIn(2f, new Color(255, 255, 255));
        ChangeCamera(WorldVirtualCam, _battleBlendList);

    }
    
    void BattleaMove()
    {
        if ((int)nowPlay !=-1 && !_isBatteEnd)
        {
            if ((int)nowPlay==0)
            {
                _battleBlendList.m_Instructions[0].m_VirtualCamera = BattleCamPos[2];
                _battleBlendList.m_Instructions[1].m_VirtualCamera = BattleCamPos[0];
                _battleBlendList.m_Instructions[2].m_VirtualCamera = BattleCamPos[1];
                _battleBlendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                _battleBlendList.m_Instructions[2].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                _battleBlendList.m_Instructions[1].m_Blend.m_Time = 1.0f;
                _battleBlendList.m_Instructions[0].m_Hold = 0f;
                _battleBlendList.m_Instructions[2].m_Blend.m_Time = 1.0f;
                _battleBlendList.m_Instructions[1].m_Hold = 0f;
                if (!_battleBlendList.IsBlending)
                {
                    DelayAttack = false;
                }
                else
                {
                    DelayAttack = true;
                }
            }
            else if ((int)nowPlay == 1)
            {
                _battleBlendList.m_Instructions[0].m_VirtualCamera = BattleCamPos[1];
                _battleBlendList.m_Instructions[1].m_VirtualCamera = BattleCamPos[0];
                _battleBlendList.m_Instructions[2].m_VirtualCamera = BattleCamPos[2];
                _battleBlendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                _battleBlendList.m_Instructions[2].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                _battleBlendList.m_Instructions[1].m_Blend.m_Time = 1.0f;
                _battleBlendList.m_Instructions[0].m_Hold = 0f;
                _battleBlendList.m_Instructions[2].m_Blend.m_Time = 1.0f;
                _battleBlendList.m_Instructions[1].m_Hold = 0f;
                if (!_battleBlendList.IsBlending)
                {
                    DelayAttack = false;
                }
                else
                {
                    DelayAttack = true;
                }
            }
        }
        else if ((int) nowPlay != -1 && _isBatteEnd)
        {
            _battleBlendList.m_Instructions[0].m_VirtualCamera = BattleCamPos[1];
            _battleBlendList.m_Instructions[1].m_VirtualCamera = BattleCamPos[0];
            _battleBlendList.m_Instructions[2].m_VirtualCamera = BattleCamPos[2];
            _battleBlendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            _battleBlendList.m_Instructions[2].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            _battleBlendList.m_Instructions[1].m_Blend.m_Time = 5.0f;
            _battleBlendList.m_Instructions[0].m_Hold = 0f;
            _battleBlendList.m_Instructions[2].m_Blend.m_Time = 5.0f;
            _battleBlendList.m_Instructions[1].m_Hold = 0f;
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        CinemachineVirtualCamera virtualCamera =null;
        if (Nowcam == _battleBlendList)
        {
             virtualCamera = (CinemachineVirtualCamera)_battleBlendList.m_Instructions[2].m_VirtualCamera;
        }
        else if (Nowcam == _caveBlendList)
        {
             virtualCamera = (CinemachineVirtualCamera)_caveBlendList.m_Instructions[1].m_VirtualCamera;
        }


        // ��鸲 ȿ���� Ȱ��ȭ�մϴ�.
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        // ���� �ð� �Ŀ� ��鸲 ȿ���� ��Ȱ��ȭ�մϴ�.
        StartCoroutine(StopShake(duration, virtualCamera));
    }
    private IEnumerator StopShake(float duration, CinemachineVirtualCamera virtualCamera)
    {
        yield return new WaitForSeconds(duration);
        // ��鸲 ȿ���� ��Ȱ��ȭ�մϴ�.
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    #endregion

    #region ���� ī�޶�
    public void SetCaveCamera(Transform Parent)
    {
        nowState = NowState.CAVE;
        CaveCamPos.Clear();
        _caveBeforecam = 3;
        CaveCamIndex = 3;
        //�̵��� ���� ����ī�޶� ����
        _caveBlendList = Parent.GetChild(0).GetComponent<CinemachineBlendListCamera>();
        foreach (Transform child in _caveBlendList.transform)
            CaveCamPos.Add(child.GetComponent<CinemachineVirtualCameraBase>());
        _caveMidBlendList = Parent.GetChild(1).GetChild(1).GetComponent<CinemachineBlendListCamera>();
        _caveBlendList.m_Instructions[0].m_VirtualCamera = CaveCamPos[3];
        _caveBlendList.m_Instructions[1].m_VirtualCamera = CaveCamPos[3];
        FadeOut.FadeIn(2f, new Color(255, 255, 255));
        ChangeCamera(WorldVirtualCam, _caveBlendList);

        RenderSettings.fog = true;
        RenderSettings.fogColor = _caveFog;
        RenderSettings.fogDensity = 0.03f;
    }

    void CaveMove()
    {
        if(CaveCamIndex != _caveBeforecam)
        {
            _caveBlendList.m_Instructions[0].m_VirtualCamera = CaveCamPos[_caveBeforecam];
            _caveBlendList.m_Instructions[1].m_VirtualCamera = CaveCamPos[CaveCamIndex];
            _caveBlendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            _caveBlendList.m_Instructions[1].m_Blend.m_Time = 1.0f;
            _caveBlendList.m_Instructions[0].m_Hold = 0f;
            _caveBeforecam = CaveCamIndex;
        }
        if (!_caveBlendList.IsBlending)
        {
            DelayAttack = false;
            
        }
        else
        {
            DelayAttack = true;
        }

        if (!_caveMidBlendList.IsBlending&& _isCaveFinal)
        {
            _isCaveFinal=false;
            Managers.Cave.NowCave.FinalReward();
        }
    }

    public void ClearStageCam()
    {
        StartCoroutine(ClearStage());
    }
    IEnumerator ClearStage()
    {
        ChangeCamera(Nowcam, Managers.Cave.GetDoor());
        if (!Managers.Cave.IsNextFinal)
        {
            yield return new WaitForSeconds(1.5f);
            FadeOut.FadeOut(1.2f, new Color(0, 0, 0));
            yield return new WaitForSeconds(1.2f);
            ChangeCamera(Nowcam, _caveBlendList);
            if (Managers.Cave.NowEvent == CaveManager.CaveEvent.SAFE)//���������� ��� ���̵� �ƿ��϶� ������Ʈ off��
            {
                Managers.Cave.NowCave.Ojs[5].SetActive(false);
            }

            Managers.Cave.SetNextNode();
            Managers.Cave.PlayNode();

            yield return new WaitForSeconds(2f);
            FadeOut.FadeIn(2f, new Color(0, 0, 0));
        }
        else
        {
           
            Managers.Cave.SetNextNode();
            Managers.Cave.PlayNode();
            yield return new WaitForSeconds(2f);
            _isCaveFinal = true;
        }
       
    }

    #endregion


    #region blurȿ��
    public void BlurEffect(bool increaseBlur, float blurTimer)// Blur ȿ�� Ȱ��ȭ ����, Blur ����ð�
    {
        _isBatteEnd = true;
        _cameraBlurCoroutine = StartCoroutine(BlurEffect(increaseBlur ? 300f : 1f, increaseBlur, blurTimer));
    }



    private IEnumerator BlurEffect(float targetBlurValue, bool increaseBlur, float blurTime)
    {
        if (_nowVolum.m_Profile.TryGet(out DepthOfField depthOfField))
        {
            float elapsedTime = 0f;
            float startBlurValue = depthOfField.focalLength.value;
            float endBlurValue = targetBlurValue;


            while (elapsedTime < blurTime)
            {
                float t = elapsedTime / blurTime; // �ð��� ���� ���� ����ġ
                float blurValue;

                /*
                if (increaseBlur)
                {
                   
                }
                else
                {
                    blurValue = Mathf.Lerp(endBlurValue, startBlurValue, t); // ����: �� ������ ���� ������ ����
                }
                */


                blurValue = Mathf.Lerp(startBlurValue, endBlurValue, t); // ����: ���� ������ �� ������ ����

                depthOfField.focalLength.value = blurValue;

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            // depthOfField.focalLength.value = endBlurValue; // ���� ��ǥ �� ����

            /*
            while (elapsedTime < blurTime)
            {
                
                float normalizedTime = elapsedTime / blurTime;
                float smoothStepValue = Mathf.SmoothStep(0f, 1f, normalizedTime);
                float blurValue;
                blurValue = Mathf.Lerp(startBlurValue, endBlurValue, smoothStepValue); // ����: ���� ������ �� ������ ����
                
                if (increaseBlur)
                {
                    blurValue = Mathf.Lerp(startBlurValue, endBlurValue, smoothStepValue); // ����: ���� ������ �� ������ ����
                }
                else
                {
                    blurValue = Mathf.Lerp(endBlurValue, startBlurValue, smoothStepValue); // ����: �� ������ ���� ������ ����
                }
                
                

                depthOfField.focalLength.value = blurValue;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            */
            depthOfField.focalLength.value = targetBlurValue;
        }
        depthOfField.focusDistance.value = 30f;
    }

    private void BlurOff(CinemachineVolumeSettings volume)
    {
        // Blur ȿ�� OFF
        volume.m_Profile.TryGet(out DepthOfField bluerEffect);  
        bluerEffect.focalLength.value = bluerEffect.focalLength.min;
        bluerEffect.focusDistance.value = 1f;
    }

    private void BlurOn(CinemachineVolumeSettings volume)
    {
        // Blur ȿ�� ON
        volume.m_Profile.TryGet(out DepthOfField bluerEffect);
        bluerEffect.focalLength.value = bluerEffect.focalLength.max;
    }
    private void SetBlur(CinemachineVolumeSettings volume, float fl, float fd)
    {

        volume.m_Profile.TryGet(out DepthOfField bluerEffect);
        bluerEffect.focalLength.value = fl;
        bluerEffect.focusDistance.value = fd;
    }

    #endregion

    #region ���� ī�޶�
    
    public void SetWorldCamera()
    {
        if (_cameraBlurCoroutine != null) StopCoroutine(_cameraBlurCoroutine);

        _isBatteEnd = false;
        nowPlay = NowPlay.NONE;
        nowState = NowState.WORLD;
        //BlurOff(_nowVolum);
        FadeOut.FadeIn(2f, new Color(255, 255, 255));

        ChangeCamera(Nowcam, WorldVirtualCam);

        RenderSettings.fog = false;
    }
    void WorldMove()
    {
        float h = Input.GetAxisRaw(_INPUT_KEY_BOARD[0]);
        float v = Input.GetAxisRaw(_INPUT_KEY_BOARD[1]);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TeleportToWorldCamera(Managers.Turn.PlayerTurn.transform.position);
        }
        if (h == 0 && v == 0) return;

        Vector3 pos =  Nowcam.transform.position;
        pos.x += h * Time.deltaTime * _WORLD_CAMERA_MANUAL_SPEED;
        pos.z += v * Time.deltaTime * _WORLD_CAMERA_MANUAL_SPEED;

        WorldVirtualCam.transform.position = pos;

      
    }
    
    /// <summary>
    /// ī�޶� �ε巴�� ������ �ƴ� �����̵��� �ʿ��� ���
    /// </summary>
    /// <param name="targetPos">������ ��ǥ</param>
    public void TeleportToWorldCamera(Vector3 targetPos)
    {
        WorldVirtualCam.transform.position = new Vector3(targetPos.x, _WORLD_MAP_CAMERA_OFFSET[1], targetPos.z + _WORLD_MAP_CAMERA_OFFSET[2]);
    }
    
    public void  ReTargetToWorldCamera(Vector3 playerPos) 
    {
        // ���� �÷��̾ �����ɴϴ�.
        Vector3 nextPlayer = Managers.Turn.PlayerTurn.transform.position;

        // ���� �÷��̾��� ��ġ�� �������� ī�޶� �ε巴�� �̵�
        Vector3 startPos = WorldVirtualCam.transform.position;
        Vector3 endPos = new Vector3(playerPos.x, _WORLD_MAP_CAMERA_OFFSET[1], playerPos.z + _WORLD_MAP_CAMERA_OFFSET[2]);
        StartCoroutine(MoveWorldCamera(startPos, endPos));
    }

    private IEnumerator MoveWorldCamera(Vector3 startPos, Vector3 endPos)
    {
        _worldCameraIsBusy = true;
        //print($"ī�޶� ������ġ {startPos} / ������ġ {endPos}");
        
        float elapsedTime = 0f;
        float totalTime = Vector3.Distance(startPos, endPos) / _WORLD_CAMERA_AUTO_MOVE_SPEED;
       // print($"ī�޶� �̵� �ð� : {totalTime}");

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / totalTime);
            Nowcam.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        WorldVirtualCam.transform.position = endPos;
        _worldCameraIsBusy = false;
    }
    
    #endregion
    
    #region ���� ī�޶�

    public void SetStoreCamera(UI_StorePopUp.StoreType storeType, Action callback)
    {
        // ī�޶� ���� ���¸� �������� ����
        nowState = NowState.STORE;

        // ī�޶� ���� ��ġ�� �� ������ �´� ��ġ�� ����
        ChangeCamera(Nowcam, _shopCamerablendList[(int)storeType]);
        _storeCameraVolume = Nowcam.GetComponent<CinemachineVolumeSettings>();

        //���� ����ϴ� ������ ����� ī�޶��� �������� �ٲٱ�
        ChangeVolume(CameraVolumeType.Store);
        BlurOn(_nowVolum);
        

        //�� blurȿ�� ó��
        CinemachineBlendListCamera nowBlendCamera = (CinemachineBlendListCamera)Nowcam;
        float blurTime = nowBlendCamera.m_Instructions[0].m_Hold + nowBlendCamera.m_Instructions[1].m_Blend.m_Time;
        
        StartCoroutine(CameraBlendingCheck(blurTime, callback));
        
        BlurEffect(false, blurTime+1.5f);
        
        
        // FadeOut ���� ����
        FadeOut.FadeIn(2f, new Color(255, 255, 255));

    }

    private IEnumerator CameraBlendingCheck(float blendTime, Action callBack)
    {
        yield return new WaitForSeconds(blendTime);
        callBack?.Invoke();
    }


    #endregion

    #region �κ��丮 Dummy ī�޶�

    public void InitInventoryDummyCamera(WorldMapPlayerCharacter[] inventoryDummy)
    {
        _inventoryDummyCamera = new Camera[inventoryDummy.Length];

        for (int i = 0; i < inventoryDummy.Length; i++)
        {
            _inventoryDummyCamera[i] = inventoryDummy[i].transform.GetChild(_INVENTORY_DUMMY_CAM_INDEX).GetComponent<Camera>();

            // �κ��丮 Dummy ī�޶� ��Ȱ��ȭ
            _inventoryDummyCamera[i].enabled = false;
        }
    }

    public void ToggleInventoryDummyCamera(int worldCharacterIndex)
    {
        _inventoryDummyCamera[worldCharacterIndex].enabled = !_inventoryDummyCamera[worldCharacterIndex].enabled;
    }
    

    #endregion
}
