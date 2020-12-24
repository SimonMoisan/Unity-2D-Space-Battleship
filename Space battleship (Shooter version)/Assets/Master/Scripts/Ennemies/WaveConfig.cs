using UnityEngine;

[CreateAssetMenu(menuName = "Ennemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Ennemies to spawn parameters :")]
    public GameObject[] ennemyPrefabs;
    public int[] ennemiesNumberToSpawn;
    public int totalNbrEnnemy;
    public int dangerIndicator;

    [Header("Wave parameters :")]
    public float spawnRate;
    public float spawnRadomFactor;
    public WavePath[] wavePath;
    public bool dieAtEnd; //Indicate if ennemies died when rush end of the waypath
    public bool pathReversed; //Indicate if the path is inversed or not

    private void OnValidate()
    {
        dangerIndicator = 0;
        for (int i = 0; i < ennemyPrefabs.Length; i++)
        {
            if(ennemyPrefabs[i].GetComponent<Ennemy>() != null)
            {
                dangerIndicator += ennemyPrefabs[i].GetComponent<Ennemy>().dangerIndicator * ennemiesNumberToSpawn[i];
            }
            else if(ennemyPrefabs[i].GetComponent<EnnemySquad>() != null)
            {
                dangerIndicator += ennemyPrefabs[i].GetComponent<EnnemySquad>().dangerIndicator * ennemiesNumberToSpawn[i];
            }
        }
    }

    public Transform[] GetWaypoints()
    {
        int randomWavepath = Random.Range(0, wavePath.Length - 1);
        Transform[] waveWayPoints = wavePath[randomWavepath].points;

        if (wavePath[randomWavepath].dieAtEnd)
        {
            dieAtEnd = true;
        }
        else
        {
            dieAtEnd = false;
        }

        if (pathReversed)
        {
            System.Array.Reverse(waveWayPoints);
        }

        return waveWayPoints;
    }

}

[System.Serializable]
public class EnnemyList
{
    public GameObject[] ennmies;
}
