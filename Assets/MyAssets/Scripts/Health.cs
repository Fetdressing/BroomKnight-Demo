using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    [System.NonSerialized]
    public int m_currHealth;
    public int m_maxHealth = 1;

    public GameObject m_deathParticle;

    [System.NonSerialized]
    public bool m_immortal = false;
    protected IEnumerator m_immortalCo;
	// Use this for initialization
	void Awake () {
        m_currHealth = m_maxHealth;

        if(m_deathParticle)
        {
            JEffectPool.CreatePool(m_deathParticle, 10);
        }

        m_immortal = false;
    }

    public bool M_Damage(int d) //return false if died
    {
        if(m_immortal)
        {
            return true;
        }

        m_currHealth = Mathf.Max(0, m_currHealth - d);

        if(m_currHealth <= 0)
        {
            //dead
            M_Die();
            return false;
        }

        return true;
    }

    public void M_Die()
    {
        if (m_deathParticle)
        {
            var hitobj = JEffectPool.CreateEffect(m_deathParticle);
            hitobj.transform.position = transform.position;
            hitobj.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            //hitobj.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, hitobj.transform.rotation.eulerAngles.y, hitobj.transform.rotation.eulerAngles.z); //endast för 2D
        }

        Destroy(gameObject);
    }

    public void M_Die(Vector3 vec)
    {
        if (m_deathParticle)
        {
            var hitobj = JEffectPool.CreateEffect(m_deathParticle);
            ExplosionChildren expl = hitobj.GetComponent<ExplosionChildren>();
            if (expl)
            {
                expl.Explode(vec);
            }
            hitobj.transform.position = transform.position;
            hitobj.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            //hitobj.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, hitobj.transform.rotation.eulerAngles.y, hitobj.transform.rotation.eulerAngles.z); //endast för 2D
        }

        Destroy(gameObject);
    }

    public void M_TurnImmortal(float time)
    {
        if(m_immortalCo != null)
        {
            StopCoroutine(m_immortalCo);
        }

        m_immortalCo = MI_TurnImmortal(time);
        StartCoroutine(m_immortalCo);
    }

    IEnumerator MI_TurnImmortal(float time)
    {
        m_immortal = true;
        yield return new WaitForSeconds(time);
        m_immortal = false;
    }
}
