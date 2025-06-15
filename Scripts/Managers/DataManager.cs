using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


public class DataManager
{

    private Dictionary<int, PlayerStats> _playerStats = new Dictionary<int, PlayerStats>();
    
    private Dictionary<int, EnemyStat> _EnemysStats = new Dictionary<int, EnemyStat>();

    private Dictionary<int, Sanctuary> _sanctuaries = new Dictionary<int, Sanctuary>();

    private Dictionary<int, Cave> _caves = new Dictionary<int, Cave>();
    
    private Dictionary<int, Weapon> _weapons = new Dictionary<int, Weapon>();
    
    private Dictionary<int, Consumable> _usedItems = new Dictionary<int, Consumable>();
    
    private Dictionary<int, Armor> _armors = new Dictionary<int, Armor>();

    private Dictionary<Item.ItemType, Dictionary<int, Item>> _items = new Dictionary<Item.ItemType, Dictionary<int, Item>>();

    private Dictionary<int, Skill> _skills = new Dictionary<int, Skill>();

    private Dictionary<Item.ItemType, Func<int, string>> _itemID = new Dictionary<Item.ItemType, Func<int, string>>()
    {
        { Item.ItemType.Weapon , weaponID => ((WeaponID)weaponID).ToString() },
        { Item.ItemType.Armor , armorID => ((ArmorID)armorID).ToString() },
        { Item.ItemType.Consumable ,  consumableID => ((ConsumableID)consumableID).ToString() },
        { Item.ItemType.GoldCoin ,  questItemID => ((GoldCoinID)questItemID).ToString() },
        { Item.ItemType.QuestItem ,  questItemID => ((QuestItemID)questItemID).ToString() },
    };


    public enum PlayerCharacter
    {
        Knight = 0,
        Archer = 1,
        Mage = 2,
    }
    
    public enum EnemyCharacter
    {
        None = -1,
        Bat = 0,
        Spider = 1,
        Snake = 2,
        Rat = 3,
        Frog = 4,
        Wasp = 5,
        SkeletonSalve = 6,
        SkeletonWarrior = 7,
        Orc = 8,
        Rogue = 9,
        Witch = 10,
        SkeletonKnight = 11,
        AX1 = 100,
    }

    // 무기 ID
    public enum WeaponID
    {
        WoodenSword = 1,
        WoodenBow = 2,
        BrokenWand = 3,
        Dagger = 4,
        LongSword = 7,
    }

    // 방어구 ID
    public enum ArmorID
    {
        LeatherVest = 0,
        OldLeatherHelm = 1,
        OldBoots = 2,
        ScholarCap = 3,
        ScholarWraps = 4,
    }

    // 소모품 ID
    public enum ConsumableID
    {
       
    }

    // 퀘스트 아이템 ID
    public enum QuestItemID
    {
        
    }
    
    // 재화 아이템 ID
    public enum GoldCoinID
    {
        GoldCoin = 0,
    }
    
    
    public enum StoreID
    {
        Caramba = 0,
        Arkeon = 1,
        Novacity = 2,
        Neocity = 3,
        Coralis = 4,
        Blueward = 5,
        Skyheight = 6,
    }

    public enum CaveID
    {
        Cave = 0,
    }

    public enum SanctuaryID
    { 
        HealthSanctuary = 0, 
        ShadowSanctuary = 1,
       FocusSanctuary = 2,
       ExperienceSanctuary = 3,
      
    }

    public enum CampID
    {
        EnemyAnimalCamp = 0,
        EnemyHumanCamp = 1,
        EnemyTribalCamp = 2,
        EnemyUndeadCamp = 3,
        PlayerSafeCamp = 4,
    }

    public enum HumanoidAnim
    {
        Humanoid__Fists = 0,
        Humanoid__OneHand_Sword,
        Humanoid__TwoHand_Sword,
        Humanoid__TwoHand_Bow,
        Humanoid__TwoHand_Wand,
    }
    
    #region 데이터 베이스 접근 함수

    

    #endregion

    #region 클라이언트 함수

