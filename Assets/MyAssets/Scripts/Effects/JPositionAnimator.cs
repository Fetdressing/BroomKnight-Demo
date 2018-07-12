using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPositionAnimator : JAnimator {

    public Vector3 startPos;
    public Vector3 endPos;
    public AnimationCurve animCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public float length;
    Coroutine animRout;

    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(Animation());
        }
    }

    public override void StartAnimation()
    {
        animRout = StartCoroutine(Animation());
    }

    public override void StopAnimation()
    {
        if (animRout != null)
        {
            StopCoroutine(animRout);
        }
    }

    IEnumerator Animation()
    {
        yield return new WaitForSeconds(delay);

        var timer = 0.0f;
        while (timer < length)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, animCurve.Evaluate(timer / length));
            yield return null;
            timer += Time.deltaTime;
        }
        transform.localPosition = Vector3.Lerp(startPos, endPos, animCurve.Evaluate(1.0f));
    }
}
