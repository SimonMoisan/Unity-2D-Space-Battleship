using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorLink : MonoBehaviour
{
    public LineRenderer line;
    public Sector startSector;
    public Sector destinationSector;

    public void initiateLink(Sector start, Sector destination)
    {
        startSector = start;
        destinationSector = destination;
        line.SetPosition(0, start.transform.position);
        line.SetPosition(1, destination.transform.position);
    }
}
