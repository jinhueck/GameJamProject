using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public static int nTowerDamageIncrease = 3;
    public static float fTowerAttackSpeedIncrease = 0.1f;

    public int m_nTowerDamageLevel = 0;
    public int m_nTowerAttackSpeedLevel = 0;

    public int m_nAwakePercentLevel = 0;
    public int m_nAwakeIncreaseHpLevel = 0;
    public int m_nAwakeIncreaseDamageLevel = 0;

    public List<Enemy> m_listEnemy = new List<Enemy>();

    public TowerController m_towerController = null;


    public void Init()
    {
        
    }
}
