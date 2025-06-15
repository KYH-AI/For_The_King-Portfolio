using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager
{
    
    // 리소스 폴더 절대경로 확인용
    public enum ResourcePath
    {
        None = -1,
        WorldMapEnemy = 0,
        WorldMapPlayer = 1,
        Store = 2,
        Sanctuary = 3,
        Cave = 4,
        BattleMapPlayer = 5,
        BattleMapEnemy = 6,
        SlotIconWorld = 7,
        StatIconBattle = 8,
        PortraitPlayer = 9,
        PortraitEnemy = 10,
        BattleIcon = 11,
        BattleSkills = 12,  // 스킬 이미지 경로
        Camp = 13,
        ItemCardIcon = 14,
        Weapon = 15,
        Armor = 16,
        Consumable = 17,
        QuestItem = 18,
        Token = 19,
        Effect = 20,
        Coin = 21,
        PortraitNpc = 22,
        CaveMapPlayer = 23,
        CaveIcon = 24,
        SanctuaryIcon = 25,
        HexOverlay = 26,
    }
    

    private readonly Dictionary<string, Object> _loadedResources = new Dictionary<string, Object>();
    
    private readonly Dictionary<Type, string> _typeDic = new Dictionary<Type, string>()
    {
        {typeof(GameObject), "Prefabs/"},
        {typeof(Sprite), "Sprite/"},
        {typeof(RuntimeAnimatorController), "RunTimeAnimator/"},
        {typeof(AudioClip), "Sound/"},
        {typeof(Material), "Material/"}
    };

    private readonly Dictionary<string, Object> _objectPoolDic = new Dictionary<string, Object>();

    // 리소스 절대경로 딕셔너리
    private readonly Dictionary<ResourcePath, string> _pathDic = new Dictionary<ResourcePath, string>()
    {
        { ResourcePath.WorldMapEnemy, "Character/WorldMapEnemyCharacter/" },
        { ResourcePath.WorldMapPlayer, "Character/WorldMapPlayerCharacter/" },
        { ResourcePath.Store, "Hex/HexProb/Store/" },
        { ResourcePath.Sanctuary, "Hex/HexProb/Sanctuary/" },
        { ResourcePath.Cave, "Hex/HexProb/Cave/" },
        { ResourcePath.BattleMapPlayer, "Character/BattleMapPlayerCharacter/" },
        { ResourcePath.CaveMapPlayer, "Character/CaveMapPlayerCharacter/" },
        { ResourcePath.BattleMapEnemy, "Character/BattleMapEnemyCharacter/" },
        { ResourcePath.SlotIconWorld, "StatIcons/World/" },
        { ResourcePath.StatIconBattle, "StatIcons/Battle/" },
        { ResourcePath.PortraitPlayer, "Portraits/Player/" },
        { ResourcePath.PortraitEnemy, "Portraits/Enemy/" },
        { ResourcePath.BattleIcon, "BattleIcons/" },
        { ResourcePath.BattleSkills, "Skills/" },
        { ResourcePath.Camp , "Hex/HexProb/Camp/"},
        { ResourcePath.ItemCardIcon , "ItemCardIcons/"},
        { ResourcePath.Weapon, "Item/Weapon/"},
        { ResourcePath.Armor, "Item/Armor/"},
        { ResourcePath.Consumable, "Item/Consumable/"},
        { ResourcePath.QuestItem, "Item/QuestItem/"},
        { ResourcePath.Token , "Token/"},
        { ResourcePath.Effect , "Item/Effect/"},
        { ResourcePath.Coin, "Item/Coin/"},
        { ResourcePath.PortraitNpc, "Portraits/Npc/"},
        { ResourcePath.CaveIcon, "CaveIcons/"},
        { ResourcePath.SanctuaryIcon, "SanctuaryIcons/"},
        { ResourcePath.HexOverlay, "Hex/HexOverlay/"}
    };



    public Dictionary<ICONTYPE, List<Sprite>> IconDic;

    public T LoadResource<T>(string path) where T : Object
    {
        string resourcepath = "";
        if (_typeDic.ContainsKey(typeof(T)))
        {
            resourcepath = _typeDic[typeof(T)] + path;
        }
        else
        {
            Debug.LogError("Failed to load resource");
            return null;
        }

        if (_loadedResources.ContainsKey(resourcepath))
        {
            return _loadedResources[resourcepath] as T;
        }

        T resource = Resources.Load<T>(resourcepath);

        if (resource == null)
        {
            Debug.LogError("Failed to load resource at path: " + resourcepath);
            return null;
        }

        _loadedResources.Add(resourcepath, resource);

        return resource;
    }
    
    public T LoadResource<T>(ResourcePath resourcePath, string resourceName, bool needSave = true) where T : Object
    {
        string path;
        if (_typeDic.ContainsKey(typeof(T)))
        {
            
            // 최종경로(예시) = "Prefabs/Character/WorldMapEnemyCharacter/2_Snake 
            path = resourcePath is ResourcePath.None ? _typeDic[typeof(T)] + resourceName : _typeDic[typeof(T)] + _pathDic[resourcePath] + resourceName;
        }
        else
        {
            Debug.LogError("Failed to load resource");
            return null;
        }

        if (_objectPoolDic.ContainsKey(resourceName))
        {
            return _objectPoolDic[resourceName] as T;
        }

        T resource = Resources.Load<T>(path);

        if (resource == null)
        {
            Debug.LogError("Failed to load resource at path: " + path);
            return null;
        }

        // 해당 오브젝트가 메모리 풀링이 필요한 경우 저장
        if (needSave)
        {
            _objectPoolDic.Add(resourceName, resource);
        }

        return resource;
    }
    
    // 아이템 프리팹 경로 
    public ResourcePath GetItemPrefabPath(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.Weapon : return ResourcePath.Weapon;
            case Item.ItemType.Armor : return ResourcePath.Armor;
            case Item.ItemType.Consumable : return ResourcePath.Consumable;
            case Item.ItemType.QuestItem : return ResourcePath.QuestItem;
            case Item.ItemType.GoldCoin : return ResourcePath.Coin;
            default: return ResourcePath.None;
        }
    }

    //IconSprit 초기화
    public ResourceManager()
    {
        List<Sprite>  Clear = new List<Sprite>()
    {
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Strength_Clear"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Intelligence_Clear"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Awareness_Clear"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Speed_Clear"),
    };

        List<Sprite>  Yel = new List<Sprite>()
    {
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Strength_Yel"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Intelligence_Yel"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Awareness_Yel"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Speed_Yel"),
    };

        List<Sprite> Fail = new List<Sprite>()
    {
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.SlotIconWorld] + "uiSlotBlank"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.SlotIconWorld] + "uiHitMiss"),
    };

        List<Sprite> InActive = new List<Sprite>()
    {
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Strength_Inact"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Intelligence_Inact"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Awareness_Inact"),
        Resources.Load<Sprite>(_typeDic[typeof(Sprite)] + _pathDic[ResourcePath.StatIconBattle] + "Speed_Inact"),

    };
        IconDic = new Dictionary<ICONTYPE, List<Sprite>>()
        {
            {ICONTYPE.CLEAR, Clear},
            {ICONTYPE.YEL, Yel},
            {ICONTYPE.INACT, InActive},
            {ICONTYPE.FAIL, Fail},

        };
    }
}
