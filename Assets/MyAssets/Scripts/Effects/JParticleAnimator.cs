using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JParticleAnimator : JAnimator {



    private void Start()
    {
        if (playOnStart)
        {
            StartAnimation();
        }
    }

    public override void StartAnimation()
    {
        AnimationDelayStart();
    }

    public override void StopAnimation()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator AnimationDelayStart()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().SetTrigger("Start");
    }


}
