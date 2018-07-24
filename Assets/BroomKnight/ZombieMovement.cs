using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    protected Transform m_player;
    protected static PlayerMovement m_playerMovement;
    private Rigidbody m_rigidbody;
    public float m_moveSpeed = 10;
    public float m_maxSpeed = 30;

    public int forwardMult = -1;
    public Vector3 m_scaleMult = new Vector3(1, 1, -1);

    [System.NonSerialized] public IEnumerator m_disableMove;

    private bool isGrounded = false;
    public LayerMask lm;

    public bool unstopable = false;
    // Use this for initialization
    public virtual void Awake()
    {
        if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        m_rigidbody = transform.GetComponent<Rigidbody>();

        Vector3 tVec = transform.localScale;

        float xMult = 1;
        if(m_player.position.x > transform.position.x)
        {
            xMult = -1;
        }

        transform.localScale = new Vector3(tVec.x * m_scaleMult.x * xMult, tVec.y * m_scaleMult.y, tVec.z * m_scaleMult.z);
        Vector3 playerNoneY = new Vector3(m_player.position.x, transform.position.y, m_player.position.z);
        transform.LookAt(playerNoneY);
    }

    public virtual void Start()
    {
        //do nothing
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        M_MovementUpdate();
    }

    public virtual void M_MovementUpdate()
    {
        if(m_disableMove != null)
        {
            return;
        }
        //transform.position += transform.forward* forwardMult * Time.deltaTime * m_moveSpeed;
        m_rigidbody.AddForce(transform.forward * forwardMult * Time.deltaTime * m_moveSpeed, ForceMode.VelocityChange);
        Vector3 currVel = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
        float velY = m_rigidbody.velocity.y;
        if (currVel.magnitude > m_maxSpeed)
        {
            //m_rigidbody.velocity = m_rigidbody.velocity.normalized * m_maxSpeed;
            currVel = m_rigidbody.velocity.normalized * m_maxSpeed;
            m_rigidbody.velocity = new Vector3(currVel.x, velY, currVel.z);
        }
    }

    public void DisableMovement(float time)
    {
        if(unstopable)
        {
            return;
        }

        if(m_disableMove != null)
        {
            return;
        }
        m_disableMove = DisableMove(time);
        StartCoroutine(m_disableMove);
    }

    IEnumerator DisableMove(float time)
    {
        float startTime = Time.time;
        yield return new WaitForSeconds(time);
        while(Mathf.Abs(m_rigidbody.velocity.y) > 0.01f || !IsGrounded())
        {
            yield return new WaitForSeconds(0.3f);
        }
        m_disableMove = null;
    }

    bool IsGrounded()
    {
        RaycastHit rHit;
        bool b = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out rHit, 2, lm);
        
        return b;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Terrain")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Terrain")
        {
            isGrounded = false;
        }
    }
}