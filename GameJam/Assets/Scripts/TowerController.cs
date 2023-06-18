using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface ICharacterFunc
{
    void Damaged(int nDamage);

    Transform GetTransform();

    bool IsDead();
}

public class TowerController : MonoBehaviour, ICharacterFunc
{
    public enum eTowerState
    {
        Idle,
        Attack,
        Dead,
    }

    public int m_nTotalHp = 5;
    public int m_nHp = 0;

    public int m_nDamage = 0;
    public float m_fAttackSpeed = 0f;
    public float m_fPresentAttackSpeed = 0f;
    public float m_fAttackRange = 1f;

    public bool m_bDead = false;

    public eTowerState m_eState = eTowerState.Idle;

    public Enemy m_enemy = null;

    public Transform m_shootPos = null;
    public ObjectPool<Bullet> m_bulletPool = null;
    public Bullet bulletPrefab = null;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        m_bulletPool = new ObjectPool<Bullet>(
            createFunc: () =>
            {
                var createdBullet = Instantiate(bulletPrefab);
                return createdBullet;
            },
            actionOnGet: (bullet) =>
            {
                if(m_enemy != null)
                {
                    Vector2 direction = (m_enemy.transform.position - this.transform.position).normalized;
                    bullet.transform.position = m_shootPos.position;
                    bullet.Setup(direction);
                    bullet.gameObject.SetActive(true);
                }
            },
            actionOnRelease: (bullet) =>
            {
                bullet.gameObject.SetActive(false);
            },
            actionOnDestroy: (bullet) =>
            {
                Destroy(bullet.gameObject);
            }, maxSize: 30);
    }

    public void Setup()
    {
        m_bDead = false;
        m_nHp = m_nTotalHp;

        m_nDamage = GameManager.Instance.m_nTowerDamageLevel * GameManager.nTowerDamageIncrease;
        m_fAttackSpeed = 1f - GameManager.Instance.m_nTowerAttackSpeedLevel * GameManager.fTowerAttackSpeedIncrease;
        m_fPresentAttackSpeed = m_fAttackSpeed;
        //m_fAttackRange = 2f;

        m_eState = eTowerState.Idle;
    }

    private void Update()
    {
        if(m_bDead == true)
        {
            return;
        }

        switch(m_eState)
        {
            case eTowerState.Idle:
                {
                    if (m_enemy == null)
                    {
                        lock (GameManager.Instance.m_listEnemy)
                        {
                            List<Enemy> list = GameManager.Instance.m_listEnemy;
                            for (int i = 0; i < list.Count; i++)
                            {
                                Enemy enemy = list[i];
                                if (Vector2.Distance(this.transform.position, enemy.transform.position) <= m_fAttackRange)
                                {
                                    m_enemy = enemy;
                                    break;
                                }
                            }
                        }
                    }

                    if (m_enemy != null && m_fPresentAttackSpeed >= m_fAttackSpeed)
                    {
                        m_eState = eTowerState.Attack;
                        return;
                    }
                    m_fPresentAttackSpeed += Time.deltaTime;
                }
                break;
            case eTowerState.Attack:
                {
                    if(m_enemy == null)
                    {
                        m_eState = eTowerState.Idle;
                        return;
                    }
                    if(m_enemy.m_bDead == true)
                    {
                        m_enemy = null;
                        m_eState = eTowerState.Idle;
                        return;
                    }

                    m_bulletPool.Get();
                    m_fPresentAttackSpeed = 0f;
                    m_eState = eTowerState.Idle;
                }
                break;
        }
    }

    public void Damaged(int nDamage)
    {
        m_nHp -= nDamage;
        if(m_nHp <= 0)
        {
            m_bDead = true;
        }
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public bool IsDead()
    {
        return m_bDead;
    }
}
