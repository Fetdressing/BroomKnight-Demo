using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAlphaFade : MonoBehaviour {
    private Image image;
    private Color startColor;
    public float startAlpha = 1;

    public void Fade()
    {
        image = GetComponent<Image>();
        startColor = image.color;

        StartCoroutine(StartFade());
    }

    IEnumerator StartFade()
    {
        float alphaVal = startAlpha;
        while (image.color.a > 0.0f)
        {
            alphaVal -= Time.deltaTime * 0.3f;
            image.color = new Color(startColor.r, startColor.g, startColor.b, alphaVal);
            yield return new WaitForEndOfFrame();
        }
        
    }
}
