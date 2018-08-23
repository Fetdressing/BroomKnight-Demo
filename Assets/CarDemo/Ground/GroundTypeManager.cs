using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New GroundType", menuName = "Groundtype", order = 1)]
public class GroundType : ScriptableObject
{
    public float speedMult = 1.0f;
    public float slideMult = 1.0f;
    public GameObject dustParticleSystem;

    public GroundType(float speedMult = 1.0f, float slideMult = 1.0f, GameObject dustParticleSystem = null)
    {
        this.speedMult = speedMult;
        this.slideMult = slideMult;
        this.dustParticleSystem = dustParticleSystem;
    }

    public void Awake()
    {
        this.speedMult = 1;
        this.slideMult = 1;
        this.dustParticleSystem = null;
    }
}

public class GroundTypeManager : MonoBehaviour {
    [System.NonSerialized]
    public GroundType defaultGroundType;

    private static GroundTypeManager _instance;

    public static GroundTypeManager Instance
    {
        get
        {
            return _instance;
        }
    } 
        

    public void Awake()
    {
        if(this != _instance && _instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        defaultGroundType = ScriptableObject.CreateInstance<GroundType>();
    }
}
