using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPingPong : MonoBehaviour {
    Color baseColor;
    public Color interpolatingColor;
    public float speed = 1;
    SpriteRenderer rend;

    Text mtext;
	// Use this for initialization
	void Start () {
        rend = GetComponent<SpriteRenderer>();
        mtext = GetComponent<Text>();

        if (rend != null)
        {
            baseColor = rend.color;
        }
        else if(mtext != null)
        {
            baseColor = mtext.color;
        }
	}
	
	// Update is called once per frame
	void Update () {
        var pingPong = Mathf.PingPong(Time.time * speed, 1);
        var color = Color.Lerp(baseColor, interpolatingColor, pingPong);

        if (rend != null)
        {
            rend.color = color;
        }
        else if (mtext != null)
        {
            mtext.color = color;
        }


    }
}
