using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using Pathfinding.Util;

public class EnnemySquad : MonoBehaviour
{
    public Ennemy[] ennemies;
    public int dangerIndicator;
    public int ennemyAlive;
    public WaveConfig waveConfig;
    public string wavePathName;
    public bool isDestroyed;

    //Configuration parameters
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public int waypointIndex;
    public float distance;

    public AIDestinationSetter destinationSetter;
    public AIPath aIPath;

    private void OnValidate()
    {
        waypointIndex = 1;

        dangerIndicator = 0;
        ennemies = GetComponentsInChildren<Ennemy>();
        ennemyAlive = ennemies.Length;
        for (int i = 0; i < ennemies.Length; i++)
        {
            dangerIndicator += ennemies[i].dangerIndicator;
        }

        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
    }


    // Start is called before the first frame update
    void Start()
    {
        destinationSetter.target = waypoints[waypointIndex].transform;
        //initiateWayPathBehavior();
    }

    private void Update()
    {
        MoveCible();
    }

    public IEnumerator imDestroyed()
    {
        ennemyAlive--;
        if (ennemyAlive <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            yield return new WaitForSeconds(0.05f);
            Destroy();
        }
    }

    private void MoveCible()
    {
        distance = Vector2.Distance(transform.position, waypoints[waypointIndex].position);

        if (distance < 1)
        {
            waypointIndex++;

            if (waypointIndex < waypoints.Length)
            {
                destinationSetter.target = waypoints[waypointIndex].transform;
            }
            else
            {
                if (waveConfig.dieAtEnd)
                {
                    EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
                    for (int i = 0; i < ennemyAlive; i++)
                    {
                        ennemySpawner.EnnemyDestroyed();
                    }
                    Destroy();
                }
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
