using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Wave : ScriptableObject
{
    //A wave is composed of several waveConfigs which are launch at the same time
    public WaveConfig[] waveConfigs;
    public int tier; //Indicator of the difficulty of this wave
    public int dangerIndicator;
    public int numberOfEnnmy;

    private void OnValidate()
    {
        numberOfEnnmy = 0;
        dangerIndicator = 0;
        for (int i = 0; i < waveConfigs.Length; i++)
        {
            numberOfEnnmy += waveConfigs[i].totalNbrEnnemy;
            dangerIndicator += waveConfigs[i].dangerIndicator;
        }
    }
}
