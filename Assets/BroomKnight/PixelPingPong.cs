using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPingPong : MonoBehaviour {
    Color baseColor;
    public Color interpolatingColor;
    public float speed = 1;
    SpriteRenderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<SpriteRenderer>();
        baseColor = rend.color;
	}
	
	// Update is called once per frame
	void Update () {
        var pingPong = Mathf.PingPong(Time.time * speed, 1);
        var color = Color.Lerp(baseColor, interpolatingColor, pingPong);
        rend.color = color;
    }
}
