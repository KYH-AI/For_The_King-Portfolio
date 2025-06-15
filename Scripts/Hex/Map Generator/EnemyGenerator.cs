using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator
{

    /* 1. Solo는 한명
     * 2. 그룹은 동일한 객체 3명
     * 3. 캠프는 대장 1명, 부하 1~2명 (부하는 동일한 객체가 아닐수 도 있음)
     */

    public List<GameObject> CreateEnemyPrefab(int enemyCount, int enemyID)
    {
        List<GameObject> enemyPrefab = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            enemyPrefab.Add(Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.WorldMapEnemy,
                enemyID + "_World" + (DataManager.EnemyCharacter)enemyID));
        }

        return enemyPrefab;
    }

    public GameObject CreateEnemyPrefab(int enemyID)
    {
        return Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.WorldMapEnemy,
            enemyID + "_World" + (DataManager.EnemyCharacter)enemyID);
    }
}
