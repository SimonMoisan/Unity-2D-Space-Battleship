using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCible : Ennemy
{
    //Configuration parameters
    [SerializeField] WaveConfig waveconfig;
    [SerializeField] Transform[] waypoints;
    [SerializeField] float moveSpeed = 2f;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        MaxHullPoints = 100;
        hullPoints = 100;
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCible();
    }

    private void MoveCible()
    {
        if (waypointIndex <= waypoints.Length - 1)
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
            Destroy(gameObject);
        }
    }
}
