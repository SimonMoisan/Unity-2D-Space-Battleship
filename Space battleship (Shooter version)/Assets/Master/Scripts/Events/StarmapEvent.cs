using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StarmapEvent : ScriptableObject
{
    public int actualEventStepIndex;
    public bool eventIsComplete;
    public EventStep[] eventSteps;

    private void OnValidate()
    {
        eventIsComplete = false;
        actualEventStepIndex = 0;
    }
}
