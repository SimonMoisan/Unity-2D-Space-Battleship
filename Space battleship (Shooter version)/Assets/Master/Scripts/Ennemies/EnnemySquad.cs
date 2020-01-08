using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySquad : MonoBehaviour
{
    public Ennemy[] ennemies;
    public int ennemyAlive;
    [SerializeField] public WaveConfig waveConfig;

    //Configuration parameters
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2f;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        ennemies = GetComponentsInChildren<Ennemy>();

        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    private void Update()
    {
        MoveCible();
    }

    public void imDestroyed()
    {
        if(ennemyAlive > 0)
        {
            ennemyAlive--;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void MoveCible()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            if (waveConfig.dieAtEnd)
            {
                EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
                ennemySpawner.ennemDestroyed += ennemyAlive;
                Destroy(gameObject);
            }
        }
    }
}
