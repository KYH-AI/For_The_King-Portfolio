using UnityEngine;

public class WorldMapEnemyCharacter : MonoBehaviour, IEventInfo
{
    private Animator _worldMapEnemyAnimator;

    private WorldMapEventInfo _worldMapEventInfo;
    
    private void Awake()
    {
        _worldMapEnemyAnimator = GetComponent<Animator>();
    }
    
    public void InitEventInfo(EnemyStat enemyStatTest)
    {
        _worldMapEventInfo = new WorldMapEventInfo(enemyStatTest.EnemyID, enemyStatTest.EnemyName,
            "적을 발견했습니다", "상당히 약해 보이지만 약한거 맞습니다", enemyStatTest.EnemyLevel, enemyStatTest.EnemyIcon);
    }
    
    public void PlayAnimationTrigger(CharacterEventListener.AnimationTrigger animationTrigger)
    {
        _worldMapEnemyAnimator.SetTrigger(animationTrigger.ToString());
    }

    public int GetEventID()
    {
        return _worldMapEventInfo.GetEventID();
    }

    public int GetEventLevel()
    {
        return _worldMapEventInfo.GetEventLevel();
    }
  
    public string GetEventName()
    {
        return _worldMapEventInfo.GetEventName();
    }

    public void SetEventName(string editEventName)
    {
        _worldMapEventInfo.SetEventName(editEventName);
    }

    public string GetEventDetailText()
    {
        return _worldMapEventInfo.GetEventDetailText();
    }

    public string GetEventDescriptionText()
    {
        return _worldMapEventInfo.GetEventDescriptionText();
    }

    public Sprite GetEventPortrait()
    {
        return _worldMapEventInfo.GetEventPortrait();
    }
}
