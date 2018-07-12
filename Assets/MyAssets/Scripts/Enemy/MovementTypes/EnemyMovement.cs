using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    protected Transform m_player;
    protected static PlayerMovement m_playerMovement;
    public float m_moveSpeed = 10;
	// Use this for initialization
	public virtual void Awake () {
        if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        m_playerMovement = m_player.GetComponent<PlayerMovement>();
        transform.LookAt(m_playerMovement.m_startPos);
    }

    public virtual void Start()
    {
        //do nothing
    }
	
	// Update is called once per frame
	public virtual void Update ()
    {
        M_MovementUpdate();
	}

    public virtual void M_MovementUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * m_moveSpeed;
    }
}
