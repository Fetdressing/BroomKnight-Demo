using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionChildren : MonoBehaviour {
    private Rigidbody[] rigidbodies;
    private Vector3[] startPos;
    private Quaternion[] startRot;
    public float explosionForce = 60;

    private static Transform player;
	// Use this for initialization
    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        startPos = new Vector3[rigidbodies.Length];
        startRot = new Quaternion[rigidbodies.Length];

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            startPos[i] = rigidbodies[i].transform.localPosition;
            startRot[i] = rigidbodies[i].transform.localRotation;
        }
    }

	public void Explode (Vector3 dir) {
        

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        for(int i = 0; i < rigidbodies.Length; i++)
        {
            //Vector3 vec = (rigidbodies[i].transform.position - player.position).normalized;
            rigidbodies[i].transform.localPosition = startPos[i];
            //rigidbodies[i].transform.localRotation = startRot[i];
            rigidbodies[i].transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-180, 180));

            Vector3 vec = dir;
            float rFloat = Random.Range(10, 40);
            Vector3 randomVec = new Vector3(rFloat, rFloat, 0);
            vec += randomVec;
            rigidbodies[i].velocity = vec;
            
        }
	}

}
