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


    private AudioSource asource;
    public AudioClip[] aclip;
    private int currAudioClipIndex = 0;
    private float cdaudiohit = 0.001f;
    private float cdaudiohitTimer = 0.0f;

    public GameObject pausMenuObject;

    private static int killedLastCheck = 0;
    public ParticleSystem psKillcount;
    private Color psKillcountStartColor;
    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_startScale = transform.localScale;

        m_startPos = m_moveObject.position;

        m_broom = GetComponentInChildren<PlayerBroom>();
        m_broomCol = m_broom.GetComponent<Collider>();

        asource = GetComponent<AudioSource>();

        Cursor.visible = false;

        var main = psKillcount.main;
        psKillcountStartColor = main.startColor.color;

        StartCoroutine(InitGame());
        StartCoroutine(KillCountChecker());
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

    void PlaySwingSound()
    {
        if (cdaudiohitTimer > Time.time)
        {
            return;
        }
        cdaudiohitTimer = Time.time + cdaudiohit;

        float pitchra = Random.Range(0.85f, 1.15f);
        asource.pitch = pitchra;

        asource.PlayOneShot(aclip[currAudioClipIndex]);

        currAudioClipIndex++;
        if (currAudioClipIndex >= aclip.Length)
        {
            currAudioClipIndex = 0;
        }
    }

    // Update is called once per frame
    void Update () {
        if (!hasBegun)
        {
            return;
        }
        //float input = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
            pausMenuObject.SetActive(!pausMenuObject.activeSelf);
            if(pausMenuObject.activeSelf)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

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
            PlaySwingSound();
            m_animator.SetTrigger(GetRandomAttack("Attack", 3));
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //left
        {
            transform.localScale = new Vector3(m_startScale.x  *- 1, transform.localScale.y, transform.localScale.z);
            PlaySwingSound();
            m_animator.SetTrigger(GetRandomAttack("Attack", 3));
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //up
        {
            PlaySwingSound();
            m_animator.SetTrigger(GetRandomAttack("AttackUp", 2));
            //m_animator.SetTrigger("AttackUp");
        }



    }


    IEnumerator KillCountChecker()
    {
        while (this != null)
        {
            yield return new WaitForSeconds(0.5f);
            Health hcol = FindObjectOfType<Health>();
            if (hcol != null)
            {
                int killcount = hcol.GetKillCount();

                int difference = Mathf.Abs(killcount - killedLastCheck);

                //use difference to show particlesystem!
                float alpha = difference / 15;

                var main = psKillcount.main;
                main.startColor = new Color(psKillcountStartColor.r, psKillcountStartColor.g, psKillcountStartColor.b, alpha);

                killedLastCheck = killcount;

            }
        }
    }

    string GetRandomAttack(string baseName, int maxIndex)
    {
        int r = Random.Range(1, maxIndex+1);
        return baseName + r.ToString();
    }
}
