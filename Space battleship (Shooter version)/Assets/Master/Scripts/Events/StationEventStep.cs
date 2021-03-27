using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StationEventStep : EventStep
{
    [Header("Station step")]
    public TurretDescription[] turretsUnlockable;

    private void Awake()
    {
        eventStepType = EventStepType.stationEventStep;
    }
}
