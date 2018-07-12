using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {
    public int m_damage = 1;

    protected Health m_health;
	// Use this for initialization
	void Awake () {
        m_health = GetComponent<Health>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "Player")
        {
            return;
        }

        Health hs = col.GetComponent<Health>();

        if (hs != null)
        {
            hs.M_Damage(m_damage);
        }

        m_health.M_Die();
    }
}
