using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JEmitter : MonoBehaviour {

    [System.Serializable]
    public struct Emitter
    {
        public ParticleSystem emitter;
        public int emitCount;
        public int amount;
        public float frequence;
    }

    public Emitter[] m_emitters;

    public void Emit(float multiplier = 1.0f)
    {
        for (int i = 0; i < m_emitters.Length; i++)
        {
            var emitter = m_emitters[i];
            emitter.emitter.Emit((int)(emitter.emitCount * multiplier));
            if (emitter.amount > 1)
            {
                StartCoroutine(EmitRoutine(i, multiplier));
            }
        }
    }

    IEnumerator EmitRoutine(int index, float multiplier)
    {
        int amount = 1;
        Emitter emitter = m_emitters[index];
        Vector3 emitPos = transform.position;

        while (amount < emitter.amount)
        {
            yield return new WaitForSeconds(emitter.frequence);
            transform.position = emitPos;
            emitter.emitter.Emit((int)(emitter.emitCount * multiplier));
            amount++;
        }

    }
}
