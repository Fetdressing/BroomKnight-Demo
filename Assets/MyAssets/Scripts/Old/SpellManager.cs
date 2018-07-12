using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellManager : MonoBehaviour {
    public Transform[] spawnPositions;

    public GameObject projectile;
    public int maxNr = 5;

    List<GameObject> activeProjectiles = new List<GameObject>();
	// Use this for initialization
	void Start () {

        InstantiateNewProjectiles(maxNr);
	}
	
	// Update is called once per frame
	void Update () {
        //int toCreateNr = GetNeededNrProjectiles();
        //InstantiateNewProjectiles(toCreateNr);
	}

    int GetNeededNrProjectiles() //kolla hur många fler projektiler som behöver skapas
    {
        int nrProjs = maxNr - activeProjectiles.Count;
        return nrProjs;
    }

    void InstantiateNewProjectiles(int nrProjs)
    {
        for (int i = 0; i < maxNr; i++)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPositions.Length);

            GameObject gTemp = Instantiate(projectile.gameObject, spawnPositions[spawnIndex]);
            activeProjectiles.Add(gTemp);
        }
    }

    public void DestroyObject(GameObject o)
    {
        activeProjectiles.Remove(o);
    }
}

struct Projectile
{
    public GameObject projectile;
    public int maxNr;
}
