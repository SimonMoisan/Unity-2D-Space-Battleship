using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnnemySpawner : MonoBehaviour
{
    [Header("Sector infos")]
    [ReadOnly] public String sectorState;
    [ReadOnly] public int wavePlayingIndex;

    //Spawn variables 
    [Header("Sector variables")]
    [ReadOnly] public int ennemyCount; //Total ennemy to spawn in the whole sector
    [ReadOnly] public int ennemySpawned; //Number of ennemy actually spawned
    [ReadOnly] public int ennemyDestroyed; //Total ennemy destroyed
    [ReadOnly] public int ennemyAlive; //Total number of ennemy alive on the map

    [Header("Actual wave variables")]
    [ReadOnly] public int ennemyToSpawnInWave;
    [ReadOnly] public int ennemyDestroyedInWave;
    [ReadOnly] public int ennemyAliveInWave; //Number of ennemy remains alive in the actual wave
    [ReadOnly] public int scrapsToWin; //Quantity of scrap to give to the player at sector's end

    //Associated objects
    [Header("Associated objects")]
    [ReadOnly] public List<Wave> wavesToPlay;
    [ReadOnly] private Wave wavePlaying;
    [ReadOnly] public bool waveIsRunning; //Actual wave is running
    [ReadOnly] public GameManagerScript gameManager;
    [ReadOnly] public PlayerStats playerStats;

    //Timers
    [Header("Timers")]
    [ReadOnly] public float timerEndSector;
    [ReadOnly] public float timerStartSector;
    [ReadOnly] public float timeBtwWaves;

    public static EnnemySpawner current;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        playerStats = FindObjectOfType<PlayerStats>();
        current = this;

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

        //StartBattle();
        //StartCoroutine(SpawnLoopWave(1));
        //StartCoroutine(SpawnAllWaves());
    }

    public void Update()
    {
        //Check if battle phase is finished
        if((ennemyCount > 0 && ennemyCount <= ennemyDestroyed) || (waveIsRunning && wavesToPlay.Count == 0))
        {
            if(timerEndSector <= 0)
            {
                //Give scraps to player
                playerStats.scraps += scrapsToWin;
                
                gameManager.scrapRewardDisplayer.text = "Scraps won : " + scrapsToWin;
                gameManager.EndBattleSector();
                scrapsToWin = 0;
            }
            timerEndSector -= Time.deltaTime; 
        }
        
        //Play next wave if the actual one is finished = every ennemies have been destroyed
        if(wavePlaying != null && wavePlaying.numberOfEnnmy == ennemyDestroyedInWave && wavePlayingIndex < wavesToPlay.Count)
        {
            wavePlayingIndex++;

            //Reset actual wave variable
            if(wavePlayingIndex < wavesToPlay.Count)
            {
                wavePlaying = wavesToPlay[wavePlayingIndex];
                ennemyToSpawnInWave = wavePlaying.numberOfEnnmy;
                ennemyAliveInWave = ennemyToSpawnInWave;
                ennemyDestroyedInWave = 0;

                //Start next wave
                StartCoroutine(SpawnWave(wavePlayingIndex));
            }  
        }
    }

    public void StartBattle()
    {
        sectorState = "In Progress";

        //Initialise first wave variables
        wavePlayingIndex = 0;
        wavePlaying = wavesToPlay[wavePlayingIndex];
        ennemyToSpawnInWave = wavePlaying.numberOfEnnmy;
        ennemyAliveInWave = ennemyToSpawnInWave;
        ennemyDestroyedInWave = 0;

        //Debug.Log("Sector Battle Phase Started");

        //Attribute waveconfigs to ennemy/squad prefabs and count total number of ennmies in this battle sector
        if (wavesToPlay != null)
        {
            for (int i = 0; i < wavesToPlay.Count; i++)
            {
                for (int j = 0; j < wavesToPlay[i].waveConfigs.Length; j++)
                {
                    for (int k = 0; k < wavesToPlay[i].waveConfigs[j].ennemiesNumberToSpawn.Length; k++)
                    {
                        if (wavesToPlay[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<Ennemy>() != null)
                        {
                            wavesToPlay[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<Ennemy>().waveConfig = wavesToPlay[i].waveConfigs[j];
                        }
                        else if (wavesToPlay[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<EnnemySquad>() != null)
                        {
                            wavesToPlay[i].waveConfigs[j].ennemyPrefabs[k].GetComponent<EnnemySquad>().waveConfig = wavesToPlay[i].waveConfigs[j];
                        }
                    }
                }
                ennemyCount += wavesToPlay[i].numberOfEnnmy;
            }
        }

        //StartCoroutine(SpawnWaves());
        StartCoroutine(SpawnWave(wavePlayingIndex));
    }

    public event Action onEnnemyDestroyed;
    //Function called when an ennemy is destroyed
    public void EnnemyDestroyed()
    {
        /*if(onEnnemyDestroyed != null)
        {
            ennemyDestroyed++;
            ennemyDestroyedInWave++;
            ennemyAliveInWave--;
        }*/
        ennemyDestroyed++;
        ennemyDestroyedInWave++;
        ennemyAliveInWave--;
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

    //Coroutine used to spawn all Waves one by one with a fixed time between waves
    private IEnumerator SpawnWaves()
    {
        ennemyAliveInWave = 0;
        for (int i = 0; i < wavesToPlay.Count; i++)
        {
            waveIsRunning = true;
            for (int j = 0; j < wavesToPlay[i].waveConfigs.Length; j++)
            {
                ennemyAliveInWave += wavesToPlay[i].waveConfigs[j].totalNbrEnnemy;
            }
            //Debug.Log("Wave " + waveRunning + " running");

            WaveConfig[] currentWaveConfigs = wavesToPlay[i].waveConfigs;
            yield return StartCoroutine(SpawnAllWavesConfigs(currentWaveConfigs));

            yield return new WaitForSeconds(timeBtwWaves);
        }
        //Debug.Log("Sector Battle Phase Finished");
    }

    //Spawn on wave, the next wave will spawn when all ennemies are destroyed
    private IEnumerator SpawnWave(int waveIndex)
    {
        //Small time before wave spawn
        yield return new WaitForSeconds(timeBtwWaves);

        waveIsRunning = true;
        for (int j = 0; j < wavePlaying.waveConfigs.Length; j++)
        {
            ennemyAliveInWave += wavePlaying.waveConfigs[j].totalNbrEnnemy;
        }

        //Debug.Log("Wave " + waveRunning + " running");

        WaveConfig[] currentWaveConfigs = wavePlaying.waveConfigs;
        yield return StartCoroutine(SpawnAllWavesConfigs(currentWaveConfigs));
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
                    ennemySpawned += go.GetComponent<EnnemySquad>().ennemies.Length;
                }
                else if(go.GetComponent<Ennemy>() != null)
                {
                    ennemySpawned++;
                }
                
                yield return new WaitForSeconds(waveConfig.spawnRate);
            }
        }
    }
}
