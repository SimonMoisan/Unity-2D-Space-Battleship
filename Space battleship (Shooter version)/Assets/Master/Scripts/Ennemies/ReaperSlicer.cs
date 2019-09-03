using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperSlicer : Ennemy
{
    //Configuration parameters
    [SerializeField] WaveConfig waveConfig;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2f;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        MaxHullPoints = 200;
        hullPoints = 200;
        bulletSpeed = 400f;
        fireRate = 0.2f;
        nbrShots = 3;
        cooldown = 5.0f;
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Targeter();
        Fire("Static");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("FriendlyProjectile"))
        {
            TakingDamage(40);
        }
        if (collision.tag.Equals("ConstantDamageFriendlyProjectile"))
        {
            isTakingConstantDamages = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("ConstantDamageFriendlyProjectile"))
        {
            isTakingConstantDamages = false;
        }
    }
}
