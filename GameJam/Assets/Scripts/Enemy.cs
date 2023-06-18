using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool m_bDead = false;

    public int m_nTotalHp = 5;
    public int m_nHp = 0;

    public void Damaged(int nDamage)
    {
        m_nHp -= nDamage;
        if(m_nHp == 0)
        {
            m_bDead = true;
        }
    }
}
