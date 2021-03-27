using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ContextualStepChoice : ScriptableObject
{
    public int nextStepTransition; //if this choice lead to the next of an event, this value will be equals to the id of the next step or 10000 to end the event, either way it will be equals to -1
    public float hullMod;
    public int scrapMod;
    public int energyCoreMod;
}