    public DataManager()
    {
        
        _items.Add(Item.ItemType.Weapon, new Dictionary<int, Item>()
        {
            { 0, new Weapon(0, "주먹", 0,  4,3,0, 3,0, 0, 1, -1, -1, -1, -1, 0, 0, "인류의 최초 무기", "2023-04-02", "2023-04-02") },
            { 1, new Weapon(1, "나무검",4, 8,0,0,5,0, 0, 1, 2, 3, -1, -1, 10, 1, "초보자용 나무 목검","2023-04-02", "2023-04-02") },
            { 2, new Weapon(2, "나무활", 5, 1,1,0, 5, 0, 0, 26, -1, -1, -1, -1,10,  1, "초보자용 나무 활", "2023-04-02", "2023-04-02") },
            { 3, new Weapon(3, "나무지팡이", 6, 7,2, 1, 4, 0,0, 25, 26, -1, -1, -1, 10, 1, "어르신 목발", "2023-04-02", "2023-04-02") },
            
        });
        
        
        _items.Add(Item.ItemType.GoldCoin, new Dictionary<int, Item>()
        {
            {0, new GoldCoin(0, "골드 코인", 0, 0 , "각 코인은 가치는 금의 무게와 같은 화폐입니다.")}
        });



        _items.Add(Item.ItemType.Armor, new Dictionary<int, Item>()
        {

            { 0, new Armor(0, "가죽 조끼", 2, 0, 3, 0, -1, 0, -1, 0, -1, 15, 5, "가죽 방어구", "2023-04-02", "2023-04-02") },
            { 1, new Armor(1, "낡은가죽 투구", 1000, 0, 1, 1, -1, 0, -1, 0, -1, 12, 3, "가죽 헬멧", "2023-04-02", "2023-04-02") },
            { 2, new Armor(2, "낡은 부츠", 5, 0, 1, 1, -1, 0, -1, 0, -1, 12, 3, "오래된 부츠", "2023-04-02", "2023-04-02") },
            { 3, new Armor(3, "학자 모자", 1001, 0, 0, 2, 2, 1, -1, 0, -1, 15, 5, "학자의 모자", "2023-04-02", "2023-04-02") },
            { 4, new Armor(4, "학자 신발", 5, 0, 0, 2, 2, 1, 0, -1, -1, 15, 5, "학자의 신발", "2023-04-02", "2023-04-02") },
        });
        
        /*
        
        _items.Add(Item.ItemType.Accessory, new Dictionary<int, Item>()
        {
            { 0, new Weapon(0, "주먹", 0, 0, 3,0, 0, 0, -1, -1, -1, -1, 0, 0, "인류의 최초 무기", "2023-04-02", "2023-04-02") },
            { 1, new Weapon(1, "나무검",0,0,5,0, 0, 7, -1, -1, -1, -1, 10, 1, "초보자용 나무 목검","2023-04-02", "2023-04-02") },
            { 2, new Weapon(2, "나무활", 0, 0, 5, 0, 0, 10, -1, -1, -1, -1,10,  1, "초보자용 나무 활", "2023-04-02", "2023-04-02") },
            { 3, new Weapon(3, "나무지팡이", 0, 1, 4, 0,0, 3, -1, -1, -1, -1, 10, 1, "어르신 목발", "2023-04-02", "2023-04-02") },
            
        });
        
        _items.Add(Item.ItemType.QuestItem, new Dictionary<int, Item>()
        {
            { 0, new Weapon(0, "주먹", 0, 0, 3,0, 0, 0, -1, -1, -1, -1, 0, 0, "인류의 최초 무기", "2023-04-02", "2023-04-02") },
            { 1, new Weapon(1, "나무검",0,0,5,0, 0, 7, -1, -1, -1, -1, 10, 1, "초보자용 나무 목검","2023-04-02", "2023-04-02") },
            { 2, new Weapon(2, "나무활", 0, 0, 5, 0, 0, 10, -1, -1, -1, -1,10,  1, "초보자용 나무 활", "2023-04-02", "2023-04-02") },
            { 3, new Weapon(3, "나무지팡이", 0, 1, 4, 0,0, 3, -1, -1, -1, -1, 10, 1, "어르신 목발", "2023-04-02", "2023-04-02") },
            
        });
        */
        

        
        _playerStats.Add
        (
            0,
            new PlayerStats(0, 0, "기사",81, 57, 45, 85,  61,
                        9, 4, 38, 1, 0,  5, 1)
            );
        
        _playerStats.Add
        (
            1,
            new PlayerStats(1, 0, "궁수", 57, 83, 51, 71,  81,
                18, 3, 37, 1, 1, 5, 2)
        );
        
        _playerStats.Add
        (
            2,
            new PlayerStats(2, 0, "마법사",47, 71, 83, 65,  75,
                15, 5, 36, 0, 1, 5, 3)
        );
        
        _EnemysStats.Add
        (
            0,
            new EnemyStat(0,"박쥐", 0, 1, 0, 9, 5, 90, 0, 0, 70, 12, 5,101,104)
        );
        _EnemysStats.Add
       (
           4,
           new EnemyStat(4,"대형 개구리", 0, 1, 0, 15, 7, 65, 1, 0, 75, 5, 5,101)
       );
        _EnemysStats.Add
      (
          5,
          new EnemyStat(5,"말벌", 0, 1, 0, 7, 10, 83, 0, 0, 70, 18, 5,101,105)
      );
        _EnemysStats.Add
     (
         6,
         new EnemyStat(6,"스켈레톤", 1, 2, 0, 9, 7, 70, 4, 0, 75, 8, 10,102,103)
     );
        _EnemysStats.Add
    (
        7,
         new EnemyStat(7,"스켈레톤 전사", 2, 2, 0, 10, 10, 63, 1, 1, 80, 10, 7,1,2)
    );
        _EnemysStats.Add
    (
          9,
         new EnemyStat(9,"도적", 2, 0, 2, 18, 15, 83, 3, 0, 85, 25, 10,1,3)
    );
        _EnemysStats.Add
        (
            11,
            new EnemyStat(11,"스켈레톤 기사", 2, 2, 0, 11, 15, 83, 1, 0, 85, 25, 10,1,2)
        );




        _caves.Add(0, new Cave(0, "지하 광산", "깊은 지하로 탐험을 진행합니다", "너무 깊어 보인다", 0));


        _sanctuaries.Add(3, new ExperienceSanctuary(3, "경험의 성소", "획득하는 경험치의 추가로 25% 획득한다", "경험보다 중요한거는 없다"));
        _sanctuaries.Add(0, new HealthSanctuary(0, "체력의 성소", "최대 체력을 5 증가시킨다", "지능보다는 힘"));
        _sanctuaries.Add(1, new ShadowSanctuary(1, "그림자의 성소", "회피력을 5 증가시킨다", "손은 눈보다 빠르다"));
        _sanctuaries.Add(2, new FocusSanctuary(2, "집중의 성소", "최대 집중력을 2 증가 시킨다", "집중을 하면 어떤 극복도 가능하다"));


        _skills.Add(1, new Skill(1, "찌르기", 0, 0, 0, 2, 5, "기절 효과 부여",false,"",31,1));
        _skills.Add(2, new Skill(2, "베기", -10, 0, 1, 3, 0, "광역 대상",false));
        _skills.Add(3, new Skill(3, "혈관 절개", -10, 0, 0, 4, 0, "출혈 효과 부여", false, "", 33, 2));
        _skills.Add(25, new Skill(25, "블래스트", -10, 1, 1, 4, 0, "약화 효과 부여", true, "FireballExplosion", 38, 1,8));
        _skills.Add(26, new Skill(26, "매직 미사일", 0, 1, 0, 2, 0, "단일 대상", true, "MagicBeam"));
        _skills.Add(0, new Skill(0, "도망치기", 0, 4, 0, 2, 0, "전투에서 도망",false));

        _skills.Add(101, new Skill(101, "일반공격", 0, 0, 0, 3, 0, "", false));
        _skills.Add(102, new Skill(102, "할퀴기", -10, 0, 0, 3, 0, "", false, "", 34, 2));
        _skills.Add(103, new Skill(103, "깨물기", -10, 0, 0, 3, 0, "", false, "", 33, 2));
        _skills.Add(104, new Skill(104, "출혈 공격", -10, 0, 0, 4, 0, "", false, "", 33, 2));
        _skills.Add(105, new Skill(105, "중독 공격", -10, 0, 0, 4, 0, "", false, "", 34, 2));
    }

