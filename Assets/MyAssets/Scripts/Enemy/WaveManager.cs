using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPosType { Random, Iterating, Vertical, Horizontal };

[System.Serializable]
public class WaveUnit
{
    public float m_spawnDelay;
    public Enemy m_enemy;
    public SpawnPosType m_spawnPosType;
}

[System.Serializable]
public class Wave
{
    public string m_waveName = "Basic wave";
    public float m_beginTime = 3;
    public float m_duration = 20;
    public WaveUnit[] m_units;
}

public class WaveManager : MonoBehaviour
{
    public Wave[] m_waves;
    protected int m_waveIndex = 0;

    protected Spawner m_spawner;

    public float m_startTime = 4.0f;

    void Awake()
    {
        m_spawner = GetComponentInChildren<Spawner>();

        StartCoroutine(M_RunWaves());
    }

    IEnumerator M_RunWaves()
    {
        yield return new WaitForSeconds(m_startTime); //start time
        while(true)
        {
            m_spawner.SendWave(m_waves[m_waveIndex]);
            yield return new WaitForSeconds(m_waves[m_waveIndex].m_duration + m_waves[m_waveIndex].m_beginTime);

            m_waveIndex++;
            if(m_waveIndex >= m_waves.Length)
            {
                m_waveIndex = 0;
            }
        }
    }
}
