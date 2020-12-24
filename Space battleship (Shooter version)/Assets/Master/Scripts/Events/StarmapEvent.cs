using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StarmapEvent : ScriptableObject
{
    public int actualEventStepIndex;
    public EventStep[] eventSteps;

    private void OnValidate()
    {
        actualEventStepIndex = 0;
    }
}
