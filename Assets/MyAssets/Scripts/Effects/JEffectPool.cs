using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JEffectPool : JPool {

    const int myDefaultPoolSize = 300;

    static JEffectPool m_instance;

    public static JEffectPool Instance
    {
        get
        {
            if (m_instance)
            {
                return m_instance;
            }
            else
            {
                var obj = new GameObject("JEffectPool");
                m_instance = obj.AddComponent<JEffectPool>();

                return m_instance;
            }
        }
    }

    Dictionary<GameObject, Dictionary<int, JEmitter>> m_particleEmitters = new Dictionary<GameObject, Dictionary<int, JEmitter>>();


    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
#if UNITY_EDITOR
        else
        {
            print("Multiple instances of " + name + " created, destroying object.");
        }
#endif
    }

    private new void OnDestroy()
    {
        if (m_instance == this)
        {
            base.OnDestroy();
            m_particleEmitters.Clear();
            m_instance = null;
        }
    }


    public static void CreatePool(GameObject prefab, int customSize = myDefaultPoolSize)
    {
        Instance.CreatePoolInt(prefab, customSize);
    }

    public static GameObject CreateEffect(GameObject prefab)
    {
        return Instance.CreateInstanceInt(prefab);
    }

    public static GameObject CreateEffect(GameObject prefab, Vector3 position, Quaternion orientation, float scaleMultiplier = 1.0f)
    {
        return Instance.CreateInstanceInt(prefab, position, orientation, scaleMultiplier);
    }

    public static GameObject CreateEffect(GameObject prefab, Vector3 position, Quaternion orientation)
    {
        return Instance.CreateInstanceInt(prefab, position, orientation);
    }

    static int FloatToIntHash(float val)
    {
        return (int)(val * 100.0f);
    }

    public static void RegisterEmitter(GameObject emitterPrefab, float scale)
    {
        Instance.RegisterEmitterInt(emitterPrefab, scale);
    }

    void RegisterEmitterInt(GameObject emitterPrefab, float scale)
    {
        Dictionary<int, JEmitter> outDict;

        if (m_particleEmitters.TryGetValue(emitterPrefab, out outDict))
        {
            int hash = FloatToIntHash(scale);

            // Ignore if we already have an emitter
            if (outDict.ContainsKey(hash))
            {
                return;
            }

            // Create particle emitter
            var obj = Instantiate(emitterPrefab);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            var sys = obj.GetComponent<JEmitter>();
#if UNITY_EDITOR
            if (sys == null)
            {
                print("Emitter registerd without JEmitter component, discarding");
            }
#endif

            // Add emitter to dict
            outDict.Add(hash, sys);
        }
        else
        {
            // Create particle emitter
            var obj = Instantiate(emitterPrefab);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            var sys = obj.GetComponent<JEmitter>();

#if UNITY_EDITOR
            if (sys == null)
            {
                print("Emitter registerd without JEmitter component, discarding");
            }
#endif

            // Create dict for all emitter of this type
            var newDict = new Dictionary<int, JEmitter>();
            newDict.Add(FloatToIntHash(scale), sys);
            m_particleEmitters.Add(emitterPrefab, newDict);
        }
    }

    public static void Emit(GameObject emitterPrefab, Vector3 position, float scale, float emitAmountFactor)
    {
        Instance.EmitInt(emitterPrefab, position, scale, emitAmountFactor);
    }

    public void EmitInt(GameObject emitterPrefab, Vector3 position, float scale, float emitAmountFactor)
    {
        Dictionary<int, JEmitter> outDict;
        if (m_particleEmitters.TryGetValue(emitterPrefab, out outDict))
        {
            JEmitter emitter;
            if (outDict.TryGetValue(FloatToIntHash(scale), out emitter))
            {
                emitter.transform.position = position;
                emitter.Emit(emitAmountFactor);
            }
            else
            {
                RegisterEmitter(emitterPrefab, scale);
                Emit(emitterPrefab, position, scale, emitAmountFactor);
            }
        }
        else
        {
            RegisterEmitter(emitterPrefab, scale);
            Emit(emitterPrefab, position, scale, emitAmountFactor);
        }
    }
}
