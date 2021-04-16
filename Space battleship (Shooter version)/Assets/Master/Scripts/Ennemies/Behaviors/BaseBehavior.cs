using System;
using UnityEngine;

public abstract class BaseBehavior
{
    protected GameObject gameObject;
    protected Transform transform;

    public BaseBehavior(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public abstract Type tick();
}
