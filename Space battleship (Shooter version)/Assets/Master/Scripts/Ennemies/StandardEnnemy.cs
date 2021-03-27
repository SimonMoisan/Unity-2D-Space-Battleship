using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Pathfinding;

public class StandardEnnemy : Ennemy
{
    [Header("Associated objects : ")]
    public WaveConfig waveConfig;
    [ReadOnly] public Transform[] waypoints;
    [ReadOnly] protected int waypointIndex = 0;
    public AIDestinationSetter destinationSetter;
    public AIPath aIPath;

    private void OnValidate()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
    }

    protected new void Awake()
    {
        base.Awake();

        //Initiate attack management
        attacks = GetComponentsInChildren<EnnemyAttack>();
        coolDownUnits = new LinkedList<EnnemyAttack>[nbrSimAttack];
        for (int i = 0; i < coolDownUnits.Length; i++)
        {
            coolDownUnits[i] = new LinkedList<EnnemyAttack>();
        }

        //Enqueue every attacks
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i] != null)
            {
                enqueuAttack(attacks[i]);
            }
        }

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null)
        {
            //waypoints = waveConfig.GetWaypoints();
            transform.position = waypoints[waypointIndex].transform.position;
            destinationSetter.target = waypoints[waypointIndex].transform;
        }

        //Can't have less than one attack
        if (nbrSimAttack <= 0)
        {
            nbrSimAttack = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].Targeter();
        }

        AttackManagement();

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null && isMoving)
        {
            MoveCible();
        }
    }

    protected void MoveCible()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            /*var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);*/
            if (aIPath.reachedEndOfPath)
            {
                waypointIndex++;
                destinationSetter.target = waypoints[waypointIndex].transform;
            }
        }
        else
        {
            //Destroyed at the end of waypoints
            if (waveConfig.dieAtEnd)
            {
                ennemySpawner.EnnemyDestroyed();
                Destroy(gameObject);
            }
        }
    }
}
