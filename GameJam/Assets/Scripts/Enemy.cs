using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacterFunc
{
    public enum eState
    {
        Idle,
        Move,
        Attack,
    }

    public eState m_eState = eState.Idle;

    public BoxCollider2D m_collider = null;
    public Rigidbody2D m_rigidBody = null;

    public bool m_bDead = false;

    public int m_nTotalHp = 5;
    public int m_nHp = 0;

    public int m_nDamage = 0;
    public float m_fAttackRange = 1f;

    public ICharacterFunc m_target = null;

    public float m_fAttackSpeed = 1f;
    public float m_fPresentAttackSpeed = 0f;

    public float m_fMoveSpeed = 10f;

    private void Awake()
    {
        Setup(10, 10);
    }

    public void Setup(int nDamage, int nTotalHp)
    {
        m_eState = eState.Idle;
        m_nTotalHp = nTotalHp;
        m_nDamage = nDamage;
        m_bDead = false;
        m_target = null;
        m_fAttackRange = transform.localScale.x * m_collider.size.x;

        m_fPresentAttackSpeed = m_fAttackSpeed;
    }

    private void FixedUpdate()
    {
        if(m_bDead == true)
        {
            return;
        }
        switch(m_eState)
        {
            case eState.Idle:
                {
                    if (m_target == null)
                    {
                        float fDistance = 0f;
                        ICharacterFunc target = null;
                        var list = GameManager.Instance.m_listPlayerTeam;
                        if (list.Count > 0)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                ICharacterFunc _target = list[i];
                                float _fDistance = Vector2.Distance(transform.position, _target.GetTransform().position);

                                if (i == 0)
                                {
                                    target = _target;
                                    fDistance = _fDistance;
                                    continue;
                                }

                                if (fDistance > _fDistance)
                                {
                                    target = _target;
                                    fDistance = _fDistance;
                                }
                            }
                            m_target = target;
                            m_eState = eState.Move;
                            return;
                        }
                    }
                    else
                    {
                        m_eState = eState.Move;
                        return;
                    }
                }
                break;
            case eState.Move:
                {
                    if(m_target == null)
                    {
                        m_eState = eState.Idle;
                        return;
                    }
                    if(m_target.IsDead() == true)
                    {
                        m_target = null;
                        m_eState = eState.Idle;
                        m_rigidBody.velocity = Vector2.zero;
                        return;
                    }

                    Vector2 targetPos = m_target.GetTransform().position;
                    float fDistance = Vector2.Distance(transform.position, targetPos);

                    if(fDistance <= m_fAttackRange)
                    {
                        m_rigidBody.velocity = Vector2.zero;
                        m_eState = eState.Attack;
                        return;
                    }
                    m_rigidBody.velocity = (targetPos - (Vector2)transform.position).normalized * Time.deltaTime * m_fMoveSpeed;
                }
                break;
            case eState.Attack:
                {
                    if (m_target == null)
                    {
                        m_eState = eState.Idle;
                        return;
                    }
                    if (m_target.IsDead() == true)
                    {
                        m_target = null;
                        m_eState = eState.Idle;
                        m_rigidBody.velocity = Vector2.zero;
                        return;
                    }
                    if (m_fPresentAttackSpeed >= m_fAttackSpeed)
                    {
                        m_fPresentAttackSpeed = 0f;
                        m_target.Damaged(m_nDamage);
                    }
                    else
                    {
                        m_fPresentAttackSpeed += Time.deltaTime;
                    }
                }
                break;
        }
        
    }

    public void Damaged(int nDamage)
    {
        m_nHp -= nDamage;
        if(m_nHp == 0)
        {
            m_bDead = true;
            GameManager.Instance.m_enemyPool.Release(this);
            if(GameManager.Instance.m_towerController.m_enemy == this)
            {
                GameManager.Instance.m_towerController.m_enemy = null;
            }
            gameObject.SetActive(false);
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
