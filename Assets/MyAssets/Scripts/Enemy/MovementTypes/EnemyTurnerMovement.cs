using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnerMovement : EnemyMovement
{
    public float m_turnDistance = 20;
    public float m_turnSpeed = 170;
    IEnumerator m_turnCo;
    protected bool m_isTurning = false;
	// Use this for initialization
	public override void Awake () {
        base.Awake();
        m_isTurning = false;
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        if(Vector3.Distance(m_player.position, transform.position) < m_turnDistance)
        {
            Turn();
        }

        if(m_isTurning)
        {
            return;
        }

        M_MovementUpdate();
	}

    protected void Turn()
    {
        if(m_turnCo != null)
        {
            return;
        }

        m_turnCo = TurnAround();
        StartCoroutine(m_turnCo);
    }

    IEnumerator TurnAround()
    {
        m_isTurning = true;

        float rotationleft = 180;
        float rotation = m_turnSpeed * Time.deltaTime;

        while(rotationleft > 0)
        {
            transform.RotateAround(m_playerMovement.m_startPos, Vector3.up, rotation);
            rotationleft -= rotation;
            yield return new WaitForEndOfFrame();
        }

        transform.LookAt(m_playerMovement.m_startPos);
        m_isTurning = false;
    }
}
