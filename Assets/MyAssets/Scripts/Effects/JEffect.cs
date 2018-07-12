using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JEffect : MonoBehaviour {

    public float destroyAfterSeconds = 10.0f;

    [Header("ParticleSystem")]
    public bool isParticleSystem = false;
    public bool playOnEnable = true;

    IEnumerator destroyRoutine = null;
    void OnEnable()
    {
        destroyRoutine = Return(destroyAfterSeconds);
        StartCoroutine(destroyRoutine);


        if (isParticleSystem && playOnEnable)
        {
            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (var item in systems)
            {
                item.Play();
            }
        }
    }

    private void OnDisable()
    {
        if (destroyRoutine != null)
        {
            StopCoroutine(destroyRoutine);
        }
    }

    IEnumerator Return(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        var sys = GetComponentInChildren<ParticleSystem>();
        if (sys)
        {
            isParticleSystem = true;
        }
        else
        {
            isParticleSystem = false;
        }
    }
}
