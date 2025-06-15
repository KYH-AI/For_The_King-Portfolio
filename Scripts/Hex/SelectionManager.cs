using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour
{
    private Camera _mainCamera;

    public LayerMask SelectionMask;

    public UnityEvent<GameObject> OnUnitSelected;
   // public UnityEvent<GameObject> HexTerrainSelected;
    public UnityEvent<Hex, bool> HexTerrainSelected;
    private void Awake()
    {
        if(_mainCamera == null)
            _mainCamera = Camera.main;
    }

    public void HandleClick(Vector3 mousePosition, bool isClick)
    {
     //   print($"마우스 좌표 {mousePosition}");
        
        /*
        if (FindTarget(mousePosition, out GameObject result))
        {
            //print($"선택된 오브젝트 {result.gameObject.name}");
            print($"선택된 오브젝트 : {result.gameObject.name} 의 좌표 : {result.gameObject.transform.position}");
            
            
            if (UnitSelected(result))
            {
                OnUnitSelected?.Invoke(result);
            }
            else
            {
                HexTerrainSelected?.Invoke(result);
            }
        }
        */

        if (FindTarget(mousePosition, out Hex result))
        {
            if (UnitSelected(result))
            {
                HexTerrainSelected?.Invoke(result, isClick);
            }
        }
    }

    
    /// <summary>
    /// 선택한 오브젝트가 Player 캐릭터를 확인
    /// </summary>
    /// <param name="result">선택된 오브젝트</param>
    /// <returns>true 반환</returns>
    private bool UnitSelected(GameObject result)
    {
        return result.GetComponent<WorldMapPlayerCharacter>() != null;
    }
    
    /// <summary>
    /// 선택한 오브젝트가 Hex 확인를 확인
    /// </summary>
    /// <param name="result">선택된 Hex</param>
    /// <returns>true 반환</returns>
    private bool UnitSelected(Hex result)
    {
        return result != null;
    }

    /*
    private bool FindTarget(Vector3 mousePosition, out GameObject result)
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, SelectionMask)) // Hex Tile 또는 Player 캐릭터 클릭 시
        {
            result = hit.collider.gameObject;
            return true;
        }

        result = null;
        return false;
    }
    */
    
    private bool FindTarget(Vector3 mousePosition, out Hex result)
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, SelectionMask)) // Hex Tile 또는 Player 캐릭터 클릭 시
        {
            result = hit.collider.GetComponent<Hex>();
            return true;
        }

        result = null;
        return false;
    }
}
