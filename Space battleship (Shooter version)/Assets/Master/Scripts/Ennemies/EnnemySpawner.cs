using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySpawner : MonoBehaviour
{
    public float timeBtwWaves;
    public String sectorState;
    public int waveRunning;

    //Spawn variables 
    public int ennemyCount = 0; //Total ennemy to spawn in the whole sector
    public int ennemySpawned = 0; //Number of ennemy actually spawned
    public int ennemDestroyed = 0; //Total ennemy destroyed by the player

    //Associated objects
    public WaveConfig[] waveConfigs;
    public Wave[] waves;
    public GameManagerScript gameManager;

    // Start is called before the first frame update
    void Start()
    {
        sectorState = "In Progress";
        waveRunning = 0;
        Debug.Log("Sector Battle Phase Started");

        gameManager = FindObjectOfType<GameManagerScript>();

        /*
        //Count number of ennemy to spawn and Give waypoints to ennemyPrefabs in waveConfigs
        for (int i = 0; i < waveConfigs.Length; i++)
        {
            for (int j = 0; j < waveConfigs[i].ennemiesNumberToSpawn.Length; j++)
            {
                if (waveConfigs[i].ennemyPrefabs[j].GetComponent<Ennemy>() != null)
                {
                    waveConfigs[i].ennemyPrefabs[j].GetComponent<Ennemy>().waveConfig = waveConfigs[i];
                }
                else if(waveConfigs[i].ennemyPrefabs[j].GetComponent<EnnemySquad>() != null)
                {
                    waveConfigs[i].ennemyPrefabs[j].GetComponent<EnnemySquad>().waveConfig = waveConfigs[i];
                }

                ennemyCount = waveConfigs[i].ennemiesNumberToSpawn[j];
            }
        }
        */

        StartBattle();
        //StartCoroutine(SpawnLoopWave(1));
        //StartCoroutine(SpawnAllWaves());
    }

    private void StartBattle()
    {
        if (waves != null)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                for (int j = 0; j < waves[i].waveConfigs.Length; j++)
                {
                    for (int k = 0; k < waves[i].waveConfigs[j].ennemiesNumberToSpawn.Length; k++)
                    {
                        if (waves[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<Ennemy>() != null)
                        {
                            waves[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<Ennemy>().waveConfig = waves[i].waveConfigs[j];
                        }
                        else if (waves[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<EnnemySquad>() != null)
                        {
                            waves[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<EnnemySquad>().waveConfig = waves[i].waveConfigs[j];
                        }
                    }
                    //ennemyCount += waves[i].countNumberOfEnnemy();
                }
            }
        }

        StartCoroutine(SpawnWaves());
    }

    //Coroutines used to repeate WaveConfig spawn in a loop
    private IEnumerator SpawnLoopWave(int loopNumber)
    {
        for(int i=0;i<loopNumber;i++)
        {
            yield return StartCoroutine(SpawnWavesConfigs(waveConfigs));
        }
    }

    //Coroutine used to spawn all WaveConfig one by one
    private IEnumerator SpawnWavesConfigs(WaveConfig[] wavesConfigTab)
    {
        for(int i=0; i< wavesConfigTab.Length;i++)
        {
            var currentWave = wavesConfigTab[i];
            yield return StartCoroutine(SpawnAllEnnemiesInWave(currentWave));
        }
    }

    //Coroutine used to spawn all Waves one by one
    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            waveRunning++;
            Debug.Log("Wave " + waveRunning + " running");

            WaveConfig[] currentWaveConfigs = waves[i].waveConfigs;
            yield return StartCoroutine(SpawnAllWavesConfigs(currentWaveConfigs));

            Debug.Log("Wave " + waveRunning + " Finished");
            yield return new WaitForSeconds(timeBtwWaves);
        }
        Debug.Log("Sector Battle Phase Finished");
    }

    //Function used to spawn all waves at the same time
    private IEnumerator SpawnAllWavesConfigs(WaveConfig[] wavesC)
    {
        for (int i = 0; i < wavesC.Length-1; i++)
        {
            var currentWave = wavesC[i];
            StartCoroutine(SpawnAllEnnemiesInWave(currentWave));
        }
        yield return StartCoroutine(SpawnAllEnnemiesInWave(wavesC[wavesC.Length - 1]));
    }

    //Coroutine used to spawn every ennemies in within a WaveConfig
    private IEnumerator SpawnAllEnnemiesInWave(WaveConfig waveConfig)
    {
        for (int i = 0; i < waveConfig.ennemyPrefabs.Length; i++)
        {
            for (int j = 0; j < waveConfig.ennemiesNumberToSpawn[i]; j++)
            {
                GameObject go = Instantiate(waveConfig.ennemyPrefabs[i], waveConfig.GetWaypoints()[0].transform.position, Quaternion.identity);
                if(go.GetComponent<EnnemySquad>() != null)
                {
                    ennemySpawned += go.GetComponent<EnnemySquad>().ennemyAlive;
                }
                else
                {
                    ennemySpawned++;
                }
                
                yield return new WaitForSeconds(waveConfig.spawnRate);
            }
        }
    }
}
