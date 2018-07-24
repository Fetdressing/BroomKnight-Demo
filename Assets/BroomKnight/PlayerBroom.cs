using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBroom : MonoBehaviour {
    protected Vector3 currMovVector;
    protected Vector3 lastFramePos;
    public float forceMult = 100;
    private Control m_control;

    public GameObject smackParticle;
    private CameraShake cShake;

    private AudioSource asource;
    public AudioClip[] aclip;
    private int currAudioClipIndex = 0;
    private float cdaudiohit = 0.001f;
    private float cdaudiohitTimer = 0.0f;
    // Use this for initialization
    void Start () {
        lastFramePos = transform.position;

        m_control = transform.root.GetComponent<Control>();
        cShake = FindObjectOfType<CameraShake>();

        asource = GetComponent<AudioSource>();
    }

    void PlayHitSound(Vector3 pos)
    {
        if(cdaudiohitTimer > Time.time)
        {
            return;
        }
        cdaudiohitTimer = Time.time + cdaudiohit;

        float pitchra = Random.Range(0.85f, 1.15f);
        asource.pitch = pitchra;

        asource.PlayOneShot(aclip[currAudioClipIndex]);

        currAudioClipIndex++;
        if(currAudioClipIndex >= aclip.Length)
        {
            currAudioClipIndex = 0;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody rig = col.transform.GetComponent<Rigidbody>();
        if (rig == null)
        {
            rig = col.transform.root.GetComponent<Rigidbody>();
        }
        ZombieMovement zmov = col.transform.root.GetComponent<ZombieMovement>();

        FallDeath falldeath = col.transform.root.GetComponentInChildren<FallDeath>();
        if(falldeath != null)
        {
            falldeath.FallingAnim();
        }

        Health hcol = col.transform.root.GetComponentInChildren<Health>();
        if(hcol != null)
        {
            hcol.m_killedbyPlayer = true;
        }

        if (rig != null)
        {
            GameObject tSmack = Instantiate(smackParticle, transform.position, transform.rotation);
            cShake.ShakeCamera(0.015f, 0.03f);

            PlayHitSound(col.transform.position);

            if (zmov != null)
            {
                zmov.DisableMovement(1);
            }
            Vector3 moveDir = (col.transform.position - transform.root.position).normalized;

            if (zmov != null && !zmov.unstopable)
            {
                rig.constraints = RigidbodyConstraints.None;
                rig.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                rig.useGravity = true;
            }
            rig.AddForceAtPosition(moveDir * forceMult, transform.root.position, ForceMode.Impulse);            
        }
    }
}
