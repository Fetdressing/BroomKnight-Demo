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
    public AudioClip pausMenuSound;

    private static int killedLastCheck = 0;
    public ParticleSystem psKillcount;
    private Color psKillcountStartColor;
    private int killcountValue = 0;

    private Renderer[] rends;
    private Color rendStartColor;
    public Color killcountColor = Color.white;

    protected bool isCCed = false;
    public GameObject m_ccedParticle;
    public Color ccedColor = Color.white;
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

        rends = GetComponentsInChildren<Renderer>();
        rendStartColor = rends[0].material.GetColor("_Color");

        //Time.timeScale = 0.2f;
        if (m_ccedParticle)
        {
            JEffectPool.CreatePool(m_ccedParticle, 5);
        }

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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) && pausMenuObject.activeSelf)
        {
            Cursor.visible = false;
            asource.PlayOneShot(pausMenuSound);
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

        //if(isCCed)
        //{
        //    return;
        //}

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
            //m_animator.SetTrigger(GetRandomAttack("Attack", 3));
            m_animator.SetTrigger("Attack");
            m_animator.SetInteger("MoveHorState", GetRandomAttack(3));
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) //left
        {
            transform.localScale = new Vector3(m_startScale.x  *- 1, transform.localScale.y, transform.localScale.z);
            PlaySwingSound();
            //m_animator.SetTrigger(GetRandomAttack("Attack", 3));
            m_animator.SetTrigger("Attack");
            m_animator.SetInteger("MoveHorState", GetRandomAttack(3));
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) //up
        {
            PlaySwingSound();
            //m_animator.SetTrigger(GetRandomAttack("AttackUp", 2));
            m_animator.SetTrigger("Attack");
            m_animator.SetInteger("MoveVerState", GetRandomAttack(2));
            //m_animator.SetTrigger("AttackUp");
        }
        
    }

    public void ResetStateIntegers() //called from blendtree
    {
        m_animator.SetInteger("MoveHorState", 0); //idle
        m_animator.SetInteger("MoveVerState", 0); //idle
        m_animator.ResetTrigger("Attack");
    }
    
    string GetRandomAttack(string baseName, int maxIndex)
    {
        int r = Random.Range(1, maxIndex+1);
        return baseName + r.ToString();
    }

    int GetRandomAttack(int maxIndex)
    {
        int r = Random.Range(1, maxIndex + 1);
        return r;
    }

    IEnumerator KillCountChecker()
    {
        while (this != null)
        {
            float alpha = 0;

            Health hcol = FindObjectOfType<Health>();
            if (hcol != null)
            {
                int killcount = hcol.GetKillCount();

                int difference = Mathf.Abs(killcount - killedLastCheck);

                killcountValue += difference;
                killcountValue = Mathf.Max(0, killcountValue - 1);
                //use difference to show particlesystem!
                alpha = killcountValue / 4;

                killedLastCheck = killcount;

            }

            if (!isCCed)
            {
                var main = psKillcount.main;
                main.startColor = new Color(psKillcountStartColor.r, psKillcountStartColor.g, psKillcountStartColor.b, alpha);

                if (alpha >= 1f)
                {
                    for (int i = 0; i < rends.Length; i++)
                    {
                        rends[i].material.color = killcountColor;
                    }

                    m_animator.SetFloat("ExtraSpeed", 1.2f);
                }
                else
                {
                    for (int i = 0; i < rends.Length; i++)
                    {
                        rends[i].material.color = new Color(rendStartColor.r, rendStartColor.g, rendStartColor.b);
                    }
                    m_animator.SetFloat("ExtraSpeed", 1.0f);
                }
            }
            else
            {
                var main = psKillcount.main;
                main.startColor = new Color(psKillcountStartColor.r, psKillcountStartColor.g, psKillcountStartColor.b, 0); //ccad, visa ingen köttarparticle
            }

            yield return new WaitForSeconds(0.7f);
        }
    }

    public void CC()
    {
        if(m_broom.enabled == true)
        {
            return;
        }

        if(isCCed == true)
        {
            return;
        }

        if (m_ccedParticle)
        {
            var hitobj = JEffectPool.CreateEffect(m_ccedParticle);
            hitobj.transform.position = transform.position + new Vector3(0, 5, 0);
            hitobj.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);

            hitobj.GetComponent<AudioSource>().Play();
            //hitobj.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, hitobj.transform.rotation.eulerAngles.y, hitobj.transform.rotation.eulerAngles.z); //endast för 2D
        }

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = ccedColor;
        }

        m_animator.SetFloat("ExtraSpeed", 0.6f);
        StartCoroutine(GetCCed());
    }

    IEnumerator GetCCed()
    {
        isCCed = true;
        yield return new WaitForSeconds(2.5f);
        isCCed = false;
    }

}
