using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JScaleAnimator : JAnimator {

    public float startScaleFactor = 1.0f;
    public float endScaleFactor = 0.0f;
    public float length;
    Coroutine corAnim;

    Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
        if (playOnStart)
        {

        }
    }

    public override void StartAnimation()
    {
        corAnim = StartCoroutine(Animate());
    }

    public override void StopAnimation()
    {
        if (corAnim != null)
        {
            StopCoroutine(corAnim);
        }
        transform.localScale = startScale;
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(delay);

        startScale = transform.localScale * startScaleFactor;
        var endScale = transform.localScale * endScaleFactor;

        var timer = 0.0f; 
        while (timer < length)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / length);
            yield return null;
            timer += Time.deltaTime;
        }
        transform.localScale = endScale;
    }
}
