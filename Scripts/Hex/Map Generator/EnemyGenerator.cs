using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator
{

    /* 1. Solo�� �Ѹ�
     * 2. �׷��� ������ ��ü 3��
     * 3. ķ���� ���� 1��, ���� 1~2�� (���ϴ� ������ ��ü�� �ƴҼ� �� ����)
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
