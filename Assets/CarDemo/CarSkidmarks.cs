using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSkidmarks : MonoBehaviour {
    private Rigidbody2D rigidB;
    private TrailRenderer[] skidmarkRenderers;

    public ParticleSystem dustParticleSystem;
    private float startdustEmitTime = 0.0f;
    private bool dustWasEmittedLastFrame = false;
    private IEnumerator dustEmitIE = null;

    // to determine what extra particleeffect we should display depending on groundtype
    private GroundType currGroundType;
    private ParticleSystem currGroundTypeParticleSystem;
    public LayerMask groundCheckLM;

    [Range(0.0f, 1.0f)]
    // a threshold for when the car is drifting, 1 meaning we are always drifting
    public float slideThreshold = 0.85f;
    // a threshold for when the car is accelerating fast enough to leave skidmarks
    public float accelerationThreshold = 1f;

    private Vector3 lastFrameVelocity = Vector3.zero;

    // Use this for initialization
    void Awake () {
        rigidB = transform.GetComponent<Rigidbody2D>();
        skidmarkRenderers = GetComponentsInChildren<TrailRenderer>();

        currGroundType = GroundTypeManager.Instance.defaultGroundType; // use the default ground type if no other is found
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(rigidB.velocity.magnitude < 5.5f) // make sure we are not standing "still" 
        {
            EmitDust(dustParticleSystem, false);
            if (currGroundTypeParticleSystem != null)
            {
                EmitDust(currGroundTypeParticleSystem, false);
            }

            for (int i = 0; i < skidmarkRenderers.Length; i++)
            {
                skidmarkRenderers[i].emitting = false;
            }
            return;
        }

        // find out which ground type we are on!
        GroundType oldGroundType = currGroundType; // get lastframe groundtype
        currGroundType = GroundTypeManager.Instance.defaultGroundType; // use the default ground type if no other is found

        // check what groundtype we are on
        RaycastHit rHit;
        if (Physics.Raycast(transform.position, Vector3.forward, out rHit, Mathf.Infinity, groundCheckLM))
        {            
            currGroundType = rHit.collider.gameObject.GetComponent<GroundTypeDummy>().groundType;
        }


        // a value which is greater than 0 if we are moving forward, and less than 0 if we are moving backwards
        float forwardCheck = Vector3.Dot(rigidB.velocity.normalized, transform.up);
        float forwAbs = Mathf.Abs(forwardCheck);

        bool isLeavingSkidmarks = false;
        float skidmarkValue = (1 + slideThreshold) * 0.5f; // a threshold value which determines if we are sliding or not

        float acceleration = Mathf.Abs(rigidB.velocity.magnitude - lastFrameVelocity.magnitude) * Time.fixedDeltaTime * 100;


        if (forwAbs < skidmarkValue || acceleration > 1f) // we are sliding!
        {
            isLeavingSkidmarks = true;

            if(!dustWasEmittedLastFrame)
            {
                startdustEmitTime = Time.time;
            }
            else
            {
                startdustEmitTime = startdustEmitTime; // leave it unchanged
            }
        }
        else // we are not sliding!
        {
            isLeavingSkidmarks = false;
            startdustEmitTime = Time.time;
        }


        for (int i = 0; i < skidmarkRenderers.Length; i++) // show skidmarks
        {
            skidmarkRenderers[i].emitting = isLeavingSkidmarks;
        }

        bool emitDust;
        const float emitDustDelay = 0.3f;
        if((Time.time - startdustEmitTime) > emitDustDelay) // only start emitting dust after a certain time
        {
            emitDust = true;
        }
        else
        {
            emitDust = false;
        }
        EmitDust(dustParticleSystem, emitDust);

        if (currGroundTypeParticleSystem != null)
        {
            EmitDust(currGroundTypeParticleSystem, true); // we have to set this to true since it might be set to false at beginning of last frames update
        }

        // activate the specific groundtype dust particlesystem
        if (oldGroundType != currGroundType)
        {
            if (currGroundTypeParticleSystem != null)
            {
                Destroy(currGroundTypeParticleSystem, 5);
                EmitDust(currGroundTypeParticleSystem, false); // disable the old particlesystem
                currGroundTypeParticleSystem = null;
            }

            if (currGroundType.dustParticleSystem != null)
            {
                currGroundTypeParticleSystem = GameObject.Instantiate(currGroundType.dustParticleSystem).GetComponent<ParticleSystem>(); // instantiate the new particlesystem when a new groundtype is presented
                currGroundTypeParticleSystem.transform.SetParent(dustParticleSystem.transform);
                currGroundTypeParticleSystem.transform.localPosition = Vector3.zero;

                EmitDust(currGroundTypeParticleSystem, true);
            }
        }


        dustWasEmittedLastFrame = isLeavingSkidmarks;
        lastFrameVelocity = new Vector3(rigidB.velocity.x, rigidB.velocity.y, 0);
    }


    void EmitDust(ParticleSystem ps, bool emit)
    {
        if(dustParticleSystem == null)
        {
            return;
        }

        ParticleSystem.EmissionModule eMod = ps.emission;
        eMod.enabled = emit;
    }

}
