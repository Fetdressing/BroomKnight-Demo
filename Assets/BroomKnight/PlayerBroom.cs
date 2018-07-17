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
	// Use this for initialization
	void Start () {
        lastFramePos = transform.position;

        m_control = transform.root.GetComponent<Control>();
        cShake = FindObjectOfType<CameraShake>();
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody rig = col.transform.GetComponent<Rigidbody>();
        if (rig == null)
        {
            rig = col.transform.root.GetComponent<Rigidbody>();
        }
        ZombieMovement zmov = col.transform.root.GetComponent<ZombieMovement>();

        if (rig != null)
        {
            GameObject tSmack = Instantiate(smackParticle, transform.position, transform.rotation);
            cShake.ShakeCamera(0.02f, 0.05f);

            if (zmov != null)
            {
                zmov.DisableMovement(1);
            }
            Vector3 moveDir = (col.transform.position - transform.root.position).normalized;

            rig.constraints = RigidbodyConstraints.None;
            rig.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            rig.AddForceAtPosition(moveDir * forceMult, transform.root.position, ForceMode.Impulse);
        }
    }
}
