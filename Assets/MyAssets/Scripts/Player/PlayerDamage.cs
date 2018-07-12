using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {
    protected int m_damage;
    protected bool m_active = false;

	// Use this for initialization
	void Awake () {
        
	}

    public void M_SetDamage(int damage, bool active)
    {
        this.m_damage = damage;
        m_active = active;
    }
	
    void OnTriggerEnter(Collider col)
    {
        if(!m_active)
        {
            return;
        }

        if(col.gameObject.tag != "Enemy")
        {
            return;
        }

        Health hs = col.GetComponent<Health>();

        if(hs != null)
        {
            hs.M_Damage(m_damage);
        }
    }
}
