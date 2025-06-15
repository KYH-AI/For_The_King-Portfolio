using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    /*
     *  상점 및 내가 가진 아이템 관리.
     *  내가 가진 아이템은 인벤토리에서 관리를 해야하나?
     *
     *  구현해야할 것
     *  1.  DB연결 부분 추가 필요.
     */
        
    
    #region 변수 선언부
    [Header("전체 아이템  텍스트 파일")][SerializeField] private TextAsset _weaponItemDatabase;
    [Header("전체 아이템  텍스트 파일")][SerializeField] private TextAsset _armorItemDatabase;
    [Header("전체 아이템  텍스트 파일")][SerializeField] private TextAsset _useItemDatabase;
    [Header("전체 아이템 리스트")][SerializeField] private List<Item> _allItemList;

    public int WholeItemCount
    {
        get { return _allItemList.Count; }
    }
    #endregion

    
    #region 싱글톤 관련부
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                return null;
            }
        }
    }
    #endregion
    
    public Item GetItem(int itemId)
    {
        return _allItemList[itemId];
    }
    
    // /// <summary>
    // /// 무기 아이템 전체 목록 불러오기
    // /// </summary>
    // void LoadWeaponItemFromDatabase()
    // {
    //     // 전체 아이템 텍스트를 리스트에 저장
    //     string[] line = _weaponItemDatabase.text.Substring(0, _weaponItemDatabase.text.Length - 1).Split('\n');
    //     for (int i = 0; i < line.Length; i++)
    //     {
    //         string[] row = line[i].Split('\t');
    //         _allItemList.Add(new Weapon( int.Parse(row[0]), row[1], int.Parse(row[2]), int.Parse(row[3]), 
    //             int.Parse(row[4]), row[5], row[6], row[7], int.Parse(row[8]), 
    //             int.Parse(row[9]), new Vector3(int.Parse(row[10]), int.Parse(row[10])),int.Parse(row[11]),row[12], int.Parse(row[13]),int.Parse(row[14])));
    //     }
    // }
    //
    // /// <summary>
    // /// 방어구 아이템 전체 목록 불러오기
    // /// </summary>
    // void LoadArmorItemFromDatabase()
    // {
    //     // 전체 아이템 텍스트를 리스트에 저장
    //     string[] line = _armorItemDatabase.text.Substring(0, _armorItemDatabase.text.Length - 1).Split('\n');
    //     for (int i = 0; i < line.Length; i++)
    //     {
    //         string[] row = line[i].Split('\t');
    //         _allItemList.Add(new Weapon( int.Parse(row[0]), row[1], int.Parse(row[2]), int.Parse(row[3]), 
    //             int.Parse(row[4]), row[5], row[6], row[7], int.Parse(row[8]), 
    //             int.Parse(row[9]), new Vector3(int.Parse(row[10]), int.Parse(row[10])),int.Parse(row[11]),row[12], int.Parse(row[13]),int.Parse(row[14])));
    //     }
    // }
    //
    // /// <summary>
    // /// 소모성 아이템 전체 목록 불러오기
    // /// </summary>
    // void LoadUseItemFromDatabase()
    // {
    //     // 전체 아이템 텍스트를 리스트에 저장
    //     string[] line = _useItemDatabase.text.Substring(0, _useItemDatabase.text.Length - 1).Split('\n');
    //     for (int i = 0; i < line.Length; i++)
    //     {
    //         string[] row = line[i].Split('\t');
    //         _allItemList.Add(new UsedItem( int.Parse(row[0]), row[1], int.Parse(row[2]), int.Parse(row[3]), 
    //             int.Parse(row[4]), row[5], row[6], row[7], int.Parse(row[8]), int.Parse(row[9])));
    //     }
    // }
    
    

}
