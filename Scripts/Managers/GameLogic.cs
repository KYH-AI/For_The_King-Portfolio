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

    // �÷��̾� HUD Ÿ��Ʋ ����
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

    // �÷��̾� �ִ� ��ȭ 
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
    /// ���� ���� ���
    /// </summary>
    /// <param name="gameResult">�¸�, �й�, ���� ��</param>
    public void SetGameResult(GameResult gameResult)
    {
        Game_Result = gameResult;
    }

    /// <summary>
    /// �� ��ü �Լ�
    /// </summary>
    /// <param name="sceneType">�����ϰ��ڴ� ��</param>
    public void SceneChange(Scene sceneType, float delayTime = 0f)
    {
        Current_Scene = sceneType;
        StartCoroutine( LoadSceneCoroutine((int)sceneType, delayTime));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        
        // �ε��� �Ϸ�� ������ ���
        while (!asyncOperation.isDone)
        {
            // �ε� ���� ��Ȳ�� ��Ÿ���� �� (0���� 1����)
           float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
           
            yield return null; // ���� �����ӱ��� ���
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
