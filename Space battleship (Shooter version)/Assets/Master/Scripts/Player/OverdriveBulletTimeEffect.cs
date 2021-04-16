using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OverdriveBulletTimeEffect : OverdriveEffect
{
    public float slowdownFactor;

    public override void overdriveExecution()
    {
        Time.timeScale = slowdownFactor;
        VolumeEffectManager.current.activateVolumeEffect(0, false);
    }

    public override void overdriveEndEffect()
    {
        Time.timeScale = 1;
        Battleship.current.overdriveIsActive = false;
        VolumeEffectManager.current.deactivateVolumeEffect(0);
    }
}
