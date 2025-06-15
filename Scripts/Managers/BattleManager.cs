using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraManager;


public class BattleManager : MonoBehaviour
{
    private string _mapName; //배틀맵 이름
    private bool _isFisrt = true; //처음 턴의 유무
    List<int> EnemyId = new List<int>();
    List<PlayerStats> _playerStatsList = new List<PlayerStats>();
    List<BattleUnit> _enemyList = new List<BattleUnit>();
    List<BattleUnit> _playerList = new List<BattleUnit>();
    List<BattleUnit> _unitsList = new List<BattleUnit>();
    
    public BattleUnit CommonUnit;
    [SerializeField]
    private PosRot[] _enemyPos; //적을 배치할 위치
    [SerializeField]
    private PosRot[] _playerPos; //플레이어를 배치할 위치
    //던전용 위치
    private PosRot[] _caveEnemyPos; //적을 배치할 위치
    private PosRot[] _cavePlayerPos; //플레이어를 배치할 위치

    GameObject _unit; //프리펩을 생성하고 저장할 게임오브젝트 변수
    BattleUnit _battleUnit; //프리펩을 생성하고 저장할 BattleUnit 변수
    [SerializeField]
    private UI_BattleTimeLineGrid _uI_BattleTimeLine;
    [SerializeField]
    private UI_Battle _uI_Battle;
    private BattleUnit _inPlayUnit; //현재 플레이중인 플레이어
    private PlayerBattle _inPlayer; //현재 플레이어중인 유닛

    public List<Item> RewardList = new List<Item>();
    public int RewardIndex = 0;
    public bool IsCave = false;
    private event Action<bool> _battleResult;
    private bool _isWin = false;
    private bool _smalleReward = false;

    public List<BattleUnit> EnemyList
    {
        get { return _enemyList; }
        set { _enemyList = value; }
    }
    public List<BattleUnit> PlayerList
    {
        get { return _playerList; }
        set { _playerList = value; }
    }
    public List<BattleUnit> UnitsList
    {
        get { return _unitsList; }
        set { _unitsList = value; }
    }
    public UI_Battle UI_Battle
    {
        get { return _uI_Battle; }
        set { _uI_Battle = value; }
    }
    public BattleUnit _InPlayUnit
    {
        get { return _inPlayUnit; }
        set { _inPlayUnit = value; }
    }
    public PlayerBattle InPlayer
    {
        get { return _inPlayer; }
        set { _inPlayer = value; }
    }

    bool _isCave;
    public struct UnitOrder
    {
        public float Order;
        public BattleUnit Unit;
    }
    int compare(UnitOrder a, UnitOrder b)
    {
        return a.Order < b.Order ? -1 : 1;
    }

    List<UnitOrder> _unitOrders = new List<UnitOrder>(); //유닛과 순서 리스트


    #region 전투 시작 알고리즘
    /// <summary>
    /// 전투 전 각 리스트 초기화 
    /// </summary>
    public void BattleInit(List<PlayerStats> playerList, List<int> enemyList, string mapName, Action<bool> battleResult, List<Item> rewardList, int hexQuestID)
    {
        // 전투가 종료될 경우 전투범위를 먼저 제거하고, 적군 Hex와 모두 초기화 시켜주는 이벤트를 등록함
        IsCave = false;
        _battleResult -= battleResult;
        _battleResult += battleResult;

        RewardList = rewardList;
        if (hexQuestID != -1)
        {
            Quest quest = Managers.Quest.Quests[hexQuestID];
            for(int i = 0; i < quest.Reward.Length; i++)
            {
                RewardList.Add(quest.Reward[i].GiveItem());
            }
        }
        print("보상개수 :" +RewardList.Count);
        
        _unitsList.Clear();
        _enemyList.Clear();
        _playerList.Clear();
        EnemyId.Clear();
        _unitOrders.Clear();
        _playerStatsList = playerList;
        EnemyId = enemyList;
        _mapName = mapName;
        _isCave = false;
        //전투 시작
        BattleStart();
        Managers.Sound.PlayBGM("BattleBGM");
    }

