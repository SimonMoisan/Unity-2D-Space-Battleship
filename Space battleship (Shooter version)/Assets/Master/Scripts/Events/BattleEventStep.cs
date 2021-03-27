using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleEventStep : EventStep
{
    [Header("Battle step :")]
    public int dangerIndicator;
    public int tier;
    public List<Wave> waves;

    private void OnValidate()
    {
        eventStepType = EventStepType.battleEventStep;
    }
}
