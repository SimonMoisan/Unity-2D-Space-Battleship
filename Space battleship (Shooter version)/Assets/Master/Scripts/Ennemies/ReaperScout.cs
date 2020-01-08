using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperScout : Ennemy
{
    //Configuration parameters
    public List<Transform> waypoints;
    public float moveSpeed;
    int waypointIndex = 0;

    public Coroutine takingConstantDamages;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = waveConfig.GetWaypoints();
        squad = GetComponentInParent<EnnemySquad>();

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null)
        {
            transform.position = waypoints[waypointIndex].transform.position;
        }
        squad = GetComponentInParent<EnnemySquad>();
    }

    // Update is called once per frame
    void Update()
    {
        Targeter();
        Fire("Normal");

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null)
        {
            MoveCible();
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
                ennemySpawner.ennemDestroyed++;
                Destroy(gameObject);
            }
        }
    }
}