    //던전 전투
    public void CaveBattleInit(List<PlayerStats> playerStatList, List<GameObject> playerDummy, List<int> enemyList, string mapName, List<Item> rewardList, PosRot[] enemy, PosRot[] player)
    {
        IsCave = true;
        _caveEnemyPos = new PosRot[3];
        Array.Copy(enemy, _caveEnemyPos, 3);
        _cavePlayerPos = new PosRot[5];
        Array.Copy(player, _cavePlayerPos, 5);
        Managers.UIManager.ChangeTimeLineGrid<UI_BattleTimeLineGrid>();
        RewardList = rewardList;

        _unitsList.Clear();
        _enemyList.Clear();
        _playerList.Clear();
        EnemyId.Clear();
        _unitOrders.Clear();
        _playerList.Clear();
        for(int i= 0; i< playerDummy.Count; i++)
        {
            _playerList.Add(playerDummy[i].GetComponent<BattleUnit>());
        }
        _playerStatsList = playerStatList;
        EnemyId = enemyList;
        _mapName = mapName;
        _isCave = true;
        //전투 시작
        

        StartCoroutine(CaveDelay());
    }
    IEnumerator CaveDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CaveBattleStart();
        Managers.Sound.PlayBGM("BattleBGM");
    }
    /// <summary>
    /// 각 유닛 생성 및 배치, 카메라 배치
    /// </summary>
    void BattleStart()
    {
        Transform Parent = GameObject.Find(_mapName).GetComponent<Transform>();
        Managers.Camera.SetBattleCamera(Parent);
        switch (EnemyId.Count)
        {
            case 1:
                CreateEnemy(EnemyId.Count, Parent);
                break;
            case 2:
                for (int i = 0; i < EnemyId.Count; i++)
                {
                    CreateEnemy(EnemyId.Count, Parent, i);
                }
                break;
            case 3:
                for (int i = 0; i < EnemyId.Count; i++)
                {
                    CreateEnemy(EnemyId.Count, Parent, i);
                }
                break;
        }
        switch (_playerStatsList.Count)
        {
            case 1:
                CreatePlayer(_playerStatsList.Count, Parent);
                break;
            case 2:
                for (int i = 0; i < _playerStatsList.Count; i++)
                {
                    CreatePlayer(_playerStatsList.Count, Parent, i);
                }
                break;
            case 3:
                for (int i = 0; i < _playerStatsList.Count; i++)
                {
                    CreatePlayer(_playerStatsList.Count, Parent, i);
                }
                break;
        }
        _uI_BattleTimeLine.CaveUI(_isCave);
        _uI_Battle.SetEnemyInfos();
        //각 유닛의 순서 정하기
        SetOrder();
    }
    /// <summary>
    /// 던전 전투 시작
    /// </summary>
    void CaveBattleStart()
    {
        Transform Parent = GameObject.Find(_mapName).GetComponent<Transform>();
        switch (EnemyId.Count)
        {
            case 1:
                CaveCreateEnemy(EnemyId.Count, Parent);
                break;
            case 2:
                for (int i = 0; i < EnemyId.Count; i++)
                {
                    CaveCreateEnemy(EnemyId.Count, Parent, i);
                }
                break;
            case 3:
                for (int i = 0; i < EnemyId.Count; i++)
                {
                    CaveCreateEnemy(EnemyId.Count, Parent, i);
                }
                break;
        }
        for (int i = 0; i < _playerList.Count; i++)
        { 
            _unitsList.Add(_playerList[i]);
            _playerList[i].BattleDataInit(_playerStatsList[i].PlayerID, _playerStatsList[i]);
        }
        _uI_BattleTimeLine.CaveUI(_isCave);
        _uI_Battle.SetEnemyInfos();
        //각 유닛의 순서 정하기
        SetOrder();
    }

    /// <summary>
    /// 유닛 생성
    /// </summary>
    /// <param name="count"> 유닛 갯수</param>
    /// <param name="Parent">유닛이 생성될 부모</param>
    /// <param name="i"> 유닛 순서</param>
    void CreateEnemy(int count, Transform Parent, int i = 0)
    {
        GameObject dummy = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.BattleMapEnemy, EnemyId[i] + "_" + Enum.GetValues(typeof(DataManager.EnemyCharacter)).GetValue(EnemyId[i]));

        _unit = Instantiate(dummy);
        _unit.transform.parent = Parent;
        if (count == 1)
        {
            _unit.transform.SetLocalPositionAndRotation(_enemyPos[3].Pos, _enemyPos[3].Rot);
        }
        else if (count == 2)
        {
            _unit.transform.SetLocalPositionAndRotation(_enemyPos[i].Pos, _enemyPos[i].Rot);
        }
        else if (count == 3)
        {
            _unit.transform.SetLocalPositionAndRotation(_enemyPos[i + 2].Pos, _enemyPos[i + 2].Rot);
        }
        _battleUnit = _unit.GetComponent<BattleUnit>();
        _battleUnit.EnemyIndex = i;
        _uI_Battle.uI_EnemyInfos[i].enemy = _battleUnit.GetComponent<EnemyBattle>();
        _battleUnit.BattleDataInit(EnemyId[i]);
        _unitsList.Add(_battleUnit);
        _enemyList.Add(_battleUnit);
    }
    void CreatePlayer(int count, Transform Parent, int i = 0)
    {
        GameObject dummy = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.BattleMapPlayer, _playerStatsList[i].PlayerID + "_" + Enum.GetValues(typeof(DataManager.PlayerCharacter)).GetValue(_playerStatsList[i].PlayerID));
        _unit = Instantiate(dummy);
        _unit.transform.parent = Parent;

        if (count == 1)
        {

            _unit.transform.SetLocalPositionAndRotation(_playerPos[3].Pos, _playerPos[3].Rot);

        }
        else if (count == 2)
        {
            _unit.transform.SetLocalPositionAndRotation(_playerPos[i].Pos, _playerPos[i].Rot);
        }
        else if (count == 3)
        {
            _unit.transform.SetLocalPositionAndRotation(_playerPos[i + 2].Pos, _playerPos[i + 2].Rot);

        }
        _battleUnit = _unit.GetComponent<BattleUnit>();
        _battleUnit.BattleDataInit(_playerStatsList[i].PlayerID, _playerStatsList[i]);
        _unitsList.Add(_battleUnit);
        _playerList.Add(_battleUnit);
    }
    void CaveCreateEnemy(int count, Transform Parent, int i = 0)
    {
        GameObject dummy = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.BattleMapEnemy, EnemyId[i] + "_" + Enum.GetValues(typeof(DataManager.EnemyCharacter)).GetValue(EnemyId[i]));

        _unit = Instantiate(dummy);
        _unit.transform.parent = Parent;
        if (count == 1)
        {
            _unit.transform.SetLocalPositionAndRotation(_caveEnemyPos[1].Pos, _caveEnemyPos[1].Rot);
        }
        else if (count == 2)
        {
            _unit.transform.SetLocalPositionAndRotation(_caveEnemyPos[i*2-2].Pos, _caveEnemyPos[i*2-2].Rot);
        }
        else if (count == 3)
        {
            _unit.transform.SetLocalPositionAndRotation(_caveEnemyPos[i].Pos, _caveEnemyPos[i].Rot);
        }
        _battleUnit = _unit.GetComponent<BattleUnit>();
        _battleUnit.EnemyIndex = i;
        _uI_Battle.uI_EnemyInfos[i].enemy = _battleUnit.GetComponent<EnemyBattle>();
        _battleUnit.BattleDataInit(EnemyId[i]);
        _unitsList.Add(_battleUnit);
        _enemyList.Add(_battleUnit);
    }


    /// <summary>
    /// 순서 정하기
    /// </summary>
    void SetOrder()
    {
        for (int i = 1; i < 30; i++)
        {
            foreach (BattleUnit unit in UnitsList)
            {

                UnitOrder oj;
                float Order = ((i / (float)unit.data.Spd) * 100);
                oj.Order = Order;
                oj.Unit = unit;
                _unitOrders.Add(oj);
            }
        }
        _unitOrders.Sort(compare);
        _uI_BattleTimeLine.InitIcons(_unitOrders);
        _uI_BattleTimeLine.SetBattlePopUp(false);
        Invoke("ReadyBattle", 4f);
    }

    /// <summary>
    /// 전투 시작
    /// </summary>
    void ReadyBattle()
    {

        _inPlayUnit = _unitOrders[0].Unit;
        if (_inPlayUnit.gameObject.CompareTag("Player")&&!_isCave)
        {
            _inPlayer = _inPlayUnit.GetComponent<PlayerBattle>();
            Managers.Camera.nowPlay = NowPlay.PLAYER;
        }
        else
        {
            _inPlayer = null;
            Managers.Camera.nowPlay = NowPlay.ENEMY;
        }
        _inPlayUnit.IsProgress = true;
    }

    #endregion

    /// <summary>
    /// 순서를 다음으로 넘기기
    /// </summary>
    public void TurnOver()
    {
        for (int i = 0; i < UnitsList.Count; i++)
        {
            UnitsList[i].ActiveToken(ActiveTime.TurnStart);
        }
        if (EnemyList.Count == 0)
        {
            Ending(false);
            return;
        }
        if (_isFisrt)
        {
            _isFisrt = false;
        }
        _unitOrders.RemoveAt(0);
        _inPlayUnit = _unitOrders[0].Unit;
        if (_inPlayUnit.gameObject.CompareTag("Player"))
        {
            _inPlayer = _inPlayUnit.GetComponent<PlayerBattle>();
            Managers.Camera.nowPlay = NowPlay.PLAYER;
            Managers.Camera.CaveCamIndex = _playerList.IndexOf(_inPlayUnit);
        }
        else
        {
            Managers.Camera.CaveCamIndex = 1;
            _inPlayer = null;
            _uI_BattleTimeLine.SelectFalse();
            Managers.Camera.nowPlay = NowPlay.ENEMY;
        }
        _uI_BattleTimeLine.SetIcons(_unitOrders);

    }

    public void InplayerTrue()
    {
        _inPlayUnit.IsProgress = true;
    }
    /// <summary>
    /// 유닛이 죽었을 때 호출하는 함수
    /// </summary
    /// <param name="target">죽은 유닛</param>
    public void Die(BattleUnit dieUnit)
    {
        if (dieUnit.CompareTag("Enemy"))
        {
            int index = dieUnit.EnemyIndex;
            _uI_Battle.uI_EnemyInfos[index].gameObject.SetActive(false);
            EnemyList.Remove(dieUnit);     
        }
        else
        {;
            PlayerList.Remove(dieUnit);
        }
        UnitsList.Remove(dieUnit);
        _unitOrders.RemoveAll(x => x.Unit == dieUnit);
        dieUnit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Dead);
    }

    /// <summary>
    /// 선택된 적 유닛의 선택아이콘 활성화
    /// </summary>
    ///  <param name="target">선택된 적 유닛</param>
    public void SelectedEnemyIcon(BattleUnit target)
    {
        _uI_BattleTimeLine.SelectTrue(_unitOrders, target, _isFisrt);
    }


    /// <summary>
    /// 전투 끝난고 아이템 고르는 타이밍
    /// </summary>
    public void Ending(bool isRun)
    {
        Managers.Sound.StopBGM();
        _uI_BattleTimeLine.Ending(_unitOrders);
       //던전이 아닐 때
        if (!_isCave)
        {
            // 도망치지 않았을 때
            if (!isRun)
            {
                //이겼을 때
                if (PlayerList.Count != 0)
                {
                    _isWin = true;
                    PlayerVictory();
                    _uI_BattleTimeLine.FlagTrue();
                    Managers.Camera.ChangeVolume(CameraVolumeType.Battle);
                    Managers.Camera.BlurEffect(true, 15f);
                    ShowReward();
                }
            }
            else // 도망칠 때
            {
                Invoke("ToWorld", 3f);
            }
        }
        else
        {
            if (PlayerList.Count != 0)
            {
                _isWin = true;
                ShowReward();
                PlayerVictory();
            }
        }
        
    }

    public void PlayerVictory()
    {
        foreach (var unit in PlayerList)
        {
            unit.CharacterEventListener.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Victory);
        }
    }

    public void PutDataForSmalleReward(List<BattleUnit> playerlist, List<Item> rewardlist)
    {
        PlayerList = playerlist;
        RewardList = rewardlist;
        _isCave = true;
        _smalleReward = true;
    }
    public void ShowReward()
    {

        if (RewardList.Count > RewardIndex)
        {
            Managers.UIManager.ShowMainItemCardUI(null, UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, false);
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBattle player = (PlayerBattle)PlayerList[i];
                Managers.UIManager.GetPlayerHUD(player.PlayerStat).UpdateVoteBtn(RewardList[RewardIndex]);
            }
            Managers.UIManager.ShowMainItemCardUI(RewardList[RewardIndex++], UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, true,true);        
        }
        else
        {
            Managers.UIManager.ShowMainItemCardUI(null,  UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, false);
            RewardIndex =0;
            RewardList.Clear();
            if (!_isCave)
            {
                Invoke("ToWorld", 2f);
            }
            else
            {
                Invoke("ToCave", 2f);
            }
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBattle player = (PlayerBattle)PlayerList[i];
                Managers.UIManager.GetPlayerHUD(player.PlayerStat).VoteBtnOff();
            }
        }
       
    }
    /// <summary>
    /// World맵으로 전환
    /// </summary>
    void ToWorld()
    {
        for (int i = 0; i < Managers.Instance.GetPlayerCount; i++)
        {
            PlayerStats player = Managers.Instance.GetPlayer(i).PlayerStats;
            Managers.UIManager.GetPlayerHUD(player).InitTokenUI();
        }
        Managers.Sound.PlayBGM("VillageBGM");
        _isFisrt = true;
        _inPlayUnit = null;
        Managers.Camera.SetWorldCamera();
        _uI_Battle.InitEnemyInfos();
        _uI_BattleTimeLine.FlagFalse();
        Managers.UIManager.ChangeTimeLineGrid<UI_WorldMapTimeLineGrid>();
        StartCoroutine(DestroyAllUnit());
        _battleResult?.Invoke(!_isWin);
        _isWin = false;

    }

    void ToCave()
    {
        for (int i = 0; i < Managers.Instance.GetPlayerCount; i++)
        {
            PlayerStats player = Managers.Instance.GetPlayer(i).PlayerStats;
            Managers.UIManager.GetPlayerHUD(player).InitTokenUI();
        }
        if (!_smalleReward)
        {
            Managers.Camera.CaveCamIndex = 3;
            _isFisrt = true;
            _inPlayUnit = null;
            _uI_Battle.InitEnemyInfos();
            Managers.UIManager.ChangeTimeLineGrid<UI_CaveTImeLineGird>();
            Managers.Cave.ClearNode();
            _isWin = false;
        }
        else
        {
            _smalleReward= false;
            Managers.Cave.NowCave.DestroyAndClear();
        }
        

    }
  
    IEnumerator DestroyAllUnit()
    {
        yield return new WaitForSeconds(3f);
        foreach (var unit in _unitsList)
        {
            Destroy(unit.gameObject);
        }
    }

    /*public void CaveOffPlayer()
    {
        foreach (var unit in _unitsList)
        {
            Destroy(unit.gameObject);
        }
    }*/


    //ToDO:엔딩 끝나고 초기화해야하는 변수들
    //_isFisrt = true, PlayUnit 아이콘 활성화, Wait아이콘 원위치,_inPlayUnit =null
}