    public PlayerStats GetPlayerStatsInfo(int playerID)
    {
        PlayerStats copyPlayerStats = null;
        if (_playerStats.TryGetValue(playerID, out PlayerStats originalPlayerStats))
        {
            if (_playerStats[playerID].PlayerPortrait == null)
            {
                _playerStats[playerID].PlayerPortrait =
                    Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.PortraitPlayer,
                        playerID.ToString(),
                        false); //Managers.Resource.LoadResource<Sprite>("Portraits/Player/" + playerID.ToString());
            }
            
            if (_playerStats[playerID].BaseWeapon == null)
            {
                _playerStats[playerID].BaseWeapon = (Weapon)GetItemInfo(Item.ItemType.Weapon, originalPlayerStats.CurrentWeaponId); //_weapons[copyPlayerStats.CurrentWeaponId];
            }
            

            copyPlayerStats = (PlayerStats)originalPlayerStats.Clone();
        }
        return copyPlayerStats;
    }

    public EnemyStat GetEnemyStatsInfo(int enemyID)
    {
        EnemyStat copyEnemyStatTests = null;
        if (_EnemysStats.TryGetValue(enemyID, out copyEnemyStatTests))
        {
            if (_EnemysStats[enemyID].EnemyIcon == null)
            {
                _EnemysStats[enemyID].EnemyIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.PortraitEnemy,
                    enemyID.ToString(),
                    false);
            }
        }
        return copyEnemyStatTests;
    }
    /*
    public Store GetStoreInfo(int storeID)
    {
        return _stores.TryGetValue(storeID, out Store store) ? store : null;
    }
    */

    public Item GetItemInfo(Item.ItemType itemType, int itemID)
    {
        if(itemType== Item.ItemType.GoldCoin)
        {
            Item coin = _items[Item.ItemType.GoldCoin][0];
            coin.ItemStock = itemID;
            return coin;
        }
        return _items.TryGetValue(itemType, out Dictionary<int, Item> itemDictionary) 
            ? itemDictionary.TryGetValue(itemID, out Item item) ? (Item)item.Clone() : null
            : null;
    }

    
    public Sanctuary GetSanctuaryInfo(int sanctuaryID)
    {
        return _sanctuaries.TryGetValue(sanctuaryID, out Sanctuary sanctuary) ? sanctuary : null;
    }

    public Cave GetCaveInfo(int caveID)
    {
        return _caves.TryGetValue(caveID, out Cave cave) ? cave : null;
    }
    
    public Skill GetUsedSkillInfo(int skillID)
    {
        Skill copySkill = null;
        if (_skills.TryGetValue(skillID, out copySkill))
        {
            if (_skills[skillID].Icon == null&& skillID < 100)
            {
                _skills[skillID].Icon = Managers.Resource.LoadResource<Sprite>("Skills/" + skillID.ToString());
            }
        }
        return copySkill;
        //return _skills.TryGetValue(itemID, out Skill skill) ? skill : null; 
    }

    // 아이템 ID기반으로 아이템 이름을 가져옴 
    public string GetItemName(Item.ItemType itemType, int itemID)
    {
       return _itemID[itemType](itemID);
    }

    // DebugOption 전용 (테스트 용도)
    public Dictionary<int, Item> ItemInfos(Item.ItemType itemType)
    {
        return _items[itemType];
    }

    public enum DataType
    {
        PlayerStat = 0,
        EnemyStat,
        Store,
        Cave,
        Sanctuary,
        Weapon,
        UsedItem,
        Armor,
        Skill
    }
    
    public int GetDataLength(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.PlayerStat: return _playerStats.Count;     
            case DataType.EnemyStat: return _EnemysStats.Count;   
            //case DataType.Store: return _stores.Count;   
            case DataType.Cave: return _caves.Count;   
            case DataType.Sanctuary: return _sanctuaries.Count;   
            case DataType.Weapon: return _weapons.Count;   
            case DataType.UsedItem: return _usedItems.Count;   
            case DataType.Armor: return _armors.Count; 
            case DataType.Skill: return _skills.Count;   
            
            default: return 0;
        }
    }
    
    #endregion

}
