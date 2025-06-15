using System;
using System.Collections;
using UnityEngine;

public class PlayerWorldMapCamera : MonoBehaviour
{

    /*  << 월드 맵 카메라 주 목표 >>
     *   1. 플레이어 턴(차례)이(가) 올 경우, (OK)
     *   2. 플레이어가 이동할 경우에만 따라오게 하기
     */
    
    #region 카메라 좌표 및 이동속도

    // 월드맵 카메라 캐싱
    private Camera _worldMapCamera;
    // 카메라 고정 좌표
    private readonly float[] _WORLD_MAP_CAMERA_OFFSET = {0f, 12f, -10f};
    // 카메라 이동 속도
    private const float _CAMERA_MOVE_SPEED = 20f;

    #endregion


    private void Awake()
    {
        // 월드맵 카메라 캐싱
        _worldMapCamera = Camera.main;
    }



    private void LateUpdate()
    {
        /*
        if (Managers.Turn.PlayerTurn == null) return;

        Vector3 playerPosition = Managers.Turn.PlayerTurn.transform.position;
        _worldMapCamera.transform.position = new Vector3(playerPosition.x, 
                                        WORLD_MAP_CAMERA_OFFSET[1], 
                                        playerPosition.z + WORLD_MAP_CAMERA_OFFSET[2]);
                                        */
    }
    
    
    public void  ReTargetToCamera(Vector3 playerPos) {
        // 다음 플레이어를 가져옵니다.
        Vector3 nextPlayer = Managers.Turn.PlayerTurn.transform.position;

        // 다음 플레이어의 위치와 방향으로 카메라를 부드럽게 이동
        Vector3 startPos = _worldMapCamera.transform.position;
        Vector3 endPos = new Vector3(nextPlayer.x, 
                       _WORLD_MAP_CAMERA_OFFSET[1], 
                        nextPlayer.z + _WORLD_MAP_CAMERA_OFFSET[2]);
        StartCoroutine(MoveCamera(startPos, endPos));
    }

    private IEnumerator MoveCamera(Vector3 startPos, Vector3 endPos) {
        //print($"카메라 시작위치 {startPos} / 종료위치 {endPos}");
        
        float elapsedTime = 0f;
        float totalTime = Vector3.Distance(startPos, endPos) / _CAMERA_MOVE_SPEED;
       // print($"카메라 이동 시간 : {totalTime}");

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / totalTime);
            _worldMapCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _worldMapCamera.transform.position = endPos;
    }
}
