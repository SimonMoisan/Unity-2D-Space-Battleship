using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs;
    int startingWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnLoopWave(3));
        //StartCoroutine(SpawnAllWaves());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnLoopWave(int loopNumber)
    {
        for(int i=0;i<loopNumber;i++)
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
    }

    private IEnumerator SpawnAllWaves()
    {
        for(int i=startingWave; i<waveConfigs.Count;i++)
        {
            var currentWave = waveConfigs[i];
            yield return StartCoroutine(SpawnAllEnnemiesInWave(currentWave));
        }
    }

    private IEnumerator SpawnAllEnnemiesInWave(WaveConfig waveConfig)
    {
        for(int i=0;i<waveConfig.GetNumberOfEnnemies();i++)
        {
            Instantiate(
                waveConfig.GetEnnemyPrefab(),
                waveConfig.GetWaypoints()[0].transform.position,
                Quaternion.identity);
            yield return new WaitForSeconds(waveConfig.GetSpawnRate());
        }
        
    }

}
