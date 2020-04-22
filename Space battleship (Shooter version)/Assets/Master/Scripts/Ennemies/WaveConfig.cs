using UnityEngine;

[CreateAssetMenu(menuName = "Ennemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Ennemies to spawn parameters :")]
    public GameObject[] ennemyPrefabs;
    public int[] ennemiesNumberToSpawn;
    public int totalNbrEnnemy = 0;

    [Header("Wave parameters :")]
    public float spawnRate;
    public float spawnRadomFactor;
    public WavePath[] wavePath;
    public bool dieAtEnd; //Indicate if ennemies died when rush end of the waypath
    public bool pathReversed; //Indicate if the path is inversed or not

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
