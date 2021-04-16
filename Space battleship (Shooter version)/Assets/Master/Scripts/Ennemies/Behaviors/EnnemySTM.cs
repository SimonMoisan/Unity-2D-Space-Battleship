using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnnemySTM : MonoBehaviour
{
    private Dictionary<Type, BaseBehavior> availableBehavior;
    public BaseBehavior currentBehavior { get; private set; }
    public event Action<BaseBehavior> OnBehaviorChanged;

    public void setBehavior(Dictionary<Type, BaseBehavior> behaviors)
    {
        availableBehavior = behaviors;
    }

    private void Update()
    {
        if(currentBehavior == null)
        {
            currentBehavior = availableBehavior.Values.First();
        }

        var nextBehavior = currentBehavior?.tick();

        if (nextBehavior != null && nextBehavior != currentBehavior?.GetType())
        {
            switchToNewState(nextBehavior);
        }
    }

    private void switchToNewState(Type nextBehavior)
    {
        currentBehavior = availableBehavior[nextBehavior];
        OnBehaviorChanged?.Invoke(currentBehavior);
    }
}
