using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class WorldMapPlayerCharacter : MonoBehaviour, I_Dummy
{
	
	#region 플레이어 이동력 관련
    
    // 플레이어 Turn 아이콘 Bill Board
    private PlayerTurnIconBillBoard _playerTurnIconBillBoard;

    public int MovementPoints { get; private set; } = 3;

    public string[] ResultMovementSlotsIcon { get; private set; }

    #endregion
    
    #region 이벤트 핸들러
    
    public event Action<bool> CheckPlayerTurn;       // 플레이어 턴 준비 또는 종료 (Ture : 준비, False : 종료)
    
    private static event Action<WorldMapPlayerCharacter> _onPlayerTurnAlarmEvent;
	
    public static event Action<WorldMapPlayerCharacter> OnPlayerTurnAlarmEvent
    {
	    add
	    {
		    // 마지막에 있던 이벤트 제거
		    if (_onPlayerTurnAlarmEvent != null)
			    _onPlayerTurnAlarmEvent = null;

		    // 새로운 이벤트 등록
		    _onPlayerTurnAlarmEvent += value;
	    }
	    remove
	    {
		    _onPlayerTurnAlarmEvent -= value;
	    }
    }
    
    public event Action<WorldMapPlayerCharacter> MovementFinished;        // 플레이어 이동종료 이벤트 핸들러
    
    public event Action<int> UpdateTurnCountIcon;      // 플레이어 남은 이동력 업데이트 이벤트 핸들러
    
    public event Func<WorldMapPlayerCharacter, Hex, bool, bool> CheckHexType;  // 플레이어 현재 위치에 Hex Type 확인
    
    [HideInInspector]
    public bool IsPlayerInEvent = false; // 현재 플레이어가 이벤트 Hex에 있는 경우

    #endregion

    #region 이동연출

    public enum MovementMode
    {
        Normal = 0,
        Attack,
        AttackFinish,
        Retreat,
    }

    // 이동 연출효과 (이동지속시간, 회전지속시간)
    private static readonly float _movementDuration = 1;
    
    private static readonly float _rotationDuration = 0.3f;
    
    private static readonly float _groupRadius = 0.6f;

    private static readonly float _hexAttackPosScale = 0.3f;

    private static readonly float _SCALE_TINY_VALUE = 0.35f;
    
    private static readonly Vector3 _DEFAULT_SCALE = Vector3.one;
    
    private static readonly Vector3 _TINY_SCALE = Vector3.one * _SCALE_TINY_VALUE;
    
	// 월드맵 캐릭터 자식 오브젝트 뼈대 위치
    private static readonly int _WORLD_CHARACTER_ROOT_INDEX = 2;
    
    // 인벤토리 캐릭터 자식 오브젝트 뼈대 위치
    private static readonly int _INVENTORY_CHARACTER_ROOT_INDEX = 3;
    
    // 월드맵 캐릭터 뼈대 오브젝트
    private Transform _worldCharacterRoot; 
    
    private bool _isMicroMoveRunning = false;

    // 플레이어 및 Hex 경로 쉐이더
    private GlowHighlight _glowHighlight;
    
    #endregion

    #region 플레이어 이동경로

    // Hex 이동경로
    private Queue<Vector3> _pathPositions = new Queue<Vector3>();
    
    private Queue<Hex> _pathHexInfo = new Queue<Hex>();
    
    [HideInInspector]
    // 플레이어 마지막 Hex 위치
    public Hex LastHex = null;

    // 플레이어 현재 Hex 위치
    private Hex _currentHex;
    
    public Hex CurrentHex
    {
	    get => _currentHex;
	    set
	    {
		    _currentHex = value;
		    if (!LastHex)
		    {
			    LastHex = _currentHex;
		    }
	    } 
    }

    #endregion

    #region  플레이어 Dummy 정보
    
    // 월드맵 캐릭터 Dummy 장비 위치
    private Dictionary<I_Dummy.CharacterPartsTransform, Transform> _worldMapCharacterDummy;

    // 인벤토리 캐릭터 Dummy 장비 위치
    private Dictionary<I_Dummy.CharacterPartsTransform, Transform> _inventoryCharacterDummy;

    private GameObject _inventoryCharacter;


    //private readonly StatRoll _statRoll = new StatRoll();
    
    public PlayerStats PlayerStats { get; private set; }
    
    public CharacterEventListener CharacterEventListener { get; private set; }

    #endregion
    
    private void Awake()
    {
        CharacterEventListener = new CharacterEventListener(GetComponent<Animator>());

        var ren = this.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        
        CharacterEventListener.UpdateCharacterRenderer(ren);
        _worldCharacterRoot = this.transform.GetChild(_WORLD_CHARACTER_ROOT_INDEX);
        _glowHighlight = GetComponent<GlowHighlight>();
        _playerTurnIconBillBoard = GetComponentInChildren<PlayerTurnIconBillBoard>();
        _glowHighlight.Init();
        _playerTurnIconBillBoard.Init(this);

        // 월드 캐릭터와 인벤토리 캐릭터 Dummy 초기화
        SetDummyCharacterItemPartsTransform(_worldCharacterRoot);
        
        StartCoroutine(testCoru());
    }

  
    private IEnumerator testCoru()
    {
	    
	    yield return new WaitForSeconds(5f);
	    PlayerStats.UpdateGold(20);
	    
	    /*
	    PlayerStats.UpdateXp(10);
	 PlayerStats.UpdateCurrentHealth(10);
	  
	  yield return new WaitForSeconds(5f);
	    
	  PlayerStats.UpdateXp(10);

	   yield return new WaitForSeconds(5f);
	   PlayerStats.UpdateCurrentHealth(25);
	   */
	   
	   
	    /*
	    PlayerStats.MX += 3;
	    yield return new WaitForSeconds(5f);
	    PlayerStats.UpdateCurrentFocus(3, true);
	    yield return new WaitForSeconds(5f);
	    PlayerStats.UpdateXp(100, true);
	    yield return new WaitForSeconds(5f);
	    PlayerStats.UpdateCurrentFocus(3, false);
	    yield return new WaitForSeconds(5f);
	    PlayerStats.MX = -3;
	    */
    }


    public void WorldMapPlayerInit(PlayerStats playerStats)
    {
	    // 플레이어 정보 초기 설정
        PlayerStats = playerStats;
    }

    public void PlayerInventoryInit()
    {
	    // 월드맵 캐릭터 Dummy 정보 초기 설정
	    PlayerStats.WorldMapCharacterDummy = this;
	    // 플레이어 인벤토리 초기 설정
	    PlayerStats.InitPlayerInventory();
    }

    /// <summary>
    /// 플레이어 하이라이트 머티리얼 비활성화
    /// </summary>
    public void Deselect()
    {
        _glowHighlight.ToggleGlow(false);
    }

    /// <summary>
    /// 플레이어 하이라이트 머티리얼 활성화
    /// </summary>
    public void Select()
    {
        _glowHighlight.ToggleGlow();
    }

    #region 이동 함수
    
    public void MoveThroughPath(List<Vector3> currentPath, List<Hex> currentHex)
    {
        _pathPositions = new Queue<Vector3>(currentPath);
        _pathHexInfo = new Queue<Hex>(currentHex);
        Vector3 firstTarget = _pathPositions.Dequeue();

        // 플레이어 턴 종료 버튼 비활성화
        Managers.UIManager.InteractionTurnOverButton(false);
        
        StartCoroutine(RotationCoroutine(firstTarget, _rotationDuration));
    }

    private IEnumerator RotationCoroutine(Vector3 endPosition, float rotationDuration)
    {
        Quaternion startRotation = transform.rotation;
        endPosition.y = transform.position.y;
        Vector3 direction = endPosition - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false)
        {
            float timeElapsed = 0;
            while (timeElapsed < _rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                yield return null;
            }

            transform.rotation = endRotation;
        }
        CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Run);
        StartCoroutine(MovementCoroutine(_pathHexInfo.Dequeue(), endPosition));
    }

    private IEnumerator MovementCoroutine(Hex hex, Vector3 endPosition)
    {
	    Vector3 startPosition = transform.position;
	    Vector3 startScale;
	    Vector3 endScale;
	    bool isVisible;
	    bool destVisible;
	    bool needChangeScale = false;

	    // 전투지역인 경우 걸치기
	    if (hex.BattleZoneType != BattleZoneType.None)
	    {
		    Vector3 vector = endPosition - transform.position;
		    endPosition = transform.position + _hexAttackPosScale * vector;
	    }

	    endPosition.y = startPosition.y;
	    float timeElapsed = 0;

	    if (!hex.IsScaleDownGroundPlayer)  LeaveHexPartyShuffle(ref endPosition, hex, MovementPoints, _pathPositions.Count);
	    
	    // 현재 hex 위치에 플레이어 크기가 Tiny인 경우
	    if (_currentHex.IsScaleDownGroundPlayer)
	    {
		    startScale = _TINY_SCALE;
		    isVisible = true;
		    destVisible = false;
		    needChangeScale = true;
	    }
	    else
	    {
		    startScale = _DEFAULT_SCALE;
		    isVisible = true;
		    destVisible = true;
	    }

	    // 다음 도착지 Hex 위치에 플레이어 크기를 Tiny가 요청되는 경우
	    if (hex.IsScaleDownGroundPlayer)
	    {
		    endScale = _TINY_SCALE;
		    isVisible = true;
		    destVisible = false;
		    needChangeScale = true;
	    }
	    else
	    {
		    endScale = _DEFAULT_SCALE;
		    isVisible = true;
		    destVisible = true;
	    }

	    // 캐릭터가 이동을 하니깐 머티리얼 활성화
	    CharacterEventListener.IsVisible = isVisible;
	    
	    _currentHex.OnPlayerExitHex(this);
	    hex.OnPlayerEnterHex(this);
	    
        while (timeElapsed < _movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / _movementDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpStep);
            if(needChangeScale) _worldCharacterRoot.localScale = Vector3.Lerp(startScale, endScale, lerpStep);
            yield return null;
        }
        
        // 캐릭터가 이동을 완료 했으니 해당 Hex 지역에 맞게 머티리얼 설정
        CharacterEventListener.IsVisible = destVisible;
        transform.position = endPosition;

        // 이동력 감소
        DecreaseMovementPoint();

        // 아직 이동해야할 경로가 남았다면
        if (_pathPositions.Count > 0)
        {
            // 이동중일 경우 전투존 && 이벤트존 && 퀘스트존 만 확인함
            if (CheckHexType?.Invoke(this, hex, true) == true)
            {
	            CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Idle);
                MovementFinished?.Invoke(this);
                yield break;
            }
            
            StartCoroutine(RotationCoroutine(_pathPositions.Dequeue(), _rotationDuration)); // 이동을 완료 했으니 회전 다시 시작
        }
        else
        {
	        CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Idle);
            // 모든 이동이 완료 되었으면 현재 위치의 Hex 이벤트 종류를 모두 확인함
            CheckHexType?.Invoke(this, hex, false);
            Managers.UIManager.InteractionTurnOverButton(true);
            MovementFinished?.Invoke(this);
        }
        
        Managers.WorldMapGenerator.RemoveHexCloud(hex);
    }

    /// <summary>
    /// 특정 지역(Hex)으로 자동으로 플레이어를 이동하는 함수
    /// </summary>
    /// <param name="destHex">이동할 지역 (기본 값 null)</param>
    public void AutoMovingHex (Hex destHex)
    {
	    UnitManager.Instacne.AutoMovingHex(this, destHex);
    }
    
    private void LeaveHexPartyShuffle(ref Vector3 destPos, Hex destHex, int _moveCount, int _pathLength)
	{                                            // 해당 Hex에 분산할 연출이 필요없는 경우
		if (_currentHex.GetInHexPlayerCount() > 1)// && !m_HexLand.IsScaleDownGroundPlayer())
		{
			Vector3 vector = destPos - _currentHex.transform.position;
			vector.y = 1f;
		//	print(vector);
			vector.Normalize();
		//	print(vector);
			if (_currentHex.GetInHexPlayerCount() == 2)
			{
				WorldMapPlayerCharacter worldMapPlayerCharacter = _currentHex.GetInHexWorldMapPlayer(0);
				if (worldMapPlayerCharacter == this)
				{
					worldMapPlayerCharacter = _currentHex.GetInHexWorldMapPlayer(1);
				}
				worldMapPlayerCharacter.StartMicroMove(worldMapPlayerCharacter.transform.position, _currentHex.transform.position, worldMapPlayerCharacter.transform.forward, vector, this);
			}
			else if (destHex.GetInHexPlayerCount() == 3)
			{
				WorldMapPlayerCharacter characterOverworld2 = destHex.GetInHexWorldMapPlayer(0);
				WorldMapPlayerCharacter characterOverworld3 = destHex.GetInHexWorldMapPlayer(1);
				if (characterOverworld2 == this)
				{
					characterOverworld2 = _currentHex.GetInHexWorldMapPlayer(2);
				}
				else if (characterOverworld3 == this)
				{
					characterOverworld3 = _currentHex.GetInHexWorldMapPlayer(2);
				}
				Vector3 vector2 = Quaternion.Euler(0f, 90f, 0f) * vector;
				Vector3 vector3 = Quaternion.Euler(0f, -90f, 0f) * vector;
				Vector3 vector4 = _currentHex.transform.position + vector2 * _groupRadius; //GameLogic.Instance.m_GroupRadius;
				Vector3 vector5 = _currentHex.transform.position + vector3 * _groupRadius; //GameLogic.Instance.m_GroupRadius;
				float sqrMagnitude = (vector4 - characterOverworld2.transform.position).sqrMagnitude;
				float sqrMagnitude2 = (vector5 - characterOverworld2.transform.position).sqrMagnitude;
				float sqrMagnitude3 = (vector4 - characterOverworld3.transform.position).sqrMagnitude;
				float sqrMagnitude4 = (vector5 - characterOverworld3.transform.position).sqrMagnitude;
				Vector3 otherPlayersDest;
				Vector3 otherPlayersDest2;
				 GetMinDist(out otherPlayersDest, out otherPlayersDest2, sqrMagnitude, sqrMagnitude2, sqrMagnitude3, sqrMagnitude4, vector4, vector5);
				characterOverworld2.StartMicroMove(characterOverworld2.transform.position, otherPlayersDest, characterOverworld2.transform.forward, vector, this);
				characterOverworld3.StartMicroMove(characterOverworld3.transform.position, otherPlayersDest2, characterOverworld3.transform.forward, vector, this);
			}
		}
		if (destHex.GetInHexPlayerCount() <= 0 )//|| destHex.IsScaleDownGroundPlayer())
		{
			return;
		}
		Vector3 vector6 = destHex.transform.position - base.transform.position;
		vector6.y = 1f;
		vector6.Normalize();
		bool flag = _moveCount == _pathLength - 1;
		if (flag)
		{
			destPos += vector6 *  _groupRadius; //GameLogic.Instance.m_GroupRadius;
		}
		float num = 120f;
		float num2 = 1f;
		if (!flag)
		{
			num = 90f;
			num2 = 1.5f;
		}
		Vector3 vector7 = Quaternion.Euler(0f, num, 0f) * vector6;
		Vector3 vector8 = Quaternion.Euler(0f, 0f - num, 0f) * vector6;
		Vector3 vector9 = destHex.transform.position + vector7 * _groupRadius * num2;// GameLogic.Instance.m_GroupRadius * num2;
		Vector3 vector10 = destHex.transform.position + vector8 * _groupRadius * num2;// GameLogic.Instance.m_GroupRadius * num2;
		if (destHex.GetInHexPlayerCount() == 1)
		{
			WorldMapPlayerCharacter characterOverworld4 = destHex.GetInHexWorldMapPlayer(0);
			float sqrMagnitude5 = (vector9 - characterOverworld4.transform.position).sqrMagnitude;
			float sqrMagnitude6 = (vector10 - characterOverworld4.transform.position).sqrMagnitude;
			Vector3 vector11 = vector9;
			if (sqrMagnitude6 < sqrMagnitude5)
			{
				vector11 = vector10;
			}
			Vector3 destLook = vector11 - destHex.transform.position;
			if (!flag)
			{
				destLook *= -1f;
			}
			destLook.Normalize();
			characterOverworld4.StartMicroMove(characterOverworld4.transform.position, vector11, characterOverworld4.transform.forward, destLook, (!flag) ? this : null);
		}
		else if (destHex.GetInHexPlayerCount() == 2)
		{
			WorldMapPlayerCharacter characterOverworld5 = destHex.GetInHexWorldMapPlayer(0);
			WorldMapPlayerCharacter characterOverworld6 = destHex.GetInHexWorldMapPlayer(1);
			float sqrMagnitude7 = (vector9 - characterOverworld5.transform.position).sqrMagnitude;
			float sqrMagnitude8 = (vector10 - characterOverworld5.transform.position).sqrMagnitude;
			float sqrMagnitude9 = (vector9 - characterOverworld6.transform.position).sqrMagnitude;
			float sqrMagnitude10 = (vector10 - characterOverworld6.transform.position).sqrMagnitude;
			Vector3 otherPlayersDest3;
			Vector3 otherPlayersDest4;
			GetMinDist(out otherPlayersDest3, out otherPlayersDest4, sqrMagnitude7, sqrMagnitude8, sqrMagnitude9, sqrMagnitude10, vector9, vector10);
			Vector3 destLook2 = otherPlayersDest3 - destHex.transform.position;
			destLook2.Normalize();
			Vector3 destLook3 = otherPlayersDest4 - destHex.transform.position;
			destLook3.Normalize();
			if (!flag)
			{
				destLook2 *= -1f;
				destLook3 *= -1f;
			}
			characterOverworld5.StartMicroMove(characterOverworld5.transform.position, otherPlayersDest3, characterOverworld5.transform.forward, destLook2, (!flag) ? this : null);
			characterOverworld6.StartMicroMove(characterOverworld6.transform.position, otherPlayersDest4, characterOverworld6.transform.forward, destLook3, (!flag) ? this : null);
		}
	}
    
    private void StartMicroMove(Vector3 _startPos, Vector3 _destPos, Vector3 _startLook, Vector3 _destLook, WorldMapPlayerCharacter _lookAtCow)
    {
	    if (_isMicroMoveRunning)
		{
		    StopCoroutine("MicroMove");
		    CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Idle);
		   // SetOverworldAnimRPC("idle");
		   StartCoroutine(MicroMove(base.transform.position, _destPos, base.transform.forward, _destLook, _lookAtCow));
		}
		else
		{
		    StartCoroutine(MicroMove(_startPos, _destPos, _startLook, _destLook, _lookAtCow));
		}
    }

    private IEnumerator MicroMove(Vector3 _startPos, Vector3 _destPos, Vector3 _startLook, Vector3 _destLook, WorldMapPlayerCharacter _lookAtCow)
    {
	    _isMicroMoveRunning = true;
	    //SetOverworldAnimRPC("walk");
	    CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Run);
	    
	    float positionAlpha2 = 0f;
	    float actualMoveSpeed = 1f;// GameLogic.Instance.m_MicroMoveSpeed;
	    Vector3 direction = _destPos - _startPos;
	   // print($"direction : {direction} = {_destPos} - {_startPos}");
	    float moveDistance = direction.magnitude;
	    if (moveDistance.AlmostEquals(0f, 0.001f))
	    {
		    if ((bool)_lookAtCow)
		    {
			  //  base.transform.LookAt(new Vector3(_lookAtCow.transform.position.x , _lookAtCow.transform.position.y, 0f)); 
			  base.transform.LookAt(_lookAtCow.transform.position); 
		    }
		    else
		    {
			    base.transform.LookAt(base.transform.position + _destLook);
		    }
		    yield return null;
	    }
	    else
	    {
		    direction.Normalize();
		  //  direction.y = 1f;
		   // print($"direction(정규화) : {direction}");
		    while (true)
		    {
			    //newPosition = new Vector3(base.transform.position.x, 1f, base.transform.position.z);
			   // newPosition += actualMoveSpeed * Time.deltaTime * direction;
			  //  base.transform.position += newPosition;
			  base.transform.position += (actualMoveSpeed * Time.deltaTime * direction);// + new Vector3(0f, 1f, 0f);
			  Vector3 diff = base.transform.position - _startPos;
		//	  print($"diff : {diff}");
			    float proj = Vector3.Dot(diff, direction);
			    if (proj > 0.9f)//0.99f
			    {
				    break;
			    }
			    positionAlpha2 = Mathf.Clamp01(diff.magnitude / moveDistance);
			    Vector3 lookAt = Vector3.Slerp(_startLook, direction, positionAlpha2);
			  //  lookAt.x = 0f;
			   // lookAt.z = 0f;
			    if ((bool)_lookAtCow)
			    {
				    //base.transform.LookAt(new Vector3(_lookAtCow.transform.position.x , _lookAtCow.transform.position.y, 0f)); 
				    base.transform.LookAt(_lookAtCow.transform.position); //  base.transform.LookAt(new Vector3(0f,_lookAtCow.transform.position.y*-1, 0f));
				  //  base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.y, 0f);
			    }
			    else
			    {
				 //   base.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y, 0f) + lookAt);
				   // base.transform.LookAt(base.transform.position + lookAt);
			    }
			    if (positionAlpha2 > 0.5f)
			    {
				    break;
			    }
			    yield return null;
		    }
	    }

	   // print($"{PlayerStats.BaseClassName}가 {_destLook}를 바라봄");
	    _destPos.y = 1;
	    base.transform.position = _destPos; 

	   base.transform.LookAt( base.transform.position +_destLook);
	 //  print($"{PlayerStats.BaseClassName}의 {transform.rotation}값. ");
	   base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
	    CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Idle);
	  //  SetOverworldAnimRPC("combatIdle");
	    _isMicroMoveRunning = false;
    }
   
    private void GetMinDist(out Vector3 otherPlayersDest0, out Vector3 otherPlayersDest1, float dist00, float dist01, float dist10, float dist11, Vector3 pos0, Vector3 pos1)
	{
		if (dist00 < dist01)
		{
			if (dist00 < dist10)
			{
				otherPlayersDest0 = pos0;
				otherPlayersDest1 = pos1;
			}
			else
			{
				otherPlayersDest0 = pos1;
				otherPlayersDest1 = pos0;
			}
		}
		else if (dist01 < dist11)
		{
			otherPlayersDest0 = pos1;
			otherPlayersDest1 = pos0;
		}
		else
		{
			otherPlayersDest0 = pos0;
			otherPlayersDest1 = pos1;
		}
	}

    #endregion
    
    #region 이동력 부여 함수
    
    
    /// <summary>
    /// 이동력 값이 변경될 때마다 이벤트 핸들러 호출
    /// </summary>
    private void UpdateTurnCount()
    {
        UpdateTurnCountIcon?.Invoke(MovementPoints);
    }

    /// <summary>
    /// 플레이어 턴 시작 (외부에서 호출)
    /// </summary>
    /// <param name="movementPoints">이동력 (최소한 1은 부여됨)</param>
    public void SetPlayerMovementPoints()
    {
        int terrainSlots = WorldMapHexMovementSlot.GetHexTerrainSlots(LastHex.HexType);
        int baseSlots =  WorldMapHexMovementSlot.GetHexBaseSlots(LastHex.HexType);
		

        // 최소한 1값은 줌
        terrainSlots = Mathf.Max(terrainSlots, 1);
        baseSlots = Mathf.Max(baseSlots, 1);

        PlayerStats.BaseMovement = terrainSlots + baseSlots;
        
        // TODO : 아이템으로 인한 추가 이동력은 baseSlots으로 추가해서 보정한다. 
        
        // 최종적으로 이동할 수 있는 Max Slot 갯수
        int movementMaxPoints = PlayerStats.MaxMovements;
        ResultMovementSlotsIcon = new string[movementMaxPoints];
        int hitCount = 0;
        
        /*  총 3가지 경우를 평가해서 이동슬롯을 부여함
         *  1. 기본적으로 주어진 보정슬롯(황금색) => 무조건 확정
         *  2. 플레이어 이동 능력치를 계산하여 정함 
         *    2-1. 기본적으로 확률로 정해서 Miss가 발생할 수 도 있음
         *    2-2. 날씨 방해를 받을경우 Miss가 발생
         */
        for (int i = 0; i < movementMaxPoints; i++)
        {
            if (i < baseSlots)
            {
                // 황금색 이동 슬롯 확정
                ResultMovementSlotsIcon[i] = "base";
            }
            else if(StatRoll.RandomFunc(PlayerStats.GetStatValue(PlayerStats.StatType.Quickness)))
            {
                // 이동속도 스텟기준으로 성공결정
                
                // TODO : 날씨로 인한 악영향 if(badWeather == true) { resultMovementSlotsIcon[i] ="badWeather"; }

                ResultMovementSlotsIcon[i] = "hit";
                hitCount++;
            }
            else
            {
                // 이동속도 스텟획득 실패
                ResultMovementSlotsIcon[i] = "miss";
            }
        }

        MovementPoints = (baseSlots + hitCount);
        // 1. Turn Icon 활성화, 2. Turn Button 활성화
        CheckPlayerTurn?.Invoke(true); 
        _onPlayerTurnAlarmEvent?.Invoke(this);
        UpdateTurnCount();
    }
    
    /// <summary>
    /// 플레이어 이동력 감소
    /// </summary>
    private void DecreaseMovementPoint()        
    {
        if (MovementPoints > 0)
        {
            MovementPoints--;
            UpdateTurnCount();
        }
    }

    
    /// <summary>
    /// 플레이어 턴 종료
    /// </summary>
    public void EndOfTurnPlayer()
    {
        //  해당 플레이어랑 관련된 Hex 및 플레이어 쉐이더 모두 끄기 
        UnitManager.Instacne.ClearOldSelection();
        //  해당 플레이어는 턴 종료 후 이동력 0으로 설정
        MovementPoints = 0;
        // 1. Turn Icon 활성화, 2. Turn Button 활성화
        CheckPlayerTurn?.Invoke(false);
    }
    
    
    #endregion
    
    #region 슬롯 확률 선정 

    // static 클래스로 만들어서 적군과 플레이어 같이 사용하면?
    public enum StatType
    {
        Strength,
        Awareness,
        Intelligence,
        Quickness,
        Vitality,
    }
    /*
    class StatRoll
    {
        
        public bool Roll(PlayerStats owner, PlayerStats.StatType statType)
        {
            bool isSuccess = false;
            float statValue = owner.GetStatValue(statType);
            float randomValue = Random.value;
            float failPercent = Mathf.Floor(randomValue * 100) / 100f;
            isSuccess = failPercent <= statValue;
            
            return isSuccess;
        }
    }*/

    #endregion

    #region Dummy 모델링 업데이트 함수
	
    public void SetDummyCharacterItemPartsTransform(Transform transform)
    {
	    I_Dummy.CharacterPartsTransform[] partsArray = (I_Dummy.CharacterPartsTransform[])Enum.GetValues(typeof(I_Dummy.CharacterPartsTransform));

	   _worldMapCharacterDummy  = new Dictionary<I_Dummy.CharacterPartsTransform, Transform>();
	   _inventoryCharacterDummy = new Dictionary<I_Dummy.CharacterPartsTransform, Transform>();

	   _inventoryCharacter = this.transform.GetChild(_INVENTORY_CHARACTER_ROOT_INDEX).gameObject;
	   
	    foreach (var partName in partsArray)
	    {
		    _worldMapCharacterDummy[partName] = ObjectUtil.FindChild<Transform>(transform.gameObject, partName.ToString(), true);
		    _inventoryCharacterDummy[partName] = ObjectUtil.FindChild<Transform>(_inventoryCharacter, partName.ToString(), true);
	    }
	    
	    // 인벤토리 Dummy 캐릭터 비활성화
	    _inventoryCharacter.SetActive(false);
    }

    // 인벤토리 Dummy 캐릭터 및 카메라 토글
    public void ToggleDummyCharacter()
    {
	    _inventoryCharacter.SetActive(!_inventoryCharacter.activeSelf);
	    Managers.Camera.ToggleInventoryDummyCamera(Managers.Instance.GetPlayerIndex(this));
    }

    public void UpdateCharacterItemPart(Equipment item, bool isEquip)
    {
	    Equipment.EquipmentSlotType itemSlotYType = item.EquipmentSlottype;
	    
	    // 아이템 장착
	    if (isEquip)
	    {
		    Transform inventoryDummyParent = null, worldDummyParent = null;
		    
		    GameObject worldItemObject = Instantiate(Managers.Resource.LoadResource<GameObject>(Managers.Resource.GetItemPrefabPath(item.Itemtype), 
			    item.ItemID + "_" + Managers.Data.GetItemName(item.Itemtype, item.ItemID)));

		    GameObject inventoryItemObject = Instantiate(Managers.Resource.LoadResource<GameObject>(Managers.Resource.GetItemPrefabPath(item.Itemtype), 
			    item.ItemID + "_" + Managers.Data.GetItemName(item.Itemtype, item.ItemID)));

		    // 인벤토리 Dummy 전용 렌더링 설정
		    inventoryItemObject.transform.GetChild(0).gameObject.layer = Managers.Camera.INVENTORY_DUMMY_LAYER;

		    switch (itemSlotYType)
		    {
			    case Equipment.EquipmentSlotType.PrimaryWeapon:
				    
				    var weapon = (Weapon)item;
				    
				    if (weapon.Weapontype is Weapon.WeaponType.WeaponSword)
				    { 
					    inventoryDummyParent  = _inventoryCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
					    worldDummyParent = _worldMapCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
				    }
				    else if (weapon.Weapontype is Weapon.WeaponType.WeaponStaff)
				    {
					    inventoryDummyParent = _inventoryCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
					    worldDummyParent = _worldMapCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
				    }
				    else if (weapon.Weapontype == Weapon.WeaponType.WeaponBow)
				    {
					    inventoryDummyParent = _inventoryCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
					    worldDummyParent = _worldMapCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot];
				    }
				    
				    break;
		    }
		    
		    
		    inventoryItemObject.transform.SetParent(inventoryDummyParent, false);
		    worldItemObject.transform.SetParent(worldDummyParent, false);
		    
			/*
		    inventoryItemObject.transform.parent = inventoryDummyParent;
		    worldItemObject.transform.parent = worldDummyParent;
		    inventoryItemObject.transform.localPosition = inventoryItemObject.transform.position;
		    inventoryItemObject.transform.localRotation = inventoryItemObject.transform.rotation;

		    worldItemObject.transform.localPosition = worldItemObject.transform.position;
		    worldItemObject.transform.localRotation = worldItemObject.transform.rotation;
		    */
			
		    return;
	    }
		
	    // 아이템 해체
	    switch (itemSlotYType)
	    {
		    case Equipment.EquipmentSlotType.PrimaryWeapon:
					Destroy(_worldMapCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot].GetChild(0).gameObject);
					Destroy(_inventoryCharacterDummy[I_Dummy.CharacterPartsTransform.RightHandWeaponRoot].GetChild(0).gameObject);
			    break;
	    }

		
    }

    #endregion

}
