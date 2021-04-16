using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveBeahavior : BaseBehavior
{
    private Transform destination;
    public Transform[] waypoints;
    protected int waypointIndex = 0;

    public AIDestinationSetter destinationSetter;
    public AIPath aIPath;

    public MoveBeahavior(Ennemy ennemy) : base(ennemy.gameObject)
    {
        waypoints = ennemy.GetComponent<StandardEnnemy>().waypoints;

        destinationSetter = ennemy.destinationSetter;
        aIPath = ennemy.aIPath;
    }

    public override Type tick()
    {
        destination = waypoints[waypointIndex];

        if (waypointIndex <= waypoints.Length - 1)
        {
            if (aIPath.reachedEndOfPath)
            {
                waypointIndex++;
                destinationSetter.target = destination;
            }
        }
        else
        {
            return typeof(DepartureBehavior);
        }

        return typeof(MoveBeahavior);
    }

    public void setDestination(Transform newDestination)
    {
        destination = newDestination;
    }
}
