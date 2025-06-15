using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    // 물리적인 오브젝트 배치 좌표 offset
    public static readonly float xOffSet = 2f, yOffSet = 1f, zOffSet = 1.73f;

    [Header("OffSet Coordinates")] 
    [SerializeField] private Vector3Int _offSetCoordinates;

    private void Awake()
    {
        _offSetCoordinates = ConvertPositionToOffSet(transform.position);
    }

    public static Vector3Int ConvertPositionToOffSet(Vector3 transformPosition)
    {
        int x = Mathf.CeilToInt(transformPosition.x / xOffSet);
        int y = Mathf.RoundToInt(transformPosition.y / yOffSet);
        int z = Mathf.RoundToInt(transformPosition.z / zOffSet);

        return new Vector3Int(x, y, z);
    }

    public Vector3Int GetHexCoords()
    {
        return _offSetCoordinates;
    }
}
