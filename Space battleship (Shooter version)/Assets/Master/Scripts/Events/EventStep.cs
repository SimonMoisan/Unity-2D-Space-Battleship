using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public enum EventStepType { contextualEventStep, battleEventStep, bossEventStep, stationEventStep }

public class EventStep : ScriptableObject
{
    [Header("Stats :")]
    public EventStepType eventStepType;
}
