using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    //A wave is composed of several waveConfigs which are launch at the same time
    public WaveConfig[] waveConfigs;
    public int level; //Indicator of the difficulty of this wave
    public int numberOfEnnmy;

    private void OnValidate()
    {
        numberOfEnnmy = 0;
        for (int i = 0; i < waveConfigs.Length; i++)
        {
            numberOfEnnmy += waveConfigs[i].totalNbrEnnemy;
        }
    }
}
