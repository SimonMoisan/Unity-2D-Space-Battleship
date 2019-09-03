using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ennemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject ennemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float spawnRate = 0.5f;
    [SerializeField] float spawnRadomFactor = 0.3f;
    [SerializeField] int numberOfEnnemies = 20;
    [SerializeField] float moveSpeed = 2f;

    public GameObject GetEnnemyPrefab()
    {
        return ennemyPrefab;
    }

    public List<Transform> GetWaypoints()
    {
        List<Transform> waveWayPoints = new List<Transform>();
        foreach(Transform child in pathPrefab.transform)
        {
            waveWayPoints.Add(child);
        }

        return waveWayPoints;
    }

    public float GetSpawnRate()
    {
        return spawnRate;
    }

    public float GetSpawnRadomFactor()
    {
        return spawnRadomFactor;
    }

    public int GetNumberOfEnnemies()
    {
        return numberOfEnnemies;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

}
