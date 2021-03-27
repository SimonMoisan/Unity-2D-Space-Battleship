using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FighterBehavior : MonoBehaviour
{
    public AIDestinationSetter destinationSetter;
    public AIPath aIPath;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
    }

    void seek()
    {
        destinationSetter.target = target.transform;
        if(aIPath.reachedEndOfPath)
        {

        }
    }

    void flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        //agent.SetDestination(this.transform.position-  fleeVector);
    }

    // Update is called once per frame
    void Update()
    {
        //seek(target.transform.position);
    }
}
