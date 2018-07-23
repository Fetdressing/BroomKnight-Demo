using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringUI : MonoBehaviour {
    RectTransform rTransform;
    Vector3 startPos;

    public float height = 1.0f;
    public float speedmult = 1.0f;
	// Use this for initialization
	void Start () {
        rTransform = transform.GetComponent<RectTransform>();
        startPos = rTransform.position;
    }
	
	// Update is called once per frame
	void Update () {
        rTransform.position = new Vector3(startPos.x, startPos.y + Mathf.PingPong(Time.unscaledTime * speedmult, 1.0f * height), startPos.z);
	}
}
