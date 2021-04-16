using UnityEngine;
using System.Text.RegularExpressions;

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
    public string[] wavePathNames;
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

        totalNbrEnnemy = 0;
        for (int i = 0; i < ennemyPrefabs.Length; i++)
        {
            if (ennemyPrefabs[i].GetComponent<Ennemy>() != null)
            {
                totalNbrEnnemy += ennemiesNumberToSpawn[i];
            }
            else if (ennemyPrefabs[i].GetComponent<EnnemySquad>() != null)
            {
                totalNbrEnnemy += ennemyPrefabs[i].GetComponent<EnnemySquad>().ennemies.Length * ennemiesNumberToSpawn[i];
            }
        }
    }

    public Transform[] getWaypoints()
    {
        int randomWavepath = Random.Range(0, wavePathNames.Length - 1);
        Transform wavepathSelected = EnnemySpawner.current.wavePaths[randomWavepath];

        Transform[] waypoints = new Transform[wavepathSelected.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = wavepathSelected.GetChild(i);
        }

        if (Regex.IsMatch(wavepathSelected.name, "End", RegexOptions.IgnoreCase))
        {
            dieAtEnd = true;
        }
        else
        {
            dieAtEnd = false;
        }

        if (pathReversed)
        {
            System.Array.Reverse(waypoints);
        }

        return waypoints;
    }

}

[System.Serializable]
public class EnnemyList
{
    public GameObject[] ennmies;
}
