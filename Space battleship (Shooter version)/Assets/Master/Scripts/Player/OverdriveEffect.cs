using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OverdriveExecType { SingleUse, ContinuousUse }
public abstract class OverdriveEffect : ScriptableObject
{
    public OverdriveExecType overdriveExecType;
    public float overdriveCost;

    abstract public void overdriveExecution();
    abstract public void overdriveEndEffect();
}
