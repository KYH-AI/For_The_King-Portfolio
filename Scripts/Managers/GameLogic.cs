using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    public enum Scene
    {
        None = -1,
        MainMenu = 0,
        InGame = 1
    }
    
    public enum GameResult
    {
        None = -1,
        Win = 0,
        Dead = 1,
        Exit = 2,
    }

    // 플레이어 HUD 타이틀 색상
    public readonly Color[] PLAYER_TITLE_COLOR = new Color[3]
    {
        new Color(202f / 255f, 83f / 255f, 83f / 255f),
        new Color(56f / 255f, 173f / 255f, 42f / 255f),
        new Color(73f / 255f, 119f / 255f, 204f / 255f),
    };

    private CharacterSelectManager.PlayerCharacterInfo[] _playerCharacterInfo;

    public bool IsDebuggingMode = false;
    
    public Scene Current_Scene { get; private set; } = Scene.None;
    
    public GameResult Game_Result { get; private set; } = GameResult.None;
    
    public static GameLogic Instance { get; private set; }
    
    public bool IsLogin { get; private set; }

    // 플레이어 최대 재화 
    public readonly int MAX_GOLD_VALUE = 10000;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public void LoginState(bool state)
    {
        IsLogin = state;
    }
    
    public void SetPlayerCharacterInfo(CharacterSelectManager.PlayerCharacterInfo [] playersInfo)
    {
        _playerCharacterInfo = playersInfo;
    }

    public int GetMaxPlayer()
    {
        return _playerCharacterInfo.Length;
    }

    public CharacterSelectManager.PlayerCharacterInfo GetPlayerCharacterInfo(int index)
    {
        return _playerCharacterInfo[index];
    }

    /// <summary>
    /// 게임 최종 결과
    /// </summary>
    /// <param name="gameResult">승리, 패배, 종료 등</param>
    public void SetGameResult(GameResult gameResult)
    {
        Game_Result = gameResult;
    }

    /// <summary>
    /// 씬 교체 함수
    /// </summary>
    /// <param name="sceneType">변경하고자는 씬</param>
    public void SceneChange(Scene sceneType, float delayTime = 0f)
    {
        Current_Scene = sceneType;
        StartCoroutine( LoadSceneCoroutine((int)sceneType, delayTime));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        
        // 로딩이 완료될 때까지 대기
        while (!asyncOperation.isDone)
        {
            // 로딩 진행 상황을 나타내는 값 (0부터 1까지)
           float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
           
            yield return null; // 다음 프레임까지 대기
        }

        if (sceneIndex == (int)Scene.MainMenu)// && Game_Result is GameResult.Dead)
        {
            EndOutroCallBack();
        }
    }

    private void EndOutroCallBack()
    {
        _playerCharacterInfo = null;
    }
}
