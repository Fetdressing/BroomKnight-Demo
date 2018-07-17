using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    protected int m_currSpawnIndex = 0;

    protected Transform[] m_spawnPositions;

    protected IEnumerator m_spawn;
    // Use this for initialization
    void Awake () {
        m_spawnPositions = GetComponentsInChildren<Transform>();
        Transform[] temp = new Transform[m_spawnPositions.Length - 1];
        for(int i = 1; i < m_spawnPositions.Length; i++)
        {
            temp[i-1] = m_spawnPositions[i];
        }
        m_spawnPositions = temp; // self has been removed from list
        
	}

    public void SendWave(Wave wave)
    {
        if(m_spawn != null)
        {
            StopCoroutine(m_spawn);
            m_spawn = null;
        }

        m_spawn = M_Spawn(wave);
        StartCoroutine(m_spawn);
    }

    IEnumerator M_Spawn(Wave wave)
    {
        int waveUnitIndex = 0;
        yield return new WaitForSeconds(wave.m_beginTime);
        while(true)
        {
            WaveUnit unit = wave.m_units[waveUnitIndex];
            yield return new WaitForSeconds(unit.m_spawnDelay);

            var o = Instantiate(unit.m_enemy.m_obj.gameObject, M_GetSpawnPosition(unit.m_spawnPosType), Quaternion.identity);
            //o.transform.rotation = M_GetSpawnRotation(unit.m_spawnPosType);

            waveUnitIndex++;
            if(waveUnitIndex >= wave.m_units.Length)
            {
                waveUnitIndex = 0;
            }
        }
    }

    protected Vector3 M_GetSpawnPosition(SpawnPosType type)
    {
        Vector3 returnPos = Vector3.zero;
        switch(type)
        {
            case SpawnPosType.Iterating:
                returnPos = m_spawnPositions[m_currSpawnIndex].position;
                m_currSpawnIndex++;
                if (m_currSpawnIndex >= m_spawnPositions.Length)
                {
                    m_currSpawnIndex = 0;
                }
                break;
            case SpawnPosType.Random:
                returnPos = m_spawnPositions[Random.Range(0, m_spawnPositions.Length)].position;
                break;
            default:
                break;
        }

        return returnPos;
    }
}
