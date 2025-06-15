using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraManager;


public class BattleManager : MonoBehaviour
{
    private string _mapName; //��Ʋ�� �̸�
    private bool _isFisrt = true; //ó�� ���� ����
    List<int> EnemyId = new List<int>();
    List<PlayerStats> _playerStatsList = new List<PlayerStats>();
    List<BattleUnit> _enemyList = new List<BattleUnit>();
    List<BattleUnit> _playerList = new List<BattleUnit>();
    List<BattleUnit> _unitsList = new List<BattleUnit>();
    
    public BattleUnit CommonUnit;
    [SerializeField]
    private PosRot[] _enemyPos; //���� ��ġ�� ��ġ
    [SerializeField]
    private PosRot[] _playerPos; //�÷��̾ ��ġ�� ��ġ
    //������ ��ġ
    private PosRot[] _caveEnemyPos; //���� ��ġ�� ��ġ
    private PosRot[] _cavePlayerPos; //�÷��̾ ��ġ�� ��ġ

    GameObject _unit; //�������� �����ϰ� ������ ���ӿ�����Ʈ ����
    BattleUnit _battleUnit; //�������� �����ϰ� ������ BattleUnit ����
    [SerializeField]
    private UI_BattleTimeLineGrid _uI_BattleTimeLine;
    [SerializeField]
    private UI_Battle _uI_Battle;
    private BattleUnit _inPlayUnit; //���� �÷������� �÷��̾�
    private PlayerBattle _inPlayer; //���� �÷��̾����� ����

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

    List<UnitOrder> _unitOrders = new List<UnitOrder>(); //���ְ� ���� ����Ʈ


    #region ���� ���� �˰���
    /// <summary>
    /// ���� �� �� ����Ʈ �ʱ�ȭ 
    /// </summary>
    public void BattleInit(List<PlayerStats> playerList, List<int> enemyList, string mapName, Action<bool> battleResult, List<Item> rewardList, int hexQuestID)
    {
        // ������ ����� ��� ���������� ���� �����ϰ�, ���� Hex�� ��� �ʱ�ȭ �����ִ� �̺�Ʈ�� �����
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
        print("���󰳼� :" +RewardList.Count);
        
        _unitsList.Clear();
        _enemyList.Clear();
        _playerList.Clear();
        EnemyId.Clear();
        _unitOrders.Clear();
        _playerStatsList = playerList;
        EnemyId = enemyList;
        _mapName = mapName;
        _isCave = false;
        //���� ����
        BattleStart();
        Managers.Sound.PlayBGM("BattleBGM");
    }

    //���� ����
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
        //���� ����
        

        StartCoroutine(CaveDelay());
    }
    IEnumerator CaveDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CaveBattleStart();
        Managers.Sound.PlayBGM("BattleBGM");
    }
    /// <summary>
    /// �� ���� ���� �� ��ġ, ī�޶� ��ġ
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
        //�� ������ ���� ���ϱ�
        SetOrder();
    }
    /// <summary>
    /// ���� ���� ����
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
        //�� ������ ���� ���ϱ�
        SetOrder();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="count"> ���� ����</param>
    /// <param name="Parent">������ ������ �θ�</param>
    /// <param name="i"> ���� ����</param>
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
    /// ���� ���ϱ�
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
    /// ���� ����
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
    /// ������ �������� �ѱ��
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
    /// ������ �׾��� �� ȣ���ϴ� �Լ�
    /// </summary
    /// <param name="target">���� ����</param>
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
    /// ���õ� �� ������ ���þ����� Ȱ��ȭ
    /// </summary>
    ///  <param name="target">���õ� �� ����</param>
    public void SelectedEnemyIcon(BattleUnit target)
    {
        _uI_BattleTimeLine.SelectTrue(_unitOrders, target, _isFisrt);
    }


    /// <summary>
    /// ���� ������ ������ ���� Ÿ�̹�
    /// </summary>
    public void Ending(bool isRun)
    {
        Managers.Sound.StopBGM();
        _uI_BattleTimeLine.Ending(_unitOrders);
       //������ �ƴ� ��
        if (!_isCave)
        {
            // ����ġ�� �ʾ��� ��
            if (!isRun)
            {
                //�̰��� ��
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
            else // ����ĥ ��
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
    /// World������ ��ȯ
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


    //ToDO:���� ������ �ʱ�ȭ�ؾ��ϴ� ������
    //_isFisrt = true, PlayUnit ������ Ȱ��ȭ, Wait������ ����ġ,_inPlayUnit =null
}
