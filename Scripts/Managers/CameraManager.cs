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

    #region 인벤토리 Dummy 카메라

    // 인벤토리 Dummy 전용 레이어
    public readonly int INVENTORY_DUMMY_LAYER = 9;

    private const int _INVENTORY_DUMMY_CAM_INDEX = 4;

    private Camera[] _inventoryDummyCamera;

    #endregion
    
    #region 월드맵 카메라 좌표 및 이동속도


    public CinemachineVirtualCameraBase WorldVirtualCam;//월드에서 사용하는 가상카메라

    public CinemachineVirtualCameraBase Nowcam;//현재 사용하고 있는 가상카메라



    // 카메라 고정 좌표
    private readonly float[] _WORLD_MAP_CAMERA_OFFSET = {0f, 12f, -10f};
    
    // 카메라 자동 이동 속도
    private readonly float _WORLD_CAMERA_AUTO_MOVE_SPEED = 50f;

    // 카메라 수동 이동 속도
    private readonly float _WORLD_CAMERA_MANUAL_SPEED = 15f;
    
    private readonly string[] _INPUT_KEY_BOARD = new string[2] { "Horizontal", "Vertical" };

    // 카메라 연출 중에 사용자 W,A,S,D 막을 예외처리
    private bool _worldCameraIsBusy = false;

    #endregion

    #region 상점 카메라

    
    [Header("각 상점 카메라 시작 위치")]
    [SerializeField] private CinemachineBlendListCamera[] _shopCamerablendList = new CinemachineBlendListCamera[3];
   
    #endregion

    #region 카메라 포스트 프로세싱 볼륨

    

    private CinemachineVolumeSettings _battleCameraVolume;//배틀카메라의 볼륨
    private CinemachineVolumeSettings _storeCameraVolume;//상점카메라의 볼륨

    private CinemachineVolumeSettings _nowVolum;//현재 사용할 볼륨

    private Coroutine _cameraBlurCoroutine;

    #endregion

    //배틀 시작했을 떄 볼륨값
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
    public UI_FadeOut FadeOut; //전투 진입시 페이드아웃할 이미지

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
        
        // 카메라 상점 연출 (Blur 연출함수 이벤트 등록)
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
    /// 카메라 전환 함수
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
    /// 현재 사용할 볼륨 변경
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
    
    #region 플로팅이미지
    
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

    #region 배틀 카메라
    public void SetBattleCamera(Transform Parent)
    {
        nowState = NowState.BATTLE;
        BattleCamPos.Clear();
        //이동할 맵의 블렌드카메라 정보
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


        // 흔들림 효과를 활성화합니다.
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        // 일정 시간 후에 흔들림 효과를 비활성화합니다.
        StartCoroutine(StopShake(duration, virtualCamera));
    }
    private IEnumerator StopShake(float duration, CinemachineVirtualCamera virtualCamera)
    {
        yield return new WaitForSeconds(duration);
        // 흔들림 효과를 비활성화합니다.
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    #endregion

    #region 던전 카메라
    public void SetCaveCamera(Transform Parent)
    {
        nowState = NowState.CAVE;
        CaveCamPos.Clear();
        _caveBeforecam = 3;
        CaveCamIndex = 3;
        //이동할 맵의 블렌드카메라 정보
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
            if (Managers.Cave.NowEvent == CaveManager.CaveEvent.SAFE)//세이프존일 경우 페이드 아웃일때 오브젝트 off함
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


    #region blur효과
    public void BlurEffect(bool increaseBlur, float blurTimer)// Blur 효과 활성화 여부, Blur 진행시간
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
                float t = elapsedTime / blurTime; // 시간에 따른 보간 가중치
                float blurValue;

                /*
                if (increaseBlur)
                {
                   
                }
                else
                {
                    blurValue = Mathf.Lerp(endBlurValue, startBlurValue, t); // 수정: 끝 값부터 시작 값으로 보간
                }
                */


                blurValue = Mathf.Lerp(startBlurValue, endBlurValue, t); // 수정: 시작 값부터 끝 값으로 보간

                depthOfField.focalLength.value = blurValue;

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            // depthOfField.focalLength.value = endBlurValue; // 최종 목표 값 적용

            /*
            while (elapsedTime < blurTime)
            {
                
                float normalizedTime = elapsedTime / blurTime;
                float smoothStepValue = Mathf.SmoothStep(0f, 1f, normalizedTime);
                float blurValue;
                blurValue = Mathf.Lerp(startBlurValue, endBlurValue, smoothStepValue); // 수정: 시작 값부터 끝 값으로 보간
                
                if (increaseBlur)
                {
                    blurValue = Mathf.Lerp(startBlurValue, endBlurValue, smoothStepValue); // 수정: 시작 값부터 끝 값으로 보간
                }
                else
                {
                    blurValue = Mathf.Lerp(endBlurValue, startBlurValue, smoothStepValue); // 수정: 끝 값부터 시작 값으로 보간
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
        // Blur 효과 OFF
        volume.m_Profile.TryGet(out DepthOfField bluerEffect);  
        bluerEffect.focalLength.value = bluerEffect.focalLength.min;
        bluerEffect.focusDistance.value = 1f;
    }

    private void BlurOn(CinemachineVolumeSettings volume)
    {
        // Blur 효과 ON
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

    #region 월드 카메라
    
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
    /// 카메라가 부드럽게 연출이 아닌 순간이동이 필요한 경우
    /// </summary>
    /// <param name="targetPos">목적지 좌표</param>
    public void TeleportToWorldCamera(Vector3 targetPos)
    {
        WorldVirtualCam.transform.position = new Vector3(targetPos.x, _WORLD_MAP_CAMERA_OFFSET[1], targetPos.z + _WORLD_MAP_CAMERA_OFFSET[2]);
    }
    
    public void  ReTargetToWorldCamera(Vector3 playerPos) 
    {
        // 다음 플레이어를 가져옵니다.
        Vector3 nextPlayer = Managers.Turn.PlayerTurn.transform.position;

        // 다음 플레이어의 위치와 방향으로 카메라를 부드럽게 이동
        Vector3 startPos = WorldVirtualCam.transform.position;
        Vector3 endPos = new Vector3(playerPos.x, _WORLD_MAP_CAMERA_OFFSET[1], playerPos.z + _WORLD_MAP_CAMERA_OFFSET[2]);
        StartCoroutine(MoveWorldCamera(startPos, endPos));
    }

    private IEnumerator MoveWorldCamera(Vector3 startPos, Vector3 endPos)
    {
        _worldCameraIsBusy = true;
        //print($"카메라 시작위치 {startPos} / 종료위치 {endPos}");
        
        float elapsedTime = 0f;
        float totalTime = Vector3.Distance(startPos, endPos) / _WORLD_CAMERA_AUTO_MOVE_SPEED;
       // print($"카메라 이동 시간 : {totalTime}");

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
    
    #region 상점 카메라

    public void SetStoreCamera(UI_StorePopUp.StoreType storeType, Action callback)
    {
        // 카메라 현재 상태를 상점으로 변경
        nowState = NowState.STORE;

        // 카메라 시작 위치를 각 상점에 맞는 위치로 설정
        ChangeCamera(Nowcam, _shopCamerablendList[(int)storeType]);
        _storeCameraVolume = Nowcam.GetComponent<CinemachineVolumeSettings>();

        //현재 사용하는 볼륨을 스토어 카메라의 볼륨으로 바꾸기
        ChangeVolume(CameraVolumeType.Store);
        BlurOn(_nowVolum);
        

        //역 blur효과 처리
        CinemachineBlendListCamera nowBlendCamera = (CinemachineBlendListCamera)Nowcam;
        float blurTime = nowBlendCamera.m_Instructions[0].m_Hold + nowBlendCamera.m_Instructions[1].m_Blend.m_Time;
        
        StartCoroutine(CameraBlendingCheck(blurTime, callback));
        
        BlurEffect(false, blurTime+1.5f);
        
        
        // FadeOut 연출 실행
        FadeOut.FadeIn(2f, new Color(255, 255, 255));

    }

    private IEnumerator CameraBlendingCheck(float blendTime, Action callBack)
    {
        yield return new WaitForSeconds(blendTime);
        callBack?.Invoke();
    }


    #endregion

    #region 인벤토리 Dummy 카메라

    public void InitInventoryDummyCamera(WorldMapPlayerCharacter[] inventoryDummy)
    {
        _inventoryDummyCamera = new Camera[inventoryDummy.Length];

        for (int i = 0; i < inventoryDummy.Length; i++)
        {
            _inventoryDummyCamera[i] = inventoryDummy[i].transform.GetChild(_INVENTORY_DUMMY_CAM_INDEX).GetComponent<Camera>();

            // 인벤토리 Dummy 카메라 비활성화
            _inventoryDummyCamera[i].enabled = false;
        }
    }

    public void ToggleInventoryDummyCamera(int worldCharacterIndex)
    {
        _inventoryDummyCamera[worldCharacterIndex].enabled = !_inventoryDummyCamera[worldCharacterIndex].enabled;
    }
    

    #endregion
}
