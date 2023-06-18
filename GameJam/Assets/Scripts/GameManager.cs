using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoSingleton<GameManager>
{
    public static int nTowerDamageIncrease = 3;
    public static float fTowerAttackSpeedIncrease = 0.1f;

    public int m_nTowerDamageLevel = 0;
    public int m_nTowerAttackSpeedLevel = 0;

    public int m_nAwakePercentLevel = 0;
    public int m_nAwakeIncreaseHpLevel = 0;
    public int m_nAwakeIncreaseDamageLevel = 0;

    public List<ICharacterFunc> m_listPlayerTeam = new List<ICharacterFunc>();

    public ObjectPool<Enemy> m_enemyPool = null;
    public Enemy m_enemyPrefab = null;

    public List<Enemy> m_listEnemy = new List<Enemy>();

    public TowerController m_towerController = null;

    public float m_fLimitWaveLevelUpTime = 15f;
    public float m_fPresentWaveLevelUpTime = 0f;
    public int m_nWaveLevel = 0;

    public float m_fLimitSpawnTime = 1f;
    public float m_fPresentSpawnTime = 0f;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        m_nWaveLevel = 0;
        m_enemyPool = new ObjectPool<Enemy>(
            createFunc: () =>
            {
                var createEnemy = Instantiate(m_enemyPrefab);
                return createEnemy;
            },
            actionOnGet: (enemy) =>
            {
                int nEnemyLevel = GetEnemyLevel();
                enemy.Setup(nEnemyLevel, nEnemyLevel * 5);
                enemy.transform.position = Random.Range(0, 2) == 0 ?
                new Vector2(Random.Range(0, 2) == 0 ? -3 : 3, Random.Range(-5.5f, 5.5f)) :
                new Vector2(Random.Range(-3f, 3f), Random.Range(0, 2) == 0 ? -5.5f : 5.5f);
                enemy.gameObject.SetActive(true);
                m_listEnemy.Add(enemy);
            },
            actionOnRelease: (enemy) =>
            {
                enemy.gameObject.SetActive(false);
                if (m_listEnemy.Contains(enemy))
                {
                    m_listEnemy.Remove(enemy);
                }
            },
            actionOnDestroy: (enemy) =>
            {
                Destroy(enemy.gameObject);
            }, maxSize: 100);

        Setup();
    }

    public void Setup()
    {
        m_listPlayerTeam.Clear();
        m_listPlayerTeam.Add(m_towerController);

        m_towerController.Setup();
    }

    private void FixedUpdate()
    {
        if(m_fLimitWaveLevelUpTime <= m_fPresentWaveLevelUpTime)
        {
            m_fPresentWaveLevelUpTime = 0f;
            m_nWaveLevel += 1;
            return;
        }
        else
        {
            m_fPresentWaveLevelUpTime += Time.deltaTime;
        }

        if(m_fLimitSpawnTime <= m_fPresentSpawnTime)
        {
            m_fPresentSpawnTime = 0f;
            m_enemyPool.Get();
        }
        else
        {
            m_fPresentSpawnTime += Time.deltaTime;
        }
    }

    public int GetEnemyLevel()
    {
        float nRandom = Random.Range(0f, 10f);
        if(m_nWaveLevel >= nRandom)
        {
            return 2;
        }
        return 1;
    }
}
