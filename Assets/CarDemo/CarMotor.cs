using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Hello world.
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class CarMotor : MonoBehaviour
{
    [Header("Car Attributes")]
    public float driveForce = 1500;
    public float maxVelocity = 80;
    public float breakVelocityForce = 2;

    public float torqueForce = 900;
    public float maxTorqueForce = 55;

    // affected by different kinds of grounds
    private GroundType currGroundType;
    public LayerMask groundCheckLM;
    // how fast the velocity should be redirected towards the cars forward
    [Tooltip("How fast the velocity should be redirected towards the cars forward (Kinda like how wheels work).")]
    public float redirectVelocitySpeed = 4;

    [Tooltip("Drifting value between 0 and 1, 1 makes the car always be drifting.")]
    [Range(0.0f, 1.0f)]
    // a threshold for when the car is drifting, this is used for when example to determine if we should redirect the velocity of the car. 1 meaning we are always drifting
    public float slideThreshold = 0.7f;
    private bool isSliding = false;
    // linear drag of the rigidbody
    public float linearDrag = 0.8f;
    // angular drag of the rigidbody
    public float angularDrag = 1.2f;

    // start localscale of body object
    private Vector3 startBodyScale;
    // reference to rigidbody
    private Rigidbody2D rigidB;
    //velocity last frame, used to calculate currVelocityAcceleration
    private Vector3 lastFrameVelocity = Vector3.zero;
    //torque last frame, used to calculate currTorqueAcceleration
    private float lastFrameTorque = 0;

    [Header("Car Juice")]
    
    // the transforms that should act as wheels
    public Transform[] rotateWheels;
    // the start rotation of the wheels, sampled from the first wheel of rotateWheels
    private Quaternion startWheelRotation;
    // the maximum angle the wheels can reach from startWheelRotation
    public float maxWheelRotAngle = 15;
    // how fast the wheels should follow the rotation
    public float wheelRotSpeed = 900;
    

    // body offsetting
    // the object that handles position of the caross
    public Transform body;
    // how much the delay should affect the body from different directions
    private Vector3 bodyTranslationMult = new Vector3(1f, 1f, 0);
    private float bodyDistanceMult = 2.5f;
    private Vector3 bodyStartPos;
    private Quaternion bodyStartRot;
    // which values should be affected by the delayed body rotation
    private Vector3 bodyLocalRotationMult = new Vector3(0, 0.05f, 0);

    void Awake () {
        // make sure that nothing weird is done when creating this object in the editor, x and y rotation should always be zero
        float zVal = transform.eulerAngles.z;
        transform.eulerAngles = new Vector3(0, 0, zVal);

        rigidB = GetComponent<Rigidbody2D>();
        rigidB.drag = linearDrag;
        rigidB.angularDrag = angularDrag;

        startBodyScale = body.localScale;

        startWheelRotation = rotateWheels[0].localRotation;
        bodyStartPos = body.localPosition;/* transform.InverseTransformDirection(body.localPosition);  body.localPosition*/
        bodyStartRot = body.localRotation;

        currGroundType = GroundTypeManager.Instance.defaultGroundType; // use the default ground type if no other is found
    }
    
    void Update()
    {
        currGroundType = GroundTypeManager.Instance.defaultGroundType; // use the default ground type if no other is found

        // check what groundtype we are on
        RaycastHit rHit;
        if(Physics.Raycast(transform.position, Vector3.forward, out rHit, Mathf.Infinity, groundCheckLM))
        {
            currGroundType = rHit.collider.gameObject.GetComponent<GroundTypeDummy>().groundType;
        }
        // IMPORTANT FOR BODY
        // Debug.DrawRay(transform.position,  Vector3.Reflect( new Vector3(rigidB.velocity.x, rigidB.velocity.y, 1), (transform.up)), Color.blue);
        // Debug.DrawRay(transform.position, new Vector3(rigidB.velocity.x, rigidB.velocity.y, 0), Color.red);

    }

    // Update is called once per frame
    void FixedUpdate () {
        // apply groundtype attributes to car values
        float fDriveForce = driveForce * currGroundType.speedMult;
        float fMaxVelocity = maxVelocity * currGroundType.speedMult;

        float fTorqueForce = torqueForce * currGroundType.slideMult;
        float fmaxTorqueForce = maxTorqueForce * currGroundType.slideMult;

        // a value which is greater than 0 if we are moving forward, and less than 0 if we are moving backwards
        float forwardCheck = Vector3.Dot(rigidB.velocity.normalized, transform.up);
        float forwAbs = Mathf.Abs(forwardCheck);

        // determine whether we are drifting or not
        if (forwAbs < slideThreshold) // 0 is when it is drifting as most
        {
            // we are drifting
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }


        float inputVer = Input.GetAxis("Vertical");
        float inputHor = Input.GetAxis("Horizontal");

        rigidB.AddForce(transform.up * inputVer * fDriveForce * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.A))
        {
            rigidB.AddTorque(fTorqueForce * Time.fixedDeltaTime, ForceMode2D.Force);

            //rigidB.AddForce(transform.up * inputVer * fDriveForce * 0.4f * Time.fixedDeltaTime); //apply force forward when only turning, TODO: fix so that it works :P

        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidB.AddTorque(-fTorqueForce * Time.fixedDeltaTime, ForceMode2D.Force);

            //rigidB.AddForce(transform.up * inputVer * fDriveForce * 0.4f * Time.fixedDeltaTime); //så att du inte kan stå still o rotera
        }

        // limit speed 

        // TODO LOOK OVER EVERYTHING ON BREAKING
        // between 0 and 1, 1 if the velocity is equal to the fMaxVelocity
        float velNor = (rigidB.velocity.magnitude) / (fMaxVelocity);
        // break more depending on how fast we are going
        float velEx = velNor * 0.5f;
        // break more depending on how much we are sliding
        float velAngEx = Mathf.Min(forwAbs, slideThreshold); // prevent it from being too great

        //float velEx = Mathf.Min((1 + velNor), 2); // when the speed is double of our fMaxVelocity - we increase the breaking by double the amount
        // limit velocity depending on how fast we are moving
        rigidB.velocity = Vector3.Lerp(rigidB.velocity, Vector3.zero, Time.fixedDeltaTime * breakVelocityForce * velEx);

        if (!isSliding) // TODO might not be the smartest?
        {
            // limit rotation
            if (rigidB.angularVelocity > fmaxTorqueForce || rigidB.angularVelocity < -fmaxTorqueForce)
            {
                rigidB.angularVelocity = Mathf.Lerp(rigidB.angularVelocity, 0, Time.fixedDeltaTime * breakVelocityForce * velEx * velAngEx); // reduce angular velocity, make angular velocity also dissappear faster at faster speeds
            }
        }


        // redirect the velocity so that we dont get a drifting car
        // if we are not drifting - then the wheels no longer have control, so we dont want to redirect them

        if (forwardCheck > 0.0f) // going forward
        {
            rigidB.velocity = Vector3.RotateTowards(rigidB.velocity, transform.up, Time.fixedDeltaTime * redirectVelocitySpeed * forwAbs, 0.0f); // use forwAbs to redirect the velocity MORE if we are moving in the wheels direction
        }
        else // going backward
        {
            rigidB.velocity = Vector3.RotateTowards(rigidB.velocity, transform.up * -1, Time.fixedDeltaTime * redirectVelocitySpeed * forwAbs, 0.0f);
        }



        // ** JUICING **

        // rotate the wheels visually
        float rotValue = Mathf.Max(Mathf.Min((rigidB.angularVelocity / fmaxTorqueForce), 1), -1); // make it no higher/smaller than 1/-1
        float rotWheelTurnValue = rotValue * maxWheelRotAngle;
        for (int i = 0; i < rotateWheels.Length; i++)
        {
            Quaternion wantedRot = startWheelRotation * Quaternion.Euler(0, -rotWheelTurnValue, 0);
            rotateWheels[i].localRotation = Quaternion.Lerp(rotateWheels[i].localRotation, wantedRot, Time.fixedDeltaTime * wheelRotSpeed);
        }


        // delay on the body (caross)

        // clamping velocity to a max force so that the offset doesnt get too great
        Vector3 clampedVel = new Vector3(rigidB.velocity.x, rigidB.velocity.y, 1).normalized * (rigidB.velocity.magnitude / (fMaxVelocity * 1.5f));
        // move the direction so that it is towards the opposite side of the velocity, as its there we want to move the caross
        Vector3 wantedDir = Vector3.Reflect(clampedVel, (transform.up)) * bodyDistanceMult;        
        Vector3 wantedBodyPos = wantedDir;

        // in order to apply scaling, we must first move the wantedBodyPos to transform space as bodyTranslationMult is in transform space
        wantedBodyPos = transform.InverseTransformDirection(wantedBodyPos);
        // apply scaling aswell to get different strength on veritcal/horizontal movement
        wantedBodyPos = Vector3.Scale(wantedBodyPos, bodyTranslationMult);
        // move it back to world space
        wantedBodyPos = transform.TransformDirection(wantedBodyPos);

        float zValue = body.position.z; // in order to keep the same z-value as before
        body.position = Vector3.Lerp(transform.position, transform.position + wantedBodyPos, Time.fixedDeltaTime * 200);
        body.position = new Vector3(body.position.x, body.position.y, zValue); // dont move in z-axis, as we are in 2D!


        // delay the bodys rotation
        float rotDelayValue = rigidB.angularVelocity;
        body.localRotation = bodyStartRot * Quaternion.Euler(bodyLocalRotationMult.x * rotDelayValue, bodyLocalRotationMult.y * rotDelayValue, bodyLocalRotationMult.z * rotDelayValue);




        lastFrameVelocity = new Vector3(rigidB.velocity.x, rigidB.velocity.y, 0);
        lastFrameTorque = rigidB.angularVelocity;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // we are using lastFrameVelocity because it is safer to use during collision, as the normal velocity might be zero
        //if (lastFrameVelocity.magnitude < (fMaxVelocity * 0.3f))
        //{
        //    return;
        //}

        ContactPoint2D cp = col.contacts[0];
        //Vector2 knockDir = Vector3.Reflect( transform.position - new Vector3( col.contacts[0].point.x, col.contacts[0].point.y, 1), new Vector3(col.contacts[0].normal.x, col.contacts[0].normal.y, 1));
        Vector2 knockDir = Vector2.Reflect(new Vector2(lastFrameVelocity.x, lastFrameVelocity.y), cp.normal); // using last frames velocity since the current velocity is affected after collision
        // calculate the reflect force depending on how similar to the cars velocity it is
        float knockForceMult = Vector2.Dot(knockDir.normalized, lastFrameVelocity.normalized);
        knockForceMult = Mathf.Max(knockForceMult, 0.0f); // TODO maybee do this, do I want negative value?

        //Debug.DrawRay(transform.position, new Vector3(cp.normal.x, cp.normal.y, 0) * 1000, Color.red, 1000);
        //Debug.DrawRay(transform.position, new Vector3(lastFrameVelocity.x, lastFrameVelocity.y, 0) * 1000, Color.green, 1000, false);
        //Debug.DrawRay(transform.position, new Vector3(knockDir.x, knockDir.y, 0) * 1000, Color.blue, 1000);

        //rigidB.velocity = knockDir * knockForceMult;
        //transform.up = Vector2.Lerp(transform.up, knockDir.normalized, knockForceMult);

    }

}
