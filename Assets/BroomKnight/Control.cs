using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {
    private Animator m_animator;
    private PlayerBroom m_broom;
    private Collider m_broomCol;

    protected Vector3 m_startScale;
    protected float m_lastInput = 0;

    public Transform m_moveObject; //the object that moves when attacking

    [System.NonSerialized] public Vector3 m_startPos;

    private bool hasBegun = false;
    public GameObject startchattBubble;
    public SpriteAlphaFade sFade;
	// Use this for initialization
	void Start () {
        m_animator = GetComponent<Animator>();
        m_startScale = transform.localScale;

        m_startPos = m_moveObject.position;

        m_broom = GetComponentInChildren<PlayerBroom>();
        m_broomCol = m_broom.GetComponent<Collider>();

        StartCoroutine(InitGame());
    }

    IEnumerator InitGame()
    {
#if UNITY_EDITOR
        hasBegun = true;
        sFade.Fade();
        startchattBubble.SetActive(false);
        yield break;
#endif

        hasBegun = false;
        sFade.Fade();
        yield return new WaitForSeconds(1.0f);
        startchattBubble.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        startchattBubble.SetActive(false);
        hasBegun = true;
    }
	
	// Update is called once per frame
	void Update () {
        //if(!hasBegun)
        //{
        //    return;
        //}
        //float input = Input.GetAxis("Horizontal");


        if (Vector3.Distance(m_startPos, m_moveObject.position) > 0.2f)
        {
            m_broom.enabled = true;
            m_broomCol.enabled = true;
        }
        else
        {
            m_broom.enabled = false;
            m_broomCol.enabled = false;
        }

        if (Vector3.Distance(m_startPos, m_moveObject.position) > 2.0f) //så att man inte inputar när man är långt från platsen
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) //right
        {
            transform.localScale = new Vector3(m_startScale.x * 1, transform.localScale.y, transform.localScale.z);
            m_animator.SetTrigger(GetRandomAttack());
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //left
        {
            transform.localScale = new Vector3(m_startScale.x  *- 1, transform.localScale.y, transform.localScale.z);
            m_animator.SetTrigger(GetRandomAttack());
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //up
        {
            m_animator.SetTrigger("AttackUp");
        }



    }

    string GetRandomAttack()
    {
        int r = Random.Range(1, 4);
        return "Attack" + r.ToString();
    }
}
