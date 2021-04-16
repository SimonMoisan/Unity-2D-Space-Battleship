using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class ArrivalBehavior : BaseBehavior
{
    //Speed stats
    public float actualSpeed;
    public float passiveMoveSpeed;
    public float arrivalSpeed;

    public AIPath aIPath;
    public AIDestinationSetter destinationSetter;
    public GameObject reactor;

    public Transform arrivalPosition;

    public bool arrivalCoroutineInitiated;
    public IEnumerator arrivalCoroutine;

    public ArrivalBehavior(Ennemy ennemy) : base(ennemy.gameObject)
    {
        passiveMoveSpeed = ennemy.passiveMoveSpeed;
        arrivalSpeed = ennemy.arrivalSpeed;
        actualSpeed = arrivalSpeed;

        reactor = ennemy.reactor;
        aIPath = ennemy.aIPath;
        destinationSetter = ennemy.destinationSetter;

        arrivalPosition = ennemy.GetComponent<StandardEnnemy>().waypoints[0];
    }

    public override Type tick()
    {
        destinationSetter.target = arrivalPosition;

        //Start
        if(!arrivalCoroutineInitiated)
        {
            arrivalCoroutineInitiated = true;
        }

        //End
        if (aIPath.reachedEndOfPath)
        {
            if(arrivalCoroutine != null)
            {
                
            }

            return typeof(MoveBeahavior);
        }

        return typeof(ArrivalBehavior);
    }
}
