using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ContextualEventStep : EventStep
{
    [Header("Contextual step")]
    public TextAsset storyJson;
    public Material contextualBackgroundMaterial;
    public Vector2 contextualBackgroundScaleRatios;
    public ContextualStepChoice[] contextualStepChoices;

    private void OnValidate()
    {
        eventStepType = EventStepType.contextualEventStep;
    }
}
