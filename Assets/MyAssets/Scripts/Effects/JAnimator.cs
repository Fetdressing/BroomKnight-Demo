using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JAnimator : MonoBehaviour {
    public bool playOnStart = true;
    public float delay = 0.0f;

    public abstract void StartAnimation();
    public abstract void StopAnimation();

}
