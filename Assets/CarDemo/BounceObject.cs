using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : MonoBehaviour {
    // start localscale of body object
    private Vector3 startBodyScale;
    // the object that is affected by the scale
    public Transform body;
    // at what velocity it will reach its full bounce
    public float maxBounceVel = 150;

    private IEnumerator BounceIE = null;
    public AnimationCurve bounceAnimCurve;
    // how long the cooldown for the bounce is
    private float bounceCD;
    private float bounceCDTimer = 0.0f;
    // Use this for initialization
    void Awake () {
        startBodyScale = body.localScale;
        bounceCD = bounceAnimCurve.keys[bounceAnimCurve.length - 1].time * 0.8f; // a value of how long the scaling takes to finish, so that it can be interrupted just before the ending
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // we are using lastFrameVelocity because it is safer to use during collision, as the normal velocity might be zero
        //if (lastFrameVelocity.magnitude < (maxVelocity * 0.3f))
        //{
        //    return;
        //}

        float magnitudeNor = Mathf.Min(col.relativeVelocity.magnitude / maxBounceVel, 1);

        Bounce(magnitudeNor); //send a magnitude value of how much the car should bump
    }

    void Bounce(float magnitude)
    {
        if (bounceCDTimer > Time.time)
        {
            return;
        }

        bounceCDTimer = Time.time + bounceCD;

        if (BounceIE != null)
        {
            // reset
            StopCoroutine(BounceIE);
            body.localScale = startBodyScale;
        }
        BounceIE = PerformBounce(magnitude);
        StartCoroutine(BounceIE);
    }

    IEnumerator PerformBounce(float magnitude)
    {
        float startTime = Time.time;
        float timeB = bounceAnimCurve.keys[bounceAnimCurve.length - 1].time; // figure out how long the curve is
        float timer = Time.time + timeB;

        while (timer > Time.time)
        {
            float vscale = bounceAnimCurve.Evaluate(Time.time - startTime);
            Vector3 startVec = new Vector3(startBodyScale.x, startBodyScale.y, startBodyScale.z);
            body.localScale = startVec + (startVec * vscale * magnitude);
            yield return new WaitForEndOfFrame();
        }


        body.localScale = startBodyScale;
        BounceIE = null;
    }
}
