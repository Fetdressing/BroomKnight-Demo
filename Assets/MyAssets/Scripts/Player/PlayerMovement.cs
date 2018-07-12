using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public int m_damage;
    public AnimationCurve m_curve;
    public PlayerDamage[] m_dmgObjects;
}

public class PlayerMovement : MonoBehaviour {
    [System.NonSerialized]
    public Vector3 m_startPos;
    public Vector3 m_forward = Vector3.forward;
    public Vector3 m_right = Vector3.right;

    IEnumerator m_currAttack;
    public Attack m_normalAttack;

    protected Health m_health;
    public Collider[] m_damageColliders;
	// Use this for initialization
	void Awake () {
        m_startPos = transform.position;

        m_health = transform.GetComponent<Health>();
    }
	
	// Update is called once per frame
	void Update () {

        if(m_currAttack != null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            M_Rotate(-m_right);
            Attack(m_normalAttack);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            M_Rotate(m_right);
            Attack(m_normalAttack);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            M_Rotate(m_forward);
            Attack(m_normalAttack);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            M_Rotate(-m_forward);
            Attack(m_normalAttack);
        }
    }

    void M_Rotate(Vector3 newForward)
    {
        transform.forward = newForward;
    }

    void Attack(Attack attack)
    {
        m_currAttack = PerformAttack(attack);

        if(m_currAttack != null)
        {
            StopCoroutine(m_currAttack); //TODO change later
        }
        StartCoroutine(m_currAttack);
    }

    IEnumerator PerformAttack(Attack attack)
    {
        float zVal = 0;
        float animTime = attack.m_curve.keys[attack.m_curve.length - 1].time;
        float startTime = Time.time;

        M_ToggleColliders(true);
        M_ToggleDmgObjects(attack, true);
        m_health.M_TurnImmortal(animTime * 0.6f); //make us immortal so that we dont accidently die during attack

        while((startTime + animTime) > Time.time)
        {
            zVal = attack.m_curve.Evaluate(Time.time - startTime);
            transform.position = m_startPos + (transform.forward * zVal);
            yield return new WaitForEndOfFrame();
        }

        transform.position = m_startPos;
        M_ToggleDmgObjects(attack, false);
        M_ToggleColliders(false);

        m_currAttack = null;
    }

    void M_ToggleDmgObjects(Attack a, bool b)
    {
        for (int i = 0; i < a.m_dmgObjects.Length; i++)
        {
            a.m_dmgObjects[i].M_SetDamage(a.m_damage, b);
        }
    }

    void M_ToggleColliders(bool b)
    {
        for(int i = 0; i < m_damageColliders.Length; i++)
        {
            m_damageColliders[i].enabled = b;
        }
    }
}
