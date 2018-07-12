using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPool : MonoBehaviour {

    Dictionary<GameObject, GameObject[]> m_pools = new Dictionary<GameObject, GameObject[]>();

    const int defaultPoolSize = 100;

    protected void OnDestroy()
    {
        m_pools.Clear();
    }

    protected void CreatePoolInt(GameObject prefab, int customSize = defaultPoolSize)
    {
        // Create pool if it doesn't exist
        if (m_pools.ContainsKey(prefab) == false)
        {
            var mArray = new GameObject[customSize];
            m_pools[prefab] = mArray;
            for (int i = 0; i < customSize; i++)
            {
                mArray[i] = Instantiate(prefab, transform);
                mArray[i].SetActive(false);
            }
        }
#if UNITY_EDITOR
        else
        {
            print("Trying to create second pool for object:" + prefab.name);
        }
#endif
    }

    protected void ExtendPoolInt(GameObject prefab, float sizeMultiplier = 2) //make a pool bigger
    {
        // Create new pool and save old ref
        var oldPool = m_pools[prefab];
        int newSize = (int)(oldPool.Length * sizeMultiplier);
        var newPool = new GameObject[newSize];

        //  Copy old instances ref to new pool
        int len = oldPool.Length;
        int index;
        for (index = 0; index < len; index++)
        {
            newPool[index] = oldPool[index];
        }

        // Create new instances for left spaces of the pool
        for (; index < newSize; index++)
        {
            newPool[index] = Instantiate(prefab, transform);
            newPool[index].SetActive(false);
        }

        m_pools[prefab] = newPool;
    }

    protected GameObject CreateInstanceInt(GameObject prefab)
    {
        if (m_pools.ContainsKey(prefab) == false)
        {
            print("Object " + prefab.name + " was called to be instanced without pool, pool is created in updates.");
            CreatePoolInt(prefab);
        }

        // Try find inactive object
        var myPool = m_pools[prefab];
        int len = myPool.Length;
        for (int i = 0; i < len; i++)
        {
            if (myPool[i] == null)
            {
                print("Object instance for " + prefab.name + " in pool has been destroyed.");
                continue;
            }

            if (!myPool[i].activeInHierarchy)
            {
                GameObject currObj = myPool[i];
                currObj.SetActive(true);
                return currObj;
            }
        }

        // If pool is full
        print("Pool for Object " + prefab.name + " was full and the pool is being extended.");
        ExtendPoolInt(prefab, 2);
        GameObject secObj = CreateInstanceInt(prefab); //try again with new bigger pool

        if (secObj == null)
        {
            print("Something went wrong trying to create a new bigger object pool for " + prefab.name);
        }

        return secObj;
    }

    protected GameObject CreateInstanceInt(GameObject prefab, Vector3 position)
    {
        var obj = CreateInstanceInt(prefab);
        obj.transform.position = position;
        return obj;
    }

    protected GameObject CreateInstanceInt(GameObject prefab, Vector3 position, Quaternion orientation)
    {
        var obj = CreateInstanceInt(prefab);
        obj.transform.position = position;
        obj.transform.rotation = orientation;
        return obj;
    }

    protected GameObject CreateInstanceInt(GameObject prefab, Vector3 position, Quaternion orientation, float localScale = 1.0f)
    {
        var obj = CreateInstanceInt(prefab);
        obj.transform.position = position;
        obj.transform.rotation = orientation;
        obj.transform.localScale = new Vector3(localScale, localScale, localScale);
        return obj;
    }
}
