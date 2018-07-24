using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider col) {
        Control c = col.transform.root.GetComponentInChildren<Control>();
        if(c != null)
        {
            c.CC();
        }
	}
}
