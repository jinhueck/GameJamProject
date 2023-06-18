using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public float m_fCoolTime = 0f;

    public Rigidbody2D m_rigid = null;

    public void Setup(Vector2 direction)
    {
        m_Direction = direction;
        m_fCoolTime = 0f;
    }

    public Vector2 m_Direction = Vector2.zero;
    void Update()
    {
        m_rigid.velocity = m_Direction;
        if (m_fCoolTime >= 5f)
        {
            DestroyBullet();
        }
        m_fCoolTime += Time.deltaTime;
    }

    public void DestroyBullet()
    {
        GameManager.Instance.m_towerController.m_bulletPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().Damaged(GameManager.Instance.m_towerController.m_nDamage);
            DestroyBullet();
        }
    }
}
