using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    //A wave is composed of several waveConfigs which are launch at the same time
    public WaveConfig[] waveConfigs;
    public int totalEnnemies;

    // Update is called once per frame
    void Start()
    {

    }

    public int countNumberOfEnnemy()
    {
        for (int i = 0; i < waveConfigs.Length; i++)
        {
            for (int j = 0; j < waveConfigs.Length; j++)
            {
                totalEnnemies += waveConfigs[i].ennemiesNumberToSpawn[j];
            }
        }
        return totalEnnemies;
    }
}
