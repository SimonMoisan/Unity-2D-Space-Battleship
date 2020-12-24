using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public enum EventStepType { contextualEventStep, battleEventStep, bossEventStep, stationEventStep }

[CreateAssetMenu(menuName = "Event Step")]
public class EventStep : ScriptableObject
{
    [Header("Stats :")]
    public EventStepType eventStepType;
    [Header("Battle step :")]
    public int dangerIndicator;
    public int tier;
    public List<Wave> waves;
    [Header("Contextual step")]
    public TextAsset storyJson;
    [Header("Station step")]
    public TurretDescritpion[] turretsUnlockable;
}
