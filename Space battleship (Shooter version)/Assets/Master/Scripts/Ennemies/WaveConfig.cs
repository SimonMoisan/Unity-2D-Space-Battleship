using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ennemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Ennemies to spawn parameters :")]
    public GameObject[] ennemyPrefabs;
    public int[] ennemiesNumberToSpawn;

    [Header("Wave parameters :")]
    public float spawnRate;
    public float spawnRadomFactor;
    public GameObject pathPrefab;
    public bool dieAtEnd; //Indicate if ennemies died when rush end of the waypath
    public bool pathReversed; //Indicate if the path is inversed or not

    public List<Transform> GetWaypoints()
    {
        List<Transform> waveWayPoints = new List<Transform>();

        if (pathReversed)
        {
            waveWayPoints.Reverse();
        }

        
        foreach(Transform child in pathPrefab.transform)
        {
            waveWayPoints.Add(child);
        }

        return waveWayPoints;
    }
}
