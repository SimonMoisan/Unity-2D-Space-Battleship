using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperScout : Ennemy
{
    //Configuration parameters
    [SerializeField] WaveConfig waveConfig;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2f;
    int waypointIndex = 0;

    public Coroutine takingConstantDamages;

    // Start is called before the first frame update
    void Start()
    {
        MaxHullPoints = 100;
        hullPoints = 100;
        bulletSpeed = 400f;
        fireRate = 0.2f;
        nbrShots = 1;
        cooldown = 4.0f;
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Targeter();
        Fire("Normal");
        MoveCible();
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
            Destroy(gameObject);
        }
    }
}
