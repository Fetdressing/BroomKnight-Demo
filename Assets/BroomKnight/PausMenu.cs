using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(this.enabled == false)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }
}
