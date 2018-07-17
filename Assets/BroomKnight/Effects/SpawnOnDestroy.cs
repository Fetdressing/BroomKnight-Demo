using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour {
    public GameObject toSpawn;
	
	// Update is called once per frame
	void OnDisable()
    {
        Instantiate(toSpawn, transform.position, transform.rotation);
    }
}
