using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    public List<int> EnemyList;
    public List<int> PlayerList;

    public Button start;
    public void GetUnitData()
    {
       // Managers.Battle.BattleInit(PlayerList, EnemyList, "BattleMap_1");
        start.gameObject.SetActive(false);
    }
}
